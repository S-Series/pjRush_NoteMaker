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
    private int lineMs, lastPower;
    private int[] linePower = new int[2];
    private int lineIndex, triggerIndex;
    private bool isLineMoving = false;
    private bool isLineSingle = false;
    private bool isCoroutineAlive = false;
    private List<LineNote> lineNotes;
    private List<LineTriggerNote> triggerNotes;
    private IEnumerator lineCoroutine;
    private LineNote[] targetNote = new LineNote[2];
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
        lineCoroutine = ICalculatePower();
        ResetLineSystem();
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
            if (lineMs <= s_nowMs)
            {
                //* Initialize TargetNotes
                UpdateTargetNote();

                //* Initialize IEnumerator
                StopCoroutine(lineCoroutine);
                lineCoroutine = ICalculatePower();
                StartCoroutine(lineCoroutine);
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

        s_LineMove.LineObject[0].transform
            .localPosition = new Vector3(0 / 200.0f, 15, 30);
        s_LineMove.LineObject[0].transform
            .localRotation = Quaternion.Euler(-20, 0, 0);
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
    private void UpdateTargetNote()
    {
        try { targetNote[0] = lineNotes[lineIndex]; }
        catch { targetNote[0] = null; }
        try { targetNote[1] = lineNotes[lineIndex + 1]; }
        catch { targetNote[1] = null; }
        try { lineMs = targetNote[1].ms; }
        catch { lineMs = 9999999; }
        lineIndex++;
    }
    private void ResetData()
    {
        StopCoroutine(lineCoroutine);

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

        if (lineNotes.Count == 0) { targetNote[0] = null; lineMs = 9999999; }
        else { targetNote[0] = lineNotes[0]; lineMs = targetNote[0].ms;}
        if (lineNotes.Count >= 1) { targetNote[1] = lineNotes[1]; }

        if (triggerNotes.Count == 0) { targetTriggerNote = null; }
        else { targetTriggerNote = triggerNotes[0]; }
        
        lineIndex = 0;
        triggerIndex = 0;
        isLineMoving = true; //! Debugging Code
    }
    private void LineTilting()
    {
        if (!isLineMoving) { return; }

        s_nowPower = Mathf.Clamp(s_nowPower, -500, 500);
        LineObject[0].transform.localPosition = new Vector3(s_nowPower / 400.0f, 15, 30);
        LineObject[0].transform.localRotation = Quaternion.Euler(-20, 0, s_nowPower / 40.0f);
    }
    private float Sigmoid(float value)
        { 
            print(1.0f / (1.0f + Mathf.Exp(-6.75f * (value - 0.5f))));
            return 1.0f / (1.0f + Mathf.Exp(-6.75f * (value - 0.5f))); }
    private IEnumerator ICalculatePower()
    {
        float[] _targetPower = new float[2];

        if (targetNote[0].isSingle)
        {
            if (targetNote[1] == null) { print("A"); yield break; }
            if (targetNote[0].startPower == targetNote[1].startPower) { print("B"); yield break; }
            _targetPower[0] = targetNote[0].startPower;
            _targetPower[1] = targetNote[1].startPower;
        }
        else
        {
            float _timer = 0.0f;
            while (true)
            {
                _timer += Time.deltaTime;
                _timer = Mathf.Clamp(_timer, 0, 0.05f);
                s_nowPower = Mathf.RoundToInt(Mathf.Lerp(
                    targetNote[0].startPower, targetNote[0].endPower, Sigmoid(_timer * 20)));
                if (_timer == 0.05f) { break;}

                yield return null;
            }
            if (targetNote[0].endPower == targetNote[1].startPower) { yield break; }
            _targetPower[0] = targetNote[0].endPower;
            _targetPower[1] = targetNote[1].startPower;
        }

        float _startMs = 0, _endMs = 0, _runMs = 0;
        _startMs = targetNote[0].ms;
        _endMs = targetNote[1].ms - _startMs;

        while (true)
        {
            _runMs = s_nowMs - _startMs;
            s_nowPower = Mathf.RoundToInt(Mathf.Lerp(
                _targetPower[0], _targetPower[1], Sigmoid(_runMs / _endMs)));

            if (_runMs / _endMs >= 1.0)
            {
                s_nowPower = Mathf.RoundToInt(_targetPower[1]);
                break;
            }
            yield return null;
        }
    }
}
