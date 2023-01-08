using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenResolution : MonoBehaviour
{
    public static bool isSetting;
    private readonly int[] width = 
        {960, 1280, 1440, 1600, 1920, 2560};
    private readonly int[] height = 
        {540, 720, 810, 900, 1080, 1440};
    private readonly int[] fps = 
        {30, 60, 75, 120, 144, 240};
    [SerializeField] GameObject SubCameraObject;
    [SerializeField] GameObject ScreenSettingObject;
    [SerializeField] TMP_Dropdown[] dropdown;
    [SerializeField] Toggle toggle;
    void Start()
    {
        isSetting = true;
        dropdown[0].value = 3;
        dropdown[1].value = 3;
        toggle.isOn = false;
        SubmitButton();
    }
    public void ToggleSetting()
    {
        isSetting = !isSetting;
        SubCameraObject.SetActive(!isSetting);
        ScreenSettingObject.SetActive(isSetting);
    }
    public void SubmitButton()
    {
        if (!isSetting) { return; }
        Screen.SetResolution(width[dropdown[0].value], height[dropdown[0].value], toggle.isOn);
        Application.targetFrameRate = fps[dropdown[1].value];
        ShowFps.UpdateResolution();
        ToggleSetting();
    }
}
