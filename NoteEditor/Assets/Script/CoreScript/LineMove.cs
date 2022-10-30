using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMove : MonoBehaviour
{
    //** static ---------------------------
    private static LineMove s_LineMove;
    public static bool s_isLineMoving = false;

    //** public ---------------------------

    //** private ---------------------------
    private int targetLineNoteMs;
    private int targetLineNoteIndex;
    private bool isLineMoving = false;

    //** Serialize ---------------------------
    [SerializeField] LineRenderer line;
    [SerializeField] GameObject PreviewNote;
    [SerializeField] GameObject PrefabObject;
    
    //** Unity Actions ---------------------------
    private void Awake()
    {
        s_LineMove = this;
        PreviewNote.SetActive(false);
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

        if (targetLineNoteMs <= JudgeSystem.s_playMs)
        {

        }
    }
    private void LateUpdate()
    {
        if (!s_isLineMoving) { return; }
        if (!isLineMoving) { return; }
    }
    
    //** static void ---------------------------
    public static void ResetLineSystem()
    {
        s_LineMove.line.positionCount = 2;
        s_LineMove.line.SetPosition(0, new Vector3(0, 0, 0));
        s_LineMove.line.SetPosition(1, new Vector3(0, 159999, 0));
    }
    public static void ReDrewLine()
    {
        ResetLineSystem();

        int _count = 2;
        for (int i = 0; i < LineNote.lineNotes.Count; i++)
        {
            if (LineNote.lineNotes[i].isHasDuration)
            {
                if (i == LineNote.lineNotes.Count - 1)
                {
                    _count += 2;
                }
                else if (Mathf.Approximately(LineNote.lineNotes[i + 1].pos,
                    LineNote.lineNotes[i].pos + LineNote.lineNotes[i].duration))
                {
                    if (LineNote.lineNotes[i + 1].isHasDuration
                        || LineNote.lineNotes[i + 1].power == 0)
                    {
                        _count++;
                    }
                    else { _count += 2; }
                }
                else { _count += 2; }
            }
            else { _count += 2; }
        }
        s_LineMove.line.positionCount = _count;

        int _index = 1;
        float _lastPower = 0;
        for (int i = 0; i < LineNote.lineNotes.Count; i++)
        {
            LineNote _target;
            _target = LineNote.lineNotes[i];

            if (_target.isHasDuration)
            {
                if (i == LineNote.lineNotes.Count - 1 || Mathf.Approximately
                    (LineNote.lineNotes[i + 1].pos, _target.pos + _target.duration))
                {

                }
            }
            else
            {
                s_LineMove.line.SetPosition
                    (_index, new Vector3(_lastPower, _target.pos, 0));
                s_LineMove.line.SetPosition
                    (_index + 1, new Vector3(_target.power, _target.pos, 0));

                _index += 2;
                _lastPower = _target.power;
            }
        }
    }

    //** public void ---------------------------

    //** private void ---------------------------

}
