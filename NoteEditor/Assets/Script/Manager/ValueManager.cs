using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ValueManager : MonoBehaviour
{
    private static ValueManager valueManager;
    [SerializeField] TMP_InputField bpmInputField;
    [SerializeField] TMP_InputField delayInputField;
    public static float bpm = 120;
    public static int delay = 0;
    private void Awake() {valueManager = this;}
    public void inputBpm()
    {
        try
        {
            bpm = Convert.ToSingle(Math.Round(Convert.ToDouble(bpmInputField.text), 2));
            if (bpm <= 0) bpm = 120.0f;
            bpmInputField.text = bpm.ToString();
        }
        catch
        {
            bpm = 120.0f;
            bpmInputField.text = "120";
        }
    }
    public void inputDelay()
    {
        try
        {
            delay = Convert.ToInt32(delayInputField.text);
            if (delay < 0) delay = 0;
            delayInputField.text = delay.ToString();
        }
        catch
        {
            delay = 0;
            delayInputField.text = "0";
        }
        if (delay < 0) 
        {
            delay = 0;
            delayInputField.text = "0";
        }
    }
    public static void DisplayValue()
    {
        valueManager.bpmInputField.text = bpm.ToString();
        valueManager.delayInputField.text = delay.ToString();
    }
}
