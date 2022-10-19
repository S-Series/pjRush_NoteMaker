using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteTool : MonoBehaviour
{
    InputManager input;
    [SerializeField] Transform frameObject;
    [SerializeField] Toggle toggle;
    [SerializeField] TMP_InputField inputField;
    private static GameObject Frame;
    private static int inputLegnth;
    private readonly float[] posX = new float[4]{-5.0f, 112.5f, 230.0f, 347.5f};
    private readonly float[] posY = new float[2]{-50.0f, -170.0f};
    private void Start()
    {
        input = InputManager.input;
        Frame = frameObject.gameObject;
        disableFrame();
    }
    private void Update()
    {
            if (AutoTest.s_isTest) return;
            if (NoteEdit.isNoteEdit) return;
            if (PlayMode.s_isPlay) return;

            if (Input.GetKeyDown(KeyCode.Alpha1)) { ButtonNormal(); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { ButtonBottom(); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { ButtonPowered(); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { ButtonSpeed(); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { ButtonEffect(); }
    }
    public static void disableFrame()
    {
        Frame.SetActive(false);
    }

    //** --
    public void ButtonNormal()
    {
        input.PreviewActivate(_prefabIndex:0);
        frameObject.localPosition = new Vector3(posX[0], posY[0], 0.0f);
    }
    public void ButtonBottom()
    {
        input.PreviewActivate(_prefabIndex:1 , _isBottom:true);
        frameObject.localPosition = new Vector3(posX[0], posY[0], 0.0f);
    }
    public void ButtonPowered()
    {
        input.PreviewActivate(_prefabIndex:2);
        frameObject.localPosition = new Vector3(posX[0], posY[0], 0.0f);
    }
    //** ----
    public void ButtonSpeed()
    {
        input.PreviewActivate(_prefabIndex:3, _isEffect:true);
        frameObject.localPosition = new Vector3(posX[2], posY[0], 0.0f);
    }
    public void ButtonEffect()
    {
        input.PreviewActivate(_prefabIndex:4 , _isEffect:true);
        frameObject.localPosition = new Vector3(posX[2], posY[1], 0.0f);
    }
}
