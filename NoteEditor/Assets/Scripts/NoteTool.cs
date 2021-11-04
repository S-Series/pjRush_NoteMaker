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
        input.InputObject = input.PreviewNote[0];
        input.InputNoteData[2] = 0;
    }

    public void ButtonLong()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = false;
        input.InputObject = input.PreviewNote[1];
        input.InputNoteData[2] = 1;
    }

    public void ButtonBtChip()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = true;
        input.InputObject = input.PreviewNote[2];
        input.InputNoteData[2] = 2;
    }

    public void ButtonBtLong()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = true;
        input.InputObject = input.PreviewNote[3];
        input.InputNoteData[2] = 3;
    }

    public void ButtonEffect()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = true;
        input.InputObject = input.PreviewNote[4];
        input.InputNoteData[2] = 4;
    }

    public void ButtonBpm()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = true;
        input.InputObject = input.PreviewNote[5];
        input.InputNoteData[2] = 5;
    }
}
