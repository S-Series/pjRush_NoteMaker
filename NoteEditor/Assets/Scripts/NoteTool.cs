using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTool : MonoBehaviour
{
    InputManager input;

    private void Start()
    {
        input = InputManager.input;
    }

    public void ButtonChip()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = false;
        input.InputNoteData[2] = 0;
    }

    public void ButtonLong()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = false;
        input.InputNoteData[2] = 1;
    }

    public void ButtonBtChip()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = true;
        input.InputNoteData[2] = 2;
    }

    public void ButtonBtLong()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = true;
        input.InputNoteData[2] = 3;
    }
}
