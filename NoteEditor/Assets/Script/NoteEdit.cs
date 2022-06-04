using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteEdit : MonoBehaviour
{
    public static NoteEdit noteEdit;
    public static bool isNoteEdit = false;
    public static bool isNoteEditBottom = false;
    public static bool isNoteEditEffect = false;
    public static GameObject Selected = null;
    public static NormalNote SelectedNormal = null;
    public static SpeedNote SelectedSpeed = null;
    public static EffectNote SelectedEffect = null;
    private const int maxPage = 984;

    //* ---------------------------------------------
    [SerializeField] public int guidePosSector;
    [SerializeField] TMP_InputField inputLine;
    [SerializeField] TMP_InputField inputPage;
    [SerializeField] TMP_InputField inputPosY;
    [SerializeField] TMP_InputField inputLegnth;
    [SerializeField] GameObject NoteFieldParent;
    [SerializeField] GameObject MirrorField;
    [SerializeField] List<GameObject> mirror;

    //* ---------------------------------------------
    [SerializeField] TMP_InputField inputEffectForce;
    [SerializeField] TMP_InputField inputEffectDuration;
    public TMP_InputField[] inputSpeedBpm;
    public Toggle isDoubleToggle;

    //* ---------------------------------------------
    [SerializeField] GameObject OriginalSector;
    [SerializeField] GameObject EffectSector;
    [SerializeField] GameObject SpeedSector;
    public void SectorSetOriginal()
    {
        OriginalSector.SetActive(true);
        EffectSector.SetActive(false);
        SpeedSector.SetActive(false);
    }
    public void SectorSetEffect()
    {
        OriginalSector.SetActive(false);
        EffectSector.SetActive(true);
        SpeedSector.SetActive(false);
        EffectManager.isEffectSelected = true;
    }
    public void SectorSetSpeed()
    {
        OriginalSector.SetActive(false);
        EffectSector.SetActive(false);
        SpeedSector.SetActive(true);
    }
    //*private ---------------------------------------------
    private void Awake()
    {
        noteEdit = this;
        isDoubleToggle.interactable = false;
    }
    private void Start()
    {
        isNoteEdit = false;
        guidePosSector = 1;
        ResetNoteInfo();

        SectorSetOriginal();
    }
    private void Update()
    {
        if (isNoteEdit == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Selected == null) return;

                isNoteEdit = false;
                EffectManager.isEffectSelected = false;
                switch (Selected.gameObject.tag)
                {
                    case "chip":
                    case "long":
                        NoteEdit.Selected.GetComponentInChildren<SpriteRenderer>()
                            .color = new Color32(230, 230, 230, 255);
                        break;

                    case "btChip":
                        NoteEdit.Selected.GetComponentInChildren<SpriteRenderer>()
                            .color = new Color32(255, 255, 255, 255);
                        break;

                    case "btLong":
                        NoteEdit.Selected.GetComponentInChildren<SpriteRenderer>()
                            .color = new Color32(255, 255, 255, 150);
                        break;
                }
                Selected = null;
                SelectedNormal = null;
                SelectedSpeed = null;
                SelectedEffect = null;
                isDoubleToggle.interactable = false;
                ResetNoteInfo();
                SectorSetOriginal();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (Selected.tag == "chip")
                {
                    bool privateBool;
                    privateBool = !Selected.transform.GetChild(0).GetChild(0)
                        .GetComponent<SpriteRenderer>().enabled;
                    Selected.transform.GetChild(0).GetChild(0)
                        .GetComponent<SpriteRenderer>().enabled = privateBool;
                    isDoubleToggle.isOn = privateBool;
                }
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteNote();
            }

            if (Selected != null)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow)) { MoveNote(Left: true); }
                if (Input.GetKeyDown(KeyCode.RightArrow)) { MoveNote(Right: true); }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        && (Selected.CompareTag("long") || Selected.CompareTag("btLong")))
                    {
                        int length;
                        length = (int)(Selected.transform.localScale.y / 100);
                        LengthNote(length + 1);
                    }
                    else { MoveNote(Up: true); }
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        && (Selected.CompareTag("long") || Selected.CompareTag("btLong")))
                    {
                        int length;
                        length = (int)(Selected.transform.localScale.y / 100);
                        LengthNote(length - 1);
                    }
                    else { MoveNote(Down: true); }
                }
            }
        }

        if (Selected == null)
        {
            isNoteEdit = false;
            ResetNoteInfo();
        }
    }
    private void LengthNote(int length)
    {
        if (Selected == null) return;
        if (SelectedNormal == null) return;
        if (SelectedNormal.noteObject != Selected) return;
        if (!NormalNote.normalNotes.Contains(SelectedNormal)) return;

        if (length <= 1) length = 2;

        Vector3 editScale;
        editScale = Selected.transform.localScale;

        editScale.y = length * 100.0f;
        SelectedNormal.legnth = length;

        Selected.transform.localScale = editScale;
    }
    private void MoveNote(bool Up = false, bool Down = false, bool Left = false, bool Right = false)
    {
        Vector3 pos;
        pos = Selected.transform.localPosition;

        if (Up)
        {
            int count;
            float inputPos;
            count = Mathf.FloorToInt( pos.y / (1600.0f / GuideGenerate.GuideCount)) + 1;
            inputPos = Mathf.Floor(1600.0f / GuideGenerate.GuideCount * 100) * count / 100;
            if (inputPos >= 1600.0f * 999) inputPos = 1600.0f * 999;
            pos.y = inputPos;
        }
        else if (Down)
        {
            int count;
            float inputPos;
            count = Mathf.CeilToInt( pos.y / (1600.0f / GuideGenerate.GuideCount)) + 1;
            inputPos = Mathf.Ceil(1600.0f / GuideGenerate.GuideCount * 100) * count / 100;
            if (inputPos < 0.0f) inputPos = 0.0f;
            pos.y = inputPos;
        }
        else if (Left)
        {
            if (Selected.CompareTag("Effect") || Selected.CompareTag("Bpm")) return;
            if (Selected.CompareTag("Normal"))
            {
                int getLine;
                getLine = SelectedNormal.line - 1;
                if (getLine < 1) getLine = 1;
                pos.x = -500.0f + getLine * 200.0f;
            }
            else if (Selected.CompareTag("Bottom"))
            {
                pos.x = -200.0f;
            }
        }
        else if (Right)
        {
            if (Selected.CompareTag("Effect") || Selected.CompareTag("Bpm")) return;
            if (Selected.CompareTag("Normal"))
            {
                int getLine;
                getLine = SelectedNormal.line + 1;
                if (getLine > 4) getLine = 4;
                pos.x = -500.0f + getLine * 200.0f;
            }
            else if (Selected.CompareTag("Bottom"))
            {
                pos.x = +200.0f;
            }
        }
        else { return; }
        NotePosition(pos);
        DisplayNoteInfo();
    }
    private void NotePosition(Vector3 inputPos)
    {
        Selected.transform.localPosition = inputPos;

        if (Selected.CompareTag("Effect"))
        {
            if (SelectedEffect == null) SelectedEffect = EffectNote.GetClass(Selected);
            SelectedEffect.pos = inputPos.y;
        }
        else if (Selected.CompareTag("Bpm"))
        {
            if (SelectedSpeed == null) SelectedSpeed = SpeedNote.GetClass(Selected);
            SelectedSpeed.pos = inputPos.y;
        }
        else
        {
            if (SelectedNormal == null) SelectedNormal = NormalNote.GetClass(Selected);
            SelectedNormal.pos = inputPos.y; 
            if (Selected.CompareTag("Normal"))
            {
                if (Mathf.Approximately(-300.0f, inputPos.x)) SelectedNormal.line = 1;
                else if (Mathf.Approximately(-100.0f, inputPos.x)) SelectedNormal.line = 2;
                else if (Mathf.Approximately(+100.0f, inputPos.x)) SelectedNormal.line = 3;
                else if (Mathf.Approximately(+300.0f, inputPos.x)) SelectedNormal.line = 4;
                else return;
            }
            else if (Selected.CompareTag("Bottom"))
            {
                if (Mathf.Approximately(-200.0f, inputPos.x)) SelectedNormal.line = 5;
                else if (Mathf.Approximately(+200.0f, inputPos.x)) SelectedNormal.line = 6;
                else return;
            }
            else { return; }
        }
    }
    private void ResetNoteInfo()
    {
        inputLine.text = "--";
        inputPage.text = "--";
        inputPosY.text = "--";
        inputLegnth.text = "--";
    }
    private void DeleteNote()
    {
        if (SelectedNormal != null) NormalNote.DeleteNote(SelectedNormal);
        if (SelectedSpeed != null) SpeedNote.DeleteNote(SelectedSpeed);
        if (SelectedEffect != null) EffectNote.DeleteNote(SelectedEffect);
        Destroy(Selected);
    }
    //*public ---------------------------------------------
    public void DisplayNoteInfo()
    {
        if (Selected == null) return;
        switch (Selected.tag)
        {
            case "Effect":
                if (SelectedEffect == null)
                {
                    try
                    {
                        SelectedEffect = EffectNote.GetClass(Selected);
                        if (SelectedEffect == null) return;
                    }
                    catch { return; }
                }
                inputPage.text = Mathf.FloorToInt((SelectedEffect.pos / 1600.0f) + 1).ToString();
                inputPosY.text = (SelectedEffect.pos % 1600.0f).ToString();
                inputEffectDuration.text = SelectedEffect.value.ToString();
                break;

            case "Bpm":
                if (SelectedSpeed == null)
                {
                    try
                    {
                        SelectedSpeed = SpeedNote.GetClass(Selected);
                        if (SelectedSpeed == null) return;
                    }
                    catch { return; }
                }
                inputPage.text = Mathf.FloorToInt((SelectedSpeed.pos / 1600.0f) + 1).ToString();
                inputPosY.text = (SelectedSpeed.pos % 1600.0f).ToString();
                inputSpeedBpm[0].text = SelectedSpeed.bpm.ToString();
                inputSpeedBpm[1].text = SelectedSpeed.multiply.ToString();
                break;

            default:
                if (SelectedNormal == null)
                {
                    try
                    {
                        SelectedNormal = NormalNote.GetClass(Selected);
                        if (SelectedNormal == null) return;
                    }
                    catch { return; }
                }
                inputPage.text = Mathf.FloorToInt((SelectedNormal.pos / 1600.0f) + 1).ToString();
                inputPosY.text = (SelectedNormal.pos % 1600.0f).ToString();
                inputLine.text = SelectedNormal.line.ToString();
                inputLegnth.text = SelectedNormal.legnth.ToString();
                break;
        }
    }
    //*Button ---------------------------------------------
    // For All Note
    public void btnPosY()
    {

    }
    public void btnPage()
    {

    }
    // NormalNote
    public void btnLine()
    {

    }
    // SpeedNote
    // EffectNote
}
