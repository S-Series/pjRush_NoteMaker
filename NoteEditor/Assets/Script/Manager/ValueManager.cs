using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ValueManager : MonoBehaviour
{
    [SerializeField] TMP_InputField bpmInputField;
    [SerializeField] TMP_InputField delayInputField;
    public static float bpm = 120;
    public static int delay = 0;
    public void btnBpm()
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
    public void btnDelay()
    {
        try
        {
            delay = Convert.ToInt32(delayInputField.text);
            delayInputField.text = bpm.ToString();
        }
        catch
        {
            delay = 0;
            delayInputField.text = "0";
        }
    }
}
