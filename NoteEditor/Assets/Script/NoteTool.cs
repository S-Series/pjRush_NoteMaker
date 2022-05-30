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
            if (AutoTest.isTest) return;
            if (NoteEdit.isNoteEdit) return;
            if (TestPlay.isPlay || TestPlay.isPlayReady) return;
            if (!InputManager.isNoteInputAble){
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
                        ButtonEffect();
                        break;

                    case 4:
                        ButtonBpm();
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
        InputManager.isNoteInputAble = true;
        input.InputObject = input.PreviewNote[0];
        triggerIndex = 0;
    }
    public void ButtonLong()
    {
        InputManager.isNoteInputAble = true;
        input.InputObject = input.PreviewNote[1];
        triggerIndex = 1;
    }
    public void ButtonBtChip()
    {
        InputManager.isNoteInputAble = true;
        input.InputObject = input.PreviewNote[2];
        triggerIndex = 2;
    }
    public void ButtonBtLong()
    {
        InputManager.isNoteInputAble = true;
        input.InputObject = input.PreviewNote[3];
        triggerIndex = 3;
    }
    public void ButtonEffect()
    {
        InputManager.isNoteInputAble = true;
        input.InputObject = input.PreviewNote[4];
        triggerIndex = 4;
    }
    public void ButtonBpm()
    {
        InputManager.isNoteInputAble = true;
        input.InputObject = input.PreviewNote[5];
        triggerIndex = 5;
    }
}
