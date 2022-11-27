using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMove : MonoBehaviour
{
    //** static ---------------------------
    private static LineMove s_LineMove;
    public static bool s_isLineMoving = false;
    public static int s_nowMs, s_nowPos, s_nowPower;

    //** public ---------------------------

    //** private ---------------------------
    private int lastPower;
    private int[] linePower = new int[2];
    private int lineIndex, triggerIndex;
    private bool isLineMoving = false;
    private bool isLineSingle = false;
    private bool isCoroutineAlive = false;
    private List<LineNote> lineNotes;
    private List<LineTriggerNote> triggerNotes;
    private IEnumerator lineCoroutine;
    private LineNote[] targetNote;
    private LineTriggerNote targetTriggerNote;

    //** Serialize ---------------------------
    [SerializeField] LineRenderer line;
    [SerializeField] GameObject PreviewNote;
    [SerializeField] GameObject PrefabObject;
    [SerializeField] GameObject[] LineObject;

    //** Unity Actions ---------------------------
    private void Awake()
    { 
        s_LineMove = this;
        PreviewNote.SetActive(false);
        ResetLineSystem();
        EndPlay();
    }
    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
    private void Update()
    {
        if (!s_isLineMoving) { return; }

        //* LineNote --------------------------------
        if (targetNote[0] != null)
        {
            if (targetNote[0].ms <= s_nowMs)
            {
                //* Initialize IEnumerator
                StopCoroutine(lineCoroutine);
                lineCoroutine = ICalculatePower();
                StartCoroutine(lineCoroutine);

                //* Initialize TargetNotes
                lineIndex++;
                if (lineIndex == lineNotes.Count) { targetNote[0] = null; }
                else { targetNote[0] = lineNotes[lineIndex]; }
                if (lineIndex + 1 >= lineNotes.Count) { targetNote[1] = null; }
                else { targetNote[1] = lineNotes[lineIndex + 1]; }

                //* Initialize linePower
                if (targetNote[0].isSingle) { linePower[0] = targetNote[0].startPower; }
                else { linePower[0] = targetNote[0].endPower; }
                if (targetNote[1] == null) { linePower[1] = linePower[0]; }
                else { linePower[1] = targetNote[1].startPower; }
            }
        }

        //* TriggerNote --------------------------------
        if (targetTriggerNote != null)
        {
            if (!isLineMoving) 
                { if (targetTriggerNote.startMs <= s_nowMs) { isLineMoving = true; } }
            else
            {
                if (targetTriggerNote.endMs <= s_nowMs)
                {
                    isLineMoving = false;
                    triggerIndex++;
                    if (triggerIndex == triggerNotes.Count) { targetTriggerNote = null; }
                    else { targetTriggerNote = triggerNotes[triggerIndex]; }
                }
            }
        }
    }
    private void LateUpdate()
    {
        if (!s_isLineMoving) { return; }
        LineTilting();
    }

    //** static void ---------------------------
    public static void ResetLineSystem()
    {
        s_LineMove.line.positionCount = 2;
        s_LineMove.line.SetPosition(0, new Vector3(0, 0, 0));
        s_LineMove.line.SetPosition(1, new Vector3(0, 1600000, 0));
    }
    public static void ReDrewLine()
    {
        ResetLineSystem();

        int _count = 1;
        for (int i = 0; i < LineNote.lineNotes.Count; i++)
        {
            if (LineNote.lineNotes[i].isSingle) { _count++; }
            else { _count += 2; }
        }
        s_LineMove.line.positionCount = _count;

        int _index = 1;

        for (int i = 0; i < LineNote.lineNotes.Count; i++)
        {
            LineNote _note;
            _note = LineNote.lineNotes[i];

            if (_note.isSingle)
            {
                s_LineMove.line.SetPosition(_index, new Vector3(_note.startPower, _note.pos, 0));
                _index++;
            }
            else
            {
                s_LineMove.line.SetPosition(_index, new Vector3(_note.startPower, _note.pos, 0));
                s_LineMove.line.SetPosition(_index + 1, new Vector3(_note.endPower, _note.pos, 0));
                _index += 2;
            }
        }
    }
    public static void PositionLine(float _pos)
    {
        s_LineMove.line.transform.localPosition = new Vector3(0, -_pos, 0);
    }
    public static void ReadyForPlay()
    {
        s_isLineMoving = true;
        s_LineMove.ResetData();
    }
    public static void EndPlay()
    {
        s_isLineMoving = false;
        s_LineMove.ResetData();
    }

    //** public void ---------------------------

    //** private void ---------------------------
    private void ResetData()
    {
        lineNotes = new List<LineNote>();
        triggerNotes = new List<LineTriggerNote>();

        for (int i = 0; i < LineNote.lineNotes.Count; i++)
        {
            LineNote _copyNote;
            _copyNote = LineNote.CopyLineNoteDate(LineNote.lineNotes[i], false);
            lineNotes.Add(_copyNote);
        }
        for (int i = 0; i < LineTriggerNote.triggerNotes.Count; i++)
        {
            LineTriggerNote _copyNote;
            _copyNote = LineTriggerNote.CopyNoteData(LineTriggerNote.triggerNotes[i], false);
            triggerNotes.Add(_copyNote);
        }

        s_nowPower = 0;
        lastPower = 0;
        LineTilting();

        if (lineNotes.Count == 0) { targetNote = null; }
        else { targetNote[0] = lineNotes[0]; }
        if (triggerNotes.Count == 0) { targetTriggerNote = null; }
        else { targetTriggerNote = triggerNotes[0]; }
        lineIndex = 0;
        triggerIndex = 0;
        isLineMoving = true; //! Debugging Code
    }
    private void LineTilting()
    {
        if (!isLineMoving) { return; }

        s_nowPower = Mathf.Clamp(-500, 500, s_nowPower);
        LineObject[0].transform.localPosition = new Vector3(s_nowPower / 200.0f, 15, 30);
        LineObject[0].transform.localRotation = Quaternion.Euler(-20, 0, s_nowPower / 20.0f);
    }
    private IEnumerator ICalculatePower()
    {
        int _startMs = 0, _endMs = 0, _runMs = 0;

        _startMs = targetNote[0].ms;

        if (!targetNote[0].isSingle)
        {
            if (targetNote[1] == null) { yield break; }
            else if (targetNote[0].startPower == targetNote[1].startPower) { yield break; }
            else { _endMs = targetNote[1].ms - targetNote[0].ms; }

            while (true)
            {
                _runMs = s_nowMs - _startMs;

                s_nowPower = Mathf.RoundToInt(Mathf.Lerp(targetNote[0].startPower,
                    targetNote[1].startPower, (_runMs * 1.0f) / _endMs));

                if (s_nowMs - _startMs >= 500)
                {
                    s_nowPower = targetNote[0].endPower;
                    break;
                }
                yield return null;
            }
        }
        else
        {
            while (true)
            {
                _runMs = s_nowMs - _startMs;

                s_nowPower = Mathf.RoundToInt(Mathf.Lerp(targetNote[0].startPower,
                    targetNote[0].endPower, (_runMs * 1.0f) / 500));

                if (s_nowMs - _startMs >= 500)
                {
                    s_nowPower = targetNote[0].endPower;
                    break;
                }
                yield return null;
            }

            if (targetNote[1] == null) { yield break; }
            else if (targetNote[0].endPower == targetNote[1].startPower) { yield break; }
            else { _endMs = targetNote[1].ms - targetNote[0].ms; }

            while (true)
            {
                _runMs = s_nowMs - _startMs;

                s_nowPower = Mathf.RoundToInt(Mathf.Lerp(targetNote[0].endPower,
                    targetNote[1].startPower, (_runMs * 1.0f) / _endMs));

                if (s_nowMs >= _endMs)
                {
                    s_nowPower = targetNote[0].endPower;
                    break;
                }
                yield return null;
            }
        }
    }
}
