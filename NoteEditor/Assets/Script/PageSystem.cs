using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PageSystem : MonoBehaviour
{
    public static PageSystem pageSystem;
    public static int nowOnPage = 0;
    const int maxPage = 985;
    readonly float[] posX = new float[4] {925.926f, 1851.852f, 2777.778f, 3703.704f };

    [SerializeField] public GameObject NoteField;
    [SerializeField] TextMeshPro[] LineNum;
    [SerializeField] TMP_InputField PageInput;

    private void Awake()
    {
        pageSystem = this;
    }

    private void Start()
    {
        nowOnPage = 0;
        DisplayPage();
    }

    private void Update(){
        if (AutoTest.s_isTest) { NoteField.transform.localPosition = new Vector3(0, 0, 0); }
        if (!AutoTest.s_isTest && !TestPlay.isPlay && !TestPlay.isPlayReady){
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                nowOnPage++;
                DisplayPage();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                nowOnPage--;
                DisplayPage();
            }
        }
    }

    public void DisplayPage()
    {
        if (nowOnPage < 0) nowOnPage = 0;
        else if (nowOnPage > maxPage) nowOnPage = maxPage;

        float posy;
        posy = nowOnPage * 1600;
        NoteField.transform.localPosition = new Vector3(0.0f, -posy, 0.0f);

        for (int i = 0; i < 15; i++)
        {
            LineNum[i].text = String.Format("{0:d3}", nowOnPage + i);
        }
        PageInput.text = String.Format("{0:d3}", nowOnPage);
    }

    public void InputPage()
    {
        if (AutoTest.s_isTest || TestPlay.isPlay) return;
        try {nowOnPage = Convert.ToInt32(PageInput.text);}
        catch {PageInput.text = nowOnPage.ToString();}
        DisplayPage();
    }

    public void ButtonPage(bool isUp)
    {
        if (isUp) {nowOnPage++;}
        else {nowOnPage--;}
        DisplayPage();
    }
}
