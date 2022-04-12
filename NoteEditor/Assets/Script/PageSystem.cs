using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PageSystem : MonoBehaviour
{
    public static PageSystem pageSystem;

    public int firstPage;

    const int maxPage = 985;

    readonly float[] posX = new float[4] {925.926f, 1851.852f, 2777.778f, 3703.704f };

    [SerializeField]
    public GameObject NoteField;

    [SerializeField]
    GameObject MirrorField;

    [SerializeField]
    TextMeshPro[] LineNum;

    [SerializeField]
    TMP_InputField PageInput;

    private void Awake()
    {
        pageSystem = this;
    }

    private void Start()
    {
        PageSet(1);
    }

    private void Update(){
        if (!AutoTest.autoTest.isTest && !TestPlay.isPlay && !TestPlay.isPlayReady){
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                firstPage++;
                if (firstPage < 1) firstPage = 1;
                else if (firstPage > maxPage) firstPage = maxPage;
                PageSet(firstPage);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                firstPage--;
                if (firstPage < 1) firstPage = 1;
                else if (firstPage > maxPage) firstPage = maxPage;
                PageSet(firstPage);
            }
        }
    }

    public void PageSet(int first)
    {
        if (first > maxPage)
            first = maxPage;
        firstPage = first;

        float posy;
        posy = (firstPage - 1) * 1600;
        NoteField.transform.localPosition = new Vector3(0.0f, -posy, 0.0f);
        NoteField.transform.localPosition = new Vector3(0.0f, -posy, 0.0f);

        for (int i = 0; i < MirrorField.transform.childCount; i++)
        {
            Destroy(MirrorField.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject copy;
            copy = Instantiate(NoteField, MirrorField.transform);
            copy.transform.localPosition = new Vector3(posX[i], -posy - 4800 * (i + 1), 0.0f);
        }

        for (int i = 0; i < 15; i++)
        {
            if ((firstPage + i) < 10)
            {
                LineNum[i].text = "00" + (firstPage + i).ToString();
            }
            else if ((firstPage + i) < 100)
            {
                LineNum[i].text = "0" + (firstPage + i).ToString();
            }
            else
            {
                LineNum[i].text = (firstPage + i).ToString();
            }
        }
    }

    public void ButtonPage()
    {
        //var auto = AutoTest.autoTest;       // 1번
        AutoTest auto = AutoTest.autoTest;  // 2번
        if (auto.isTest || TestPlay.isPlay) return;

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
