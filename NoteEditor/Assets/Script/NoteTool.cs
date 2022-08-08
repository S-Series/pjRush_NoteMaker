using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteTool : MonoBehaviour
{
    InputManager input;
    [SerializeField] Transform frameObject;
    private static GameObject Frame;
    private readonly float[] posX = new float[4]{-5.0f, 112.5f, 230.0f, 347.5f};
    private readonly float[] posY = new float[2]{-50.0f, -170.0f};
    private enum Status {Note, LongNote, Bottom, BottomLong, Speed, Effect, Tik};
    private Status status = Status.Note;
    private void Start()
    {
        input = InputManager.input;
        Frame = frameObject.gameObject;
        disableFrame();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (AutoTest.s_isTest) return;
            if (NoteEdit.isNoteEdit) return;
            if (TestPlay.isPlay || TestPlay.isPlayReady) return;
            if (!InputManager.isNoteInputAble) { ButtonChip(); }
            else
            {
                switch (status){
                    case Status.Note:
                        ButtonLong();
                        break;

                    case Status.LongNote:
                        ButtonBtChip();
                        break;

                    case Status.Bottom:
                        ButtonBtLong();
                        break;

                    case Status.BottomLong:
                        ButtonBpm();
                        break;

                    case Status.Speed:
                        ButtonEffect();
                        break;

                    case Status.Effect:
                        ButtonTik();
                        break;

                    case Status.Tik:
                        ButtonChip();
                        break;
                }
            }
        }
    }
    public static void disableFrame()
    {
        Frame.SetActive(false);
    }
    public void ButtonChip()
    {
        input.PreviewActivate(0, false, false);
        status = Status.Note;
        frameObject.localPosition = new Vector3(posX[0], posY[0], 0.0f);
    }
    public void ButtonLong()
    {
        input.PreviewActivate(1, false, false);
        status = Status.LongNote;
        frameObject.localPosition = new Vector3(posX[0], posY[1], 0.0f);
    }
    public void ButtonBtChip()
    {
        input.PreviewActivate(2, true, false);
        status = Status.Bottom;
        frameObject.localPosition = new Vector3(posX[1], posY[0], 0.0f);
    }
    public void ButtonBtLong()
    {
        input.PreviewActivate(3, true, false);
        status = Status.BottomLong;
        frameObject.localPosition = new Vector3(posX[1], posY[1], 0.0f);
    }
    public void ButtonBpm()
    {
        input.PreviewActivate(4, false, true);
        status = Status.Speed;
        frameObject.localPosition = new Vector3(posX[2], posY[0], 0.0f);
    }
    public void ButtonEffect()
    {
        input.PreviewActivate(5, false, true);
        status = Status.Effect;
        frameObject.localPosition = new Vector3(posX[2], posY[1], 0.0f);
    }
    public void ButtonTik()
    {
        input.PreviewActivate(6, false, false);
        status = Status.Effect;
        frameObject.localPosition = new Vector3(posX[3], posY[0], 0.0f);   
    }
}
