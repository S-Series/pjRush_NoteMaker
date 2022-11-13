using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOption : MonoBehaviour
{
    [SerializeField] PowerOption powerOption;
    [SerializeField] SpriteRenderer[] noteRenderer;
    
    public void ChangeLegnth(float _posY)
    {
        if (_posY == 0)
        {
            foreach(SpriteRenderer _renderer in noteRenderer) { _renderer.enabled = false; }
        }
        else
        {
            foreach(SpriteRenderer _renderer in noteRenderer) { _renderer.enabled = true; }
            noteRenderer[0].transform.localScale = new Vector3(48.5f, _posY / 1600.0f * 159.25f, 1);
            noteRenderer[1].transform.localPosition = new Vector3(0, _posY, 0);
            powerOption.ChangePosition(_posY);
        }
    }
    public void Selected(bool _isSelected, LineNote _note)
    {
        powerOption.ActivateSlider(_isSelected, _note);
        if (!_isSelected) { ChangeLegnth(0); }
    }
    public void DurationAvailable(bool _isAvailable, float _value = 0)
    {
        powerOption.DurationAvailable(_isAvailable, Convert.ToInt32(_value));
    }
}
