using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LineOption : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject[] SliderObject;

    public void SliderChange(bool _isStart)
    {
        int index, value;
        if (_isStart) { index = 0; }
        else { index = 1; }

        value = Mathf.RoundToInt(SliderObject[index].GetComponent<Slider>().value);
        SliderObject[index].GetComponentInChildren<TMP_InputField>().text = value.ToString();

        LineEdit.ChangePower(_isStart, value);
    }
    public void InputValueChange(bool _isStart)
    {
        int index, value;
        if (_isStart) { index = 0; }
        else { index = 1; }

        try
        {
            value = Convert.ToInt32(SliderObject[index].GetComponentInChildren<TMP_InputField>().text);
        }
        catch { value = Mathf.RoundToInt(SliderObject[index].GetComponent<Slider>().value); }

        SliderObject[index].GetComponentInChildren<TMP_InputField>().text = value.ToString();

        LineEdit.ChangePower(_isStart, value);
    }
    public void Selected(bool _isSelected)
    {
        canvas.gameObject.SetActive(_isSelected);
    }
    public void UpdateSliderInfo(LineNote _note)
    {
        SliderObject[1].SetActive(!_note.isSingle);

        SliderObject[0].GetComponent<Slider>().value = _note.startPower;
        SliderObject[0].GetComponentInChildren<TMP_InputField>().text = _note.startPower.ToString();

        SliderObject[1].GetComponent<Slider>().value = _note.endPower;
        SliderObject[1].GetComponentInChildren<TMP_InputField>().text = _note.endPower.ToString();
    }
    public void EnableCollider(bool _isEnable)
    {
        GetComponent<BoxCollider2D>().enabled = _isEnable;
    }
}
