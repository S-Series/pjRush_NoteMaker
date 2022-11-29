using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LineEdit : MonoBehaviour
{
    public static LineEdit s_lineEdit;
    public static bool s_isLineEdit = false;
    public static LineNote s_selectLineNote;
    public Camera[] lineCamera;
    private LineOption selectLineOption;
    [SerializeField] private GameObject prefabObject;

    #region Side Sector Objects
    [SerializeField] private GameObject editSector;
    [SerializeField] private TMP_InputField[] inputFields;
    [SerializeField] private Toggle toggle;
    #endregion

    void Awake()
    {
        s_lineEdit = this;
        SectorActivate(false);
    }
    void Update()
    {
        if (!s_isLineEdit) { return; }

        if (Input.GetKeyDown(KeyCode.Escape)) { DeselectNote(); return; }
        if (Input.GetKeyDown(KeyCode.UpArrow)) { MoveNote(_isUp: true); }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { MoveNote(_isUp: false); }
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.F)) { ChangeSingle(); }
        if (Input.GetKeyDown(KeyCode.Delete)) { DeleteNote(); return; }
    }

    public static void DeselectNote()
    {
        if (!s_isLineEdit) { return; }
        s_selectLineNote.noteObject.GetComponentInChildren<BoxCollider2D>().enabled = true;
        s_isLineEdit = false;
        s_lineEdit.selectLineOption.Selected(false);
        s_selectLineNote = null;
        s_lineEdit.selectLineOption = null;
        NoteClasses.EnableCollider(true);
        SectorActivate(false);
    }
    public static void SelectLineNote(GameObject _object)
    {
        if (s_isLineEdit) { DeselectNote(); }

        s_selectLineNote = LineNote.GetLineNote(_object);
        s_lineEdit.selectLineOption = _object.GetComponent<LineOption>();
        s_lineEdit.selectLineOption.Selected(true);
        s_isLineEdit = true;
        _object.GetComponentInChildren<BoxCollider2D>().enabled = false;

        SectorActivate(true);
    }
    public static void ChangePower(bool _isStart, int _value)
    {
        if (!s_isLineEdit) { return; }
        if (s_selectLineNote == null) { return; }
        if (s_lineEdit.selectLineOption == null) { return; }

        if (_isStart) { s_selectLineNote.startPower = _value; }
        else { s_selectLineNote.endPower = _value; }
        LineMove.ReDrewLine();
        s_lineEdit.selectLineOption.UpdateSliderInfo(s_selectLineNote);
    }
    public static void SectorActivate(bool _isActive)
    {
        if (!s_isLineEdit) { return; }
        if (s_selectLineNote == null) { return; }
        if (s_lineEdit.selectLineOption == null) { return; }

        s_lineEdit.editSector.SetActive(_isActive);
        s_lineEdit.ActiveEndInput(s_selectLineNote.isSingle);
    }
    private void MoveNote(bool _isUp)
    {
        if (!s_isLineEdit) { return; }
        if (s_selectLineNote == null) { return; }
        if (selectLineOption == null) { return; }



        LineMove.ReDrewLine();
    }
    private void ChangeSingle()
    {
        if (!s_isLineEdit) { return; }
        if (s_selectLineNote == null) { return; }
        if (selectLineOption == null) { return; }

        bool _reversed;
        _reversed = !s_selectLineNote.isSingle;
        s_selectLineNote.isSingle = _reversed;
        selectLineOption.UpdateSliderInfo(s_selectLineNote);

        ActiveEndInput(s_selectLineNote.isSingle);
        LineMove.ReDrewLine();
    }
    private void DeleteNote()
    {
        Destroy(s_selectLineNote.noteObject);
        LineNote.DeleteNote(s_selectLineNote);
        DeselectNote();
        LineMove.ReDrewLine();
    }
    private void ActiveEndInput(bool _isSingle)
    {
        if (_isSingle) { inputFields[3].interactable = false; }
        else { inputFields[3].interactable = true; }
    }
    private void UpdateSectorInfo()
    {
        if (s_selectLineNote == null) { return; }

        inputFields[0].text = (s_selectLineNote.pos % 1600).ToString();
        inputFields[1].text = (Mathf.FloorToInt(s_selectLineNote.pos / 1600.0f)).ToString();
        inputFields[2].text = s_selectLineNote.startPower.ToString();
        ActiveEndInput(s_selectLineNote.isSingle);
        if (!s_selectLineNote.isSingle) { inputFields[3].text = "---"; }
        else { inputFields[3].text = s_selectLineNote.endPower.ToString();}

        toggle.isOn = s_selectLineNote.isSingle;
    }

    //* Sector Action
    public void ToggleSingle() //$ Along to toggle
    {
        if (s_selectLineNote == null) { return; }
        ChangeSingle();
        UpdateSectorInfo();
    }
    public void InputPos() //$ Along to inputFields[0]
    {
        if (s_selectLineNote == null) { return; }

        int _inputPos;
        int _notePage;
        _notePage = Mathf.FloorToInt(s_selectLineNote.pos / 1600.0f);

        try { _inputPos = Convert.ToInt32(inputFields[0].text); }
        catch { _inputPos = s_selectLineNote.pos; }
        _inputPos = Mathf.Clamp(_inputPos, 0, 1600);

        s_selectLineNote.pos = _inputPos;
        UpdateSectorInfo();
    }
    public void InputPage() //$ Along to inputFields[1]
    {
        if (s_selectLineNote == null) { return; }

        int _inputPage;
        int _notePos;
        _notePos = Mathf.FloorToInt(s_selectLineNote.pos % 1600.0f);

        try { _inputPage = Convert.ToInt32(inputFields[1].text); }
        catch { _inputPage = s_selectLineNote.pos; }

        s_selectLineNote.pos = _inputPage;
        UpdateSectorInfo();
    }
    public void InputPower(bool _isStart) //$ Along to inputFields[2] & inputFields[3]
    {
        if (s_selectLineNote == null) { return; }

        int _inputPower;

        if (_isStart)
        {
            try { _inputPower = Convert.ToInt32(inputFields[2].text); }
            catch { _inputPower = s_selectLineNote.startPower; }

            inputFields[1].text = _inputPower.ToString();
        }
        else
        {
            try { _inputPower = Convert.ToInt32(inputFields[3].text); }
            catch { _inputPower = s_selectLineNote.endPower; }

            inputFields[2].text = _inputPower.ToString();
        }

        ChangePower(_isStart, _inputPower);
        UpdateSectorInfo();
    }
}
