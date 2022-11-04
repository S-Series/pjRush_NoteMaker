using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOption : MonoBehaviour
{
    public Canvas canvas;
    [SerializeField] SpriteRenderer[] noteRenderer;
    
    public void ChangeLegnth(int _legnth)
    {
        if (_legnth == 0)
        {
            noteRenderer[1].enabled = false;
            noteRenderer[2].enabled = false;
        }
    }
    public void Selected(bool _isSelected)
    {
        foreach(SpriteRenderer renderer in noteRenderer)
        {
            renderer.enabled = _isSelected;
        }
    }
}
