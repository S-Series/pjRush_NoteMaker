﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NoteEdit : MonoBehaviour
{
    public static NoteEdit noteEdit;

    public GameObject Selected;

    public bool isNoteEdit;

    private const int maxPage = 984;

    private readonly float[] rdPosX = new float[5] {0.0f, 925.926f, 1851.852f, 2777.778f, 3703.704f };

    [SerializeField]
    public float guidePosSector;

    [SerializeField]
    TMP_InputField inputLine;

    [SerializeField]
    TMP_InputField inputPage;

    [SerializeField]
    TMP_InputField inputPosY;

    [SerializeField]
    TMP_InputField inputLegnth;

    [SerializeField]
    GameObject NoteFieldParent;

    [SerializeField]
    GameObject MirrorField;

    [SerializeField]
    List<GameObject> mirror;

    //---------------------------------------------

    [SerializeField]
    TMP_InputField inputEffectForce;

    [SerializeField]
    TMP_InputField inputEffectDuration;

    [SerializeField]
    TMP_InputField inputSpeedBpm;

    //---------------------------------------------

    [SerializeField]
    GameObject OriginalSector;

    [SerializeField]
    GameObject EffectSector;

    [SerializeField]
    GameObject SpeedSector;

    private void Awake()
    {
        noteEdit = this;
    }

    private void Start()
    {
        isNoteEdit = false;
        ResetNoteInfo();

        SectorSetOriginal();
    }

    private void Update()
    {
        if (isNoteEdit == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isNoteEdit = false;
                Selected = null;
                ResetNoteInfo();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                StartCoroutine(DeleteNote());
            }

            if (Selected != null)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    Vector3 selectPos;
                    selectPos = Selected.transform.localPosition;

                    switch (Selected.transform.localPosition.x)
                    {
                        case -100:
                            selectPos.x = -300;
                            Selected.transform.localPosition = selectPos;
                            DisplayNoteInfo();
                            MirrorPage();
                            break;

                        case +100:
                            selectPos.x = -100;
                            Selected.transform.localPosition = selectPos;
                            DisplayNoteInfo();
                            MirrorPage(); break;

                        case +300:
                            selectPos.x = +100;
                            Selected.transform.localPosition = selectPos;
                            DisplayNoteInfo();
                            MirrorPage(); break;

                        default:
                            return;
                    }
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    Vector3 selectPos;
                    selectPos = Selected.transform.localPosition;

                    switch (Selected.transform.localPosition.x)
                    {
                        case -300:
                            selectPos.x = -100;
                            Selected.transform.localPosition = selectPos;
                            DisplayNoteInfo();
                            MirrorPage();
                            break;

                        case -100:
                            selectPos.x = +100;
                            Selected.transform.localPosition = selectPos;
                            DisplayNoteInfo();
                            MirrorPage();
                            break;

                        case +100:
                            selectPos.x = +300;
                            Selected.transform.localPosition = selectPos;
                            DisplayNoteInfo();
                            MirrorPage();
                            break;

                        default:
                            return;
                    }
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    bool isUp;
                    isUp = true;

                    Vector3 notePos;
                    notePos = Selected.transform.localPosition;

                    if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        && (Selected.tag == "long" || Selected.tag == "btLong"))
                    {
                        int length;
                        length = (int)(Selected.transform.localScale.y / 100);
                        StartCoroutine(UpdateLengthNote(length + 1));

                        DisplayNoteInfo();
                        MirrorPage();
                    }
                    else
                    {
                        ArrowKeyNoteMove(notePos, isUp);
                    }
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    bool isUp;
                    isUp = false;

                    Vector3 notePos;
                    notePos = Selected.transform.localPosition;

                    if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        && (Selected.tag == "long" || Selected.tag == "btLong"))
                    {
                        int length;
                        length = (int)(Selected.transform.localScale.y / 100);
                        StartCoroutine(UpdateLengthNote(length - 1));

                        DisplayNoteInfo();
                        MirrorPage();
                    }
                    else
                    {
                        ArrowKeyNoteMove(notePos, isUp);
                    }
                }
            }
        }

        if (Selected == null)
        {
            isNoteEdit = false;
            ResetNoteInfo();
        }
    }

    private IEnumerator UpdateLengthNote(int length)
    {
        Vector3 pos;
        pos = Selected.transform.localPosition;

        GameObject NoteField;
        NoteField = PageSystem.pageSystem.NoteField;

        GameObject editNote;
        editNote = null;
        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            if (NoteField.transform.GetChild(i).localPosition == pos)
            {
                editNote = NoteField.transform.GetChild(i).gameObject;
                break;
            }
        }
        if (editNote != null)
        {
            Vector3 SelectedScale;
            if (Selected.tag == "long" || Selected.tag == "btLong")
            {
                try
                {
                    SelectedScale = Selected.transform.localScale;
                    if (length < 2)
                    {
                        DisplayNoteInfo();
                        yield break;
                    }
                    SelectedScale.y = length * 100;

                    Selected.transform.localScale = SelectedScale;
                    MirrorPage();
                }
                catch { yield break; }
            }
        }
        else { Debug.LogError("조정 대상을 찾지 못함"); yield break; }

        yield return new WaitForSeconds(.1f);

        PageSystem.pageSystem.PageSet(PageSystem.pageSystem.firstPage);
    }

    private void ArrowKeyNoteMove(Vector3 pos, bool isUp)
    {
        if (isUp == true)
        {
            int guideNum;

            if ((pos.y - pos.y % 1600) / 1600 == 0)
            {
                guideNum = 0;
            }
            else
            {
                guideNum = (int)((pos.y - pos.y % 1600) / 1600);
            }

            if (guideNum == 0)
            {
                pos.y = pos.y - pos.y % 1600 + guidePosSector;
            }
            else
            {
                pos.y = pos.y - pos.y % 1600 + guidePosSector * (guideNum + 1);
            }

            Selected.transform.localPosition = pos;
            DisplayNoteInfo();
            MirrorPage();
        }
        else
        {
            int guideNum;

            if ((pos.y - pos.y % 1600) / 1600 == 0)
            {
                guideNum = 0;
            }
            else
            {
                guideNum = (int)((pos.y - pos.y % 1600) / 1600);
            }

            if (guideNum == 0)
            {
                pos.y = pos.y - pos.y % 1600 - guidePosSector;
            }
            else
            {
                pos.y = pos.y - pos.y % 1600 + guidePosSector * (guideNum - 1);
            }

            Selected.transform.localPosition = pos;
            DisplayNoteInfo();
            MirrorPage();
        }
    }

    private void ResetNoteInfo()
    {
        inputLine.text = "--";
        inputPage.text = "--";
        inputPosY.text = "--";
        inputLegnth.text = "--";
    }

    public void DisplayNoteInfo()
    {
        switch (Selected.tag)
        {
            case "Effect":
                break;

            case "Bpm":
                break;

            default:
                switch (Selected.transform.localPosition.x)
                {
                    case -300:
                        inputLine.text = "1";
                        break;

                    case -100:
                        inputLine.text = "2";
                        break;

                    case +100:
                        inputLine.text = "3";
                        break;

                    case +300:
                        inputLine.text = "4";
                        break;

                    case 0:
                        inputLine.text = "5";
                        break;

                    default:
                        return;
                }

                try
                {
                    float posY;
                    posY = Selected.transform.localPosition.y;

                    inputPage.text = ((posY - posY % 1600) / 1600).ToString();
                    inputPosY.text = (posY % 1600).ToString();
                }
                catch { return; }

                try
                {
                    if (Selected.tag == "long" || Selected.tag == "btLong")
                    {
                        inputLegnth.text = ((int)(Selected.transform.localScale.y / 100)).ToString();
                    }
                    else { inputLegnth.text = "--"; }
                }
                catch { return; }
                break;
        }
    }

    IEnumerator DeleteNote()
    {
        Vector3 pos;
        pos = Selected.transform.localPosition;

        GameObject NoteField;
        NoteField = PageSystem.pageSystem.NoteField;

        GameObject deleteNote;
        deleteNote = null;
        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            if (NoteField.transform.GetChild(i).localPosition == pos)
            {
                deleteNote = NoteField.transform.GetChild(i).gameObject;
                break;
            }
        }
        if (deleteNote != null)
        {
            Destroy(deleteNote);
        }
        else { Debug.LogError("삭제 대상을 찾지 못함"); yield break; }

        yield return new WaitForSeconds(.1f);

        PageSystem.pageSystem.PageSet(PageSystem.pageSystem.firstPage);
    }

    IEnumerator LengthNote()
    {
        Vector3 pos;
        pos = Selected.transform.localPosition;

        GameObject NoteField;
        NoteField = PageSystem.pageSystem.NoteField;

        GameObject editNote;
        editNote = null;
        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            if (NoteField.transform.GetChild(i).localPosition == pos)
            {
                editNote = NoteField.transform.GetChild(i).gameObject;
                break;
            }
        }
        if (editNote != null)
        {
            int legnthInput;
            Vector3 SelectedScale;
            if (Selected.tag == "long" || Selected.tag == "btLong")
            {
                try
                {
                    legnthInput = int.Parse(inputLegnth.text);
                    SelectedScale = Selected.transform.localScale;
                    if (legnthInput < 2)
                    {
                        DisplayNoteInfo();
                        yield break;
                    }
                    SelectedScale.y = legnthInput * 100;

                    Selected.transform.localScale = SelectedScale;
                    MirrorPage();
                }
                catch { yield break; }
            }
        }
        else { Debug.LogError("조정 대상을 찾지 못함"); yield break; }

        yield return new WaitForSeconds(.1f);

        PageSystem.pageSystem.PageSet(PageSystem.pageSystem.firstPage);
    }

    // ------------------------------------------------------------

    public void ButtonLine()
    {
        int lineInput;
        Vector3 SelectedPos;
        try
        {
            lineInput = int.Parse(inputLine.text);
            SelectedPos = Selected.transform.localPosition;
            if (lineInput < 1 || lineInput > 5)
            {
                DisplayNoteInfo();
                return;
            }
            switch (lineInput)
            {
                case 1:
                    SelectedPos.x = -300;
                    break;

                case 2:
                    SelectedPos.x = -100;
                    break;

                case 3:
                    SelectedPos.x = +100;
                    break;

                case 4:
                    SelectedPos.x = +300;
                    break;

                case 5:
                    SelectedPos.x = 0;
                    break;
            }
            Selected.transform.localPosition = SelectedPos;
            DisplayNoteInfo();
            MirrorPage();
        }
        catch { return; }
    }

    public void ButtonPage()
    {
        int pageInput;
        Vector3 SelectedPos;
        try
        {
            pageInput = int.Parse(inputPage.text);
            SelectedPos = Selected.transform.localPosition;
            if (pageInput < 1 || pageInput > maxPage)
            {
                DisplayNoteInfo();
                return;
            }
            SelectedPos.y = SelectedPos.y % 1600 + pageInput * 1600;

            Selected.transform.localPosition = SelectedPos;
            MirrorPage();
        }
        catch { return; }
    }

    public void ButtonPos()
    {
        int posInput;
        Vector3 SelectedPos;
        try
        {
            posInput = int.Parse(inputPage.text);
            SelectedPos = Selected.transform.localPosition;
            if (posInput < 0 || posInput >= 1600)
            {
                DisplayNoteInfo();
                return;
            }
            SelectedPos.y -= SelectedPos.y % 1600 + posInput;

            Selected.transform.localPosition = SelectedPos;
        }
        catch { return; }
    }

    public void ButtonLegnth()
    {
        StartCoroutine(LengthNote());
    }

    // Effect Force =
    //      EffectNote.child.transform.localposition.x
    // Effect Duration = 
    //      EffectNote.child.transform.localposition.y
    public void ButtonEffect()
    {
        Vector3 forcePos;
        try
        {
            forcePos = Selected.transform.localPosition;
            forcePos.x = Convert.ToSingle(inputEffectForce.text);
            Selected.transform.GetChild(0).localPosition = forcePos;
            MirrorPage();
        }
        catch { return; }
    }

    public void ButtonDuration()
    {
        Vector3 durationPos;
        try
        {
            durationPos = Selected.transform.localPosition;
            durationPos.y = Convert.ToSingle(inputEffectForce.text);
            Selected.transform.GetChild(0).localPosition = durationPos;
            MirrorPage();
        }
        catch { return; }
    }

    // Speed Bpm = 
    //      SpeedNote.child.child.transform.localposition.y
    public void ButtonSpeed()
    {
        float speedBpm;
        try
        {
            speedBpm = Convert.ToSingle(inputSpeedBpm.text);
            if (speedBpm < 0)
            {
                speedBpm = 120;
                inputSpeedBpm.text = "120";
            }
            Selected.transform.GetChild(0).GetComponent<TextMeshPro>().text = speedBpm.ToString();
            Selected.transform.GetChild(0).GetChild(0).localPosition = new Vector3(0, speedBpm, 0);
            MirrorPage();
        }
        catch { return; }
    }

    private void MirrorPage()
    {
        float posX;
        posX = Selected.transform.parent.localPosition.x;

        mirror = new List<GameObject>(5);

        mirror.Add(PageSystem.pageSystem.NoteField);

        for (int i = 0; i < MirrorField.transform.childCount; i++)
        {
            mirror.Add(MirrorField.transform.GetChild(i).gameObject);
        }

        mirror.Sort(delegate (GameObject A, GameObject B)
        {
            if (A.transform.localPosition.x > B.transform.localPosition.x) return 1;
            else if (A.transform.localPosition.x < B.transform.localPosition.x) return -1;
            return 0;
        });

        if (posX != 0)
        {
            Vector3 pos;
            pos = PageSystem.pageSystem.NoteField.transform.localPosition;

            Destroy(PageSystem.pageSystem.NoteField);
            PageSystem.pageSystem.NoteField =
                Instantiate(Selected.transform.parent.gameObject
                , NoteFieldParent.transform);
            mirror[0] = PageSystem.pageSystem.NoteField;
            mirror[0].transform.localPosition = pos;
            mirror[0].name = "NoteField";
            InputManager.input.inputNoteFieldSet(mirror[0]);
        }

        for (int i = 1; i < 5; i++)
        {
            if (posX != rdPosX[i])
            {
                Vector3 pos;
                pos = mirror[i].transform.localPosition;
                Destroy(mirror[i]);
                mirror[i] = Instantiate(Selected.transform.parent.gameObject
                , MirrorField.transform);
                mirror[i].transform.localPosition = pos;
                mirror[i].name = "MirrorField" + i.ToString();
            }
        }
    }

    // this fuction is triggered by button
    public void SectorSetOriginal()
    {
        OriginalSector.SetActive(true);
        EffectSector.SetActive(false);
        SpeedSector.SetActive(false);
    }

    // this fuction is triggered by button
    public void SectorSetEffect()
    {
        OriginalSector.SetActive(false);
        EffectSector.SetActive(true);
        SpeedSector.SetActive(false);
    }

    // this fuction is triggered by button
    public void SectorSetSpeed()
    {
        OriginalSector.SetActive(false);
        EffectSector.SetActive(false);
        SpeedSector.SetActive(true);
    }
}
