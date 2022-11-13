using System;
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
        s_LineMove.line.SetPosition(1, new Vector3(0, 1600000, 0));
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
                    if (LineNote.lineNotes[i].durationPower == 666) { _count += 2; }
                    else { _count += 3; }
                }
                else if (Mathf.Approximately(LineNote.lineNotes[i + 1].pos,
                    LineNote.lineNotes[i].pos + LineNote.lineNotes[i].duration))
                {
                    if (LineNote.lineNotes[i + 1].isHasDuration
                        || LineNote.lineNotes[i + 1].power == 0)
                    {
                        if (LineNote.lineNotes[i].durationPower == 666) { _count++; }
                        else { _count += 2; }
                    }
                    else { _count += 2; }
                }
                else if (LineNote.lineNotes[i].durationPower != 666) { _count += 3; }
                else { _count += 2; }
            }
            else { _count += 2; }
        }
        s_LineMove.line.positionCount = _count;

        int _index = 1;
        bool _isSame;
        float _lastPower = 0;

        for (int i = 0; i < LineNote.lineNotes.Count; i++)
        {
            LineNote _target;
            _target = LineNote.lineNotes[i];

            if (_target.isHasDuration)
            {
                if (i + 1 == LineNote.lineNotes.Count) { _isSame = false; }
                else if (Mathf.Approximately
                    (LineNote.lineNotes[i + 1].pos, _target.pos + _target.duration)) { _isSame = true; }
                else { _isSame = false; }

                if (_isSame)
                {
                    if (LineNote.lineNotes[i].durationPower == 666)
                    {
                        s_LineMove.line.SetPosition
                            (_index, new Vector3(_target.power, _target.pos + _target.duration, 0));
                        _index++;
                        _lastPower = Convert.ToSingle(_target.power);
                    }
                    else
                    {
                        s_LineMove.line.SetPosition(_index,
                            new Vector3(Convert.ToSingle(_target.durationPower), 
                            _target.pos, 0));
                        s_LineMove.line.SetPosition(_index + 1,
                            new Vector3(Convert.ToSingle(_target.power),
                            _target.pos + _target.duration, 0));

                        _index += 2;
                        _lastPower = Convert.ToSingle(_target.power);
                    }
                }
                else //*if (!_isSame)
                {
                    if (LineNote.lineNotes[i].durationPower == 666)
                    {
                        s_LineMove.line.SetPosition
                            (_index, new Vector3(_lastPower, _target.pos, 0));
                        s_LineMove.line.SetPosition(_index + 1,
                            new Vector3(Convert.ToSingle(_target.power),
                            _target.pos + _target.duration, 0));

                        _index += 2;
                        _lastPower = Convert.ToSingle(_target.power);
                    }
                    else
                    {
                        s_LineMove.line.SetPosition(_index,
                            new Vector3(_lastPower, _target.pos, 0));
                        s_LineMove.line.SetPosition(_index + 1,
                            new Vector3(Convert.ToSingle(_target.durationPower),
                            _target.pos, 0));
                        s_LineMove.line.SetPosition(_index + 2,
                            new Vector3(Convert.ToSingle(_target.power),
                            _target.pos + _target.duration, 0));

                        _index += 3;
                        _lastPower = Convert.ToSingle(_target.power);
                    }
                }
            }
            else
            {
                s_LineMove.line.SetPosition
                    (_index, new Vector3(_lastPower, _target.pos, 0));
                s_LineMove.line.SetPosition
                    (_index + 1, new Vector3(Convert.ToSingle(_target.power), _target.pos, 0));

                _index += 2;
                _lastPower = Convert.ToSingle(_target.power);
            }
        }
        s_LineMove.line.SetPosition
            (s_LineMove.line.positionCount - 1, new Vector3(_lastPower, 1600000, 0));
    }
    public static void PositionLine(float _pos)
    {
        s_LineMove.line.transform.localPosition = new Vector3(0, -_pos, 0);
    }

    //** public void ---------------------------

    //** private void ---------------------------

}
