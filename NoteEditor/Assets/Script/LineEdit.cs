using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEdit : MonoBehaviour
{
    public static LineEdit s_lineEdit;
    public static bool s_isLineEdit = false;
    public static LineNote s_selectLineNote;
    public Camera[] lineCamera;
    private LineOption selectLineOption;
    [SerializeField] private GameObject prefabObject;

    void Awake()
    {
        s_lineEdit = this;
    }
    void Update()
    {
        if (!s_isLineEdit) { return; }

        if (Input.GetKeyDown(KeyCode.Escape)) { DeselectNote(); return; }
        if (Input.GetKeyDown(KeyCode.UpArrow)) { MoveNote(_isUp: true); }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { MoveNote(_isUp: false); }
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.F)) { ChangeDuration(); }
        if (Input.GetKeyDown(KeyCode.Delete)) { DeleteNote(); return; }
    }

    public static void DeselectNote()
    {
        if (!s_isLineEdit) { return; }
        s_selectLineNote.noteObject.GetComponentInChildren<BoxCollider2D>().enabled = true;
        s_isLineEdit = false;
        s_lineEdit.selectLineOption.Selected(false, s_selectLineNote);
        s_selectLineNote = null;
        s_lineEdit.selectLineOption = null;

    }
    public static void SelectLineNote(GameObject _object)
    {
        if (s_isLineEdit) { DeselectNote(); }

        s_selectLineNote = LineNote.GetLineNote(_object);
        s_lineEdit.selectLineOption = _object.GetComponent<LineOption>();
        s_lineEdit.selectLineOption.Selected(true, s_selectLineNote);
        s_lineEdit.selectLineOption
            .DurationAvailable(s_selectLineNote.isHasDuration, s_selectLineNote.duration);
        if (!s_selectLineNote.isHasDuration) 
            { s_lineEdit.selectLineOption.ChangeLegnth(0); }
        else 
            { s_lineEdit.selectLineOption.ChangeLegnth(s_selectLineNote.duration);}
        s_isLineEdit = true;
        _object.GetComponentInChildren<BoxCollider2D>().enabled = false;
    }
    public void PowerNote(bool _isStart, int _value)
    {
        if (!s_isLineEdit) { return; }
        if (s_selectLineNote == null) { return; }
        if (selectLineOption == null) { return; }

        if (_isStart) { s_selectLineNote.power = _value; }
        else { s_selectLineNote.durationPower = _value; }
    }
    public void DurationNote(float _value)
    {
        if (!s_isLineEdit) { return; }
        if (s_selectLineNote == null) { return; }
        if (selectLineOption == null) { return; }

        s_selectLineNote.duration = _value;
    }
    private void MoveNote(bool _isUp)
    {
        if (!s_isLineEdit) { return; }
        if (s_selectLineNote == null) { return; }
        if (selectLineOption == null) { return; }

        Vector3 pos;
        pos = new Vector3(0, s_selectLineNote.pos, 0);

        int count;
        float inputPos;

        if (_isUp)
        {
            count = Mathf.FloorToInt(pos.y / (1600.0f / GuideGenerate.GuideCount)) + 1;
            inputPos = Mathf.Floor(1600.0f / GuideGenerate.GuideCount * 100) * count / 100;
            if (inputPos >= 1600.0f * 999) inputPos = 1600.0f * 999;

            while (LineNote.IsNoteOverlap(inputPos))
            {
                count = Mathf.FloorToInt(pos.y / (1600.0f / GuideGenerate.GuideCount)) + 1;
                inputPos = Mathf.Floor(1600.0f / GuideGenerate.GuideCount * 100) * count / 100;
                if (inputPos >= 1600.0f * 999) inputPos = 1600.0f * 999;
            }
        }
        else
        {
            count = Mathf.CeilToInt(pos.y / (1600.0f / GuideGenerate.GuideCount)) - 1;
            inputPos = Mathf.Ceil(1600.0f / GuideGenerate.GuideCount * 100) * count / 100;
            if (inputPos < 0.0f) inputPos = 0.0f;

            while (LineNote.IsNoteOverlap(inputPos))
            {
                count = Mathf.CeilToInt(pos.y / (1600.0f / GuideGenerate.GuideCount)) - 1;
                inputPos = Mathf.Ceil(1600.0f / GuideGenerate.GuideCount * 100) * count / 100;
                if (inputPos < 0.0f) inputPos = 0.0f;
            }
        }
        pos.y = inputPos;
        s_selectLineNote.pos = inputPos;
        s_selectLineNote.noteObject.transform.localPosition = pos;
    }
    private void ChangeDuration()
    {
        if (!s_isLineEdit) { return; }
        if (s_selectLineNote == null) { return; }
        if (selectLineOption == null) { return; }

        s_selectLineNote.isHasDuration = !s_selectLineNote.isHasDuration;
        selectLineOption.DurationAvailable(s_selectLineNote.isHasDuration, s_selectLineNote.duration);
        LineMove.ReDrewLine();
    }
    private void DeleteNote()
    {
        Destroy(s_selectLineNote.noteObject);
        LineNote.DeleteNote(s_selectLineNote);
        DeselectNote();
        LineMove.ReDrewLine();
    }
}
