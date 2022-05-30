using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PageSystem : MonoBehaviour
{
    public static PageSystem pageSystem;

    public static int nowOnPage = 1;

    const int maxPage = 985;

    readonly float[] posX = new float[4] {925.926f, 1851.852f, 2777.778f, 3703.704f };

    [SerializeField] public GameObject NoteField;
    [SerializeField] TextMeshPro[] LineNum;
    [SerializeField]TMP_InputField PageInput;

    private void Awake()
    {
        pageSystem = this;
    }

    private void Start()
    {
        PageSet(1);
    }

    private void Update(){
        if (!AutoTest.isTest && !TestPlay.isPlay && !TestPlay.isPlayReady){
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                nowOnPage++;
                if (nowOnPage < 1) nowOnPage = 1;
                else if (nowOnPage > maxPage) nowOnPage = maxPage;
                PageSet(nowOnPage);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                nowOnPage--;
                if (nowOnPage < 1) nowOnPage = 1;
                else if (nowOnPage > maxPage) nowOnPage = maxPage;
                PageSet(nowOnPage);
            }
        }
    }

    public void PageSet(int page)
    {
        if (page > maxPage) page = maxPage;
        nowOnPage = page;

        float posy;
        posy = (nowOnPage - 1) * 1600;
        NoteField.transform.localPosition = new Vector3(0.0f, -posy, 0.0f);

        for (int i = 0; i < 15; i++)
        {
            LineNum[i].text = String.Format("{0:d3}", nowOnPage - 1 + i);
        }
    }

    public void ButtonPage()
    {
        if (AutoTest.isTest || TestPlay.isPlay) return;

        int input;
        try
        {
            input = Convert.ToInt32(PageInput.text);
        }
        catch { return; }
        PageInput.text = "";
        PageSet(input - 1);
    }
}
