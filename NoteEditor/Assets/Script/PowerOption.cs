using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerOption : MonoBehaviour
{
    private bool isHasDuration = false;
    [SerializeField] LineOption lineOption;
    [SerializeField] private Slider[] slider;
    [SerializeField] private TMP_InputField[] inputField;
    [SerializeField] private GameObject sliderObject;
    public void StartSliderChange()
    {
        LineEdit.s_lineEdit.PowerNote(!isHasDuration, Convert.ToInt32(slider[0].value));
        inputField[0].text = slider[0].value.ToString();
        LineMove.ReDrewLine();
    }
    public void EndSliderChange()
    {
        LineEdit.s_lineEdit.PowerNote(isHasDuration, Convert.ToInt32(slider[1].value));
        inputField[2].text = slider[1].value.ToString();
        LineMove.ReDrewLine();
    }
    public void StartInputChange()
    {
        try
        {
            int _value;
            _value = Convert.ToInt32(inputField[0].text);
            if (_value > 500) { _value = 500; }
            if (_value < -500) { _value = -500; }
            slider[0].value = _value;
            StartSliderChange();
        }
        catch
        {
            inputField[0].text = slider[0].value.ToString();
        }
    }
    public void EndInputChange()
    {
        try
        {
            int _value;
            _value = Convert.ToInt32(inputField[2].text);
            if (_value > 500) { _value = 500; }
            if (_value < -500) { _value = -500; }
            slider[1].value = _value;
            EndSliderChange();
        }
        catch
        {
            inputField[2].text = slider[1].value.ToString();
        }
    }
    public void DurationAvailable(bool _isAvailable, int _value)
    {
        isHasDuration = _isAvailable;
        slider[1].gameObject.SetActive(_isAvailable);
        inputField[1].gameObject.SetActive(_isAvailable);
        inputField[1].text = _value.ToString();
    }
    public void DurationChange()
    {
        int _value;
        try
        {
            _value = Convert.ToInt32(inputField[1].text);
            if (_value < 0) { _value = 0; }
            if (_value > 9999) { _value = 9999; }
        }
        catch
        {
            _value = 0;
        }
        inputField[1].text = _value.ToString();
        LineEdit.s_lineEdit.DurationNote(_value);
        LineMove.ReDrewLine();
    }
    public void ActivateSlider(bool _isActive, LineNote _note = null)
    {
        slider[0].enabled = _isActive;
        sliderObject.SetActive(_isActive);
        if (!_note.isHasDuration) { slider[0].value = _note.power; }
        else { slider[0].value = _note.durationPower; }

        slider[1].enabled = _note.isHasDuration;
        slider[1].gameObject.SetActive(_note.isHasDuration);
        if (!_note.isHasDuration) { slider[1].value = _note.durationPower; }
        else { slider[1].value = _note.power; }

    }
    public void DefualtStartDuration()
    {
        LineEdit.s_lineEdit.PowerNote(false, 666);
        inputField[0].text = "- - -";
        LineMove.ReDrewLine();
    }
    public void ChangePosition(float _pos)
    {
        float _posY;
        _posY = _pos / 1.441442f;
        slider[1].gameObject.transform.localPosition = new Vector3(0, _posY, 0);
        slider[1].GetComponentInChildren<Button>()
            .gameObject.transform.localPosition = new Vector3(0, -_posY - 140, 0);
    }
}
