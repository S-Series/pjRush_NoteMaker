using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTool : MonoBehaviour
{
    InputManager input;
    private int triggerIndex = 0;
    private void Start()
    {
        input = InputManager.input;
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)){
            if (AutoTest.autoTest.isTest) return;
            if (NoteEdit.noteEdit.isNoteEdit) return;
            if (TestPlay.isPlay || TestPlay.isPlayReady) return;
            if (!input.isNoteInputAble){
                ButtonChip();
            }
            else{
                switch (triggerIndex){
                    case 0:
                        ButtonLong();
                        break;

                    case 1:
                        ButtonBtChip();
                        break;

                    case 2:
                        ButtonBtLong();
                        break;

                    case 3:
                        ButtonBpm();
                        //ButtonEffect();
                        break;

                    case 4:
                        // ButtonBpm();
                        // EffectNote, None Use Yet
                        break;

                    case 5:
                        ButtonChip();
                        break;
                }
            }
        }
    }
    public void ButtonChip()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = false;
        input.InputObject = input.PreviewNote[0];
        input.InputNoteData[2] = 0;
        triggerIndex = 0;
    }
    public void ButtonLong()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = false;
        input.InputObject = input.PreviewNote[1];
        input.InputNoteData[2] = 1;
        triggerIndex = 1;
    }
    public void ButtonBtChip()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = true;
        input.InputObject = input.PreviewNote[2];
        input.InputNoteData[2] = 2;
        triggerIndex = 2;
    }
    public void ButtonBtLong()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = true;
        input.InputObject = input.PreviewNote[3];
        input.InputNoteData[2] = 3;
        triggerIndex = 3;
    }
    public void ButtonEffect()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = true;
        input.InputObject = input.PreviewNote[4];
        input.InputNoteData[2] = 4;
        triggerIndex = 4;
    }
    public void ButtonBpm()
    {
        input.isNoteInputAble = true;
        input.isNoteBottom = true;
        input.InputObject = input.PreviewNote[5];
        input.InputNoteData[2] = 5;
        triggerIndex = 5;
    }
}
