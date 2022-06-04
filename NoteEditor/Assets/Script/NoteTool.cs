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
            else
            {
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
        input.PreviewActivate(0, false, false);
        triggerIndex = 0;
    }
    public void ButtonLong()
    {
        input.PreviewActivate(1, false, false);
        triggerIndex = 1;
    }
    public void ButtonBtChip()
    {
        input.PreviewActivate(2, true, false);
        triggerIndex = 2;
    }
    public void ButtonBtLong()
    {
        input.PreviewActivate(3, true, false);
        triggerIndex = 3;
    }
    public void ButtonEffect()
    {
        input.PreviewActivate(4, false, true);
        triggerIndex = 4;
    }
    public void ButtonBpm()
    {
        input.PreviewActivate(5, false, true);
        triggerIndex = 5;
    }
}
