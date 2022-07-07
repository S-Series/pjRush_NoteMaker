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
    private const string NormalNoteTag = "Normal";
    private const string BottomNoteTag = "Bottom";
    private const string SpeedNoteTag = "Bpm";
    private const string EffectNoteTag = "Effect";
    public enum SelectedType {Null, Normal, Speed, Effect}
    public static SelectedType selectedType = SelectedType.Null;

    //* ---------------------------------------------
    public int guidePosSector;

    //* ---------------------------------------------
    private bool isNoteSwitch = false;

    //* ---------------------------------------------
    [SerializeField] GameObject NoteFieldParent;

    //* ---------------------------------------------
    [SerializeField] TMP_InputField inputNotePos;
    [SerializeField] TMP_InputField inputNotePage;
    [SerializeField] GameObject OriginalSector;     //* --
    [SerializeField] TMP_InputField inputNormalLine;
    [SerializeField] TMP_InputField inputNormalLegnth;
    [SerializeField] Toggle toggleNormalPowered;
    [SerializeField] GameObject SpeedSector;        //* --
    [SerializeField] TMP_InputField inputSpeedBpm;
    [SerializeField] TMP_InputField inputSpeedMultiply;
    [SerializeField] GameObject EffectSector;       //* --
    [SerializeField] GameObject[] isPauseOnOff;
    [SerializeField] TMP_InputField inputEffectValue;
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
    }
    private void Start()
    {
        selectedType = SelectedType.Null;
        isNoteEdit = false;
        guidePosSector = 1;
        ResetNoteInfo();

        SectorSetOriginal();
    }
    private void Update()
    {
        if (SaveLoad.s_isWorking) return;
        if (isNoteEdit == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Selected == null) return;

                selectedType = SelectedType.Null;
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
                toggleNormalPowered.interactable = false;
                ResetNoteInfo();
                SectorSetOriginal();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (Selected.tag == NormalNoteTag)
                {
                    if (SelectedNormal.legnth == 0)
                    {
                        SelectedNormal.isPowered = !SelectedNormal.isPowered;
                    }
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
                    if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                    {
                        if (selectedType == SelectedType.Normal)
                        {
                            if (SelectedNormal.legnth == 0) {MoveNote(Up: true);}
                            else {LengthNote(SelectedNormal.legnth + 1);}
                        }
                        else { MoveNote(Up: true); }
                    }
                    else { MoveNote(Up: true); }
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                    {
                        if (selectedType == SelectedType.Normal)
                        {
                            if (SelectedNormal.legnth == 0) {MoveNote(Down: true);}
                            else {LengthNote(SelectedNormal.legnth + 1);}
                        }
                        else { MoveNote(Down: true); }
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
            count = Mathf.CeilToInt( pos.y / (1600.0f / GuideGenerate.GuideCount)) - 1;
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
            case NormalNoteTag:
            case BottomNoteTag:
                selectedType = SelectedType.Normal;
                SectorSetOriginal();
                inputNotePos.text = 
                    (SelectedNormal.pos % 1600.0f).ToString();
                inputNotePage.text = Mathf.FloorToInt(SelectedNormal.pos / 1600.0f).ToString();
                if (SelectedNormal.line >= 5) {inputNormalLine.text = (SelectedNormal.line - 4).ToString();}
                else {inputNormalLine.text = SelectedNormal.line.ToString();}
                inputNormalLegnth.text = SelectedNormal.legnth.ToString();
                break;

            case SpeedNoteTag:
                selectedType = SelectedType.Speed;
                SectorSetSpeed();
                inputNotePos.text = (SelectedSpeed.pos % 1600.0f).ToString();
                inputNotePage.text = (Mathf.CeilToInt(SelectedSpeed.pos / 1600.0f)).ToString();
                inputSpeedBpm.text = SelectedSpeed.bpm.ToString();
                inputSpeedMultiply.text = SelectedSpeed.multiply.ToString();
                break;

            case EffectNoteTag:
                selectedType = SelectedType.Effect;
                SectorSetEffect();
                inputNotePos.text = (SelectedEffect.pos % 1600.0f).ToString();
                inputNotePage.text = (Mathf.CeilToInt(SelectedEffect.pos / 1600.0f)).ToString();
                inputEffectValue.text = SelectedEffect.value.ToString();
                if (SelectedEffect.isPause)
                {
                    isPauseOnOff[0].SetActive(true);
                    isPauseOnOff[1].SetActive(false);
                    
                }
                else
                {
                    isPauseOnOff[0].SetActive(false);
                    isPauseOnOff[1].SetActive(true);
                }
                break;
                
            default:
                selectedType = SelectedType.Null;
                Debug.LogError("NoteType Out Of Range");
                return;
        }
    }
    //*input Field && Button ---------------------------------------------
    // For All Note
    public void inputValuePos()
    {
        float pos;
        try
        {
            Vector3 notePos;
            pos = Convert.ToSingle(inputNotePos.text);
            notePos = Selected.transform.localPosition;
            switch (selectedType)
            {
                case SelectedType.Null:
                    return;

                case SelectedType.Normal:
                    SelectedNormal.pos = SelectedNormal.pos - (SelectedNormal.pos % 1600) + pos;
                    notePos.y = SelectedNormal.pos;
                    break;

                case SelectedType.Speed:
                    SelectedSpeed.pos = SelectedSpeed.pos - (SelectedSpeed.pos % 1600) + pos;
                    notePos.y = SelectedSpeed.pos;
                    break;

                case SelectedType.Effect:
                    SelectedEffect.pos = SelectedEffect.pos - (SelectedEffect.pos % 1600) + pos;
                    notePos.y = SelectedEffect.pos;
                    break;
            }
            Selected.transform.localPosition = notePos;
        }
        catch 
        {
            switch (selectedType)
            {
                case SelectedType.Null:
                    return;

                case SelectedType.Normal:
                    inputNotePos.text = (SelectedNormal.pos % 1600).ToString();
                    break;

                case SelectedType.Speed:
                    inputNotePos.text = (SelectedSpeed.pos % 1600).ToString();
                    break;

                case SelectedType.Effect:
                    inputNotePos.text = (SelectedEffect.pos % 1600).ToString();
                    break;
            }
        }
        DisplayNoteInfo();
    }
    public void inputValuePage()
    {
        int page;
        Vector3 notePos;
        try {page = Convert.ToInt32(inputNotePage.text);}
        catch
        {
            switch (selectedType)
            {
                case SelectedType.Null:
                    return;

                case SelectedType.Normal:
                    inputNotePos.text = (Mathf.FloorToInt(SelectedNormal.pos % 1600) + 1).ToString();
                    break;

                case SelectedType.Speed:
                    inputNotePos.text = (Mathf.FloorToInt(SelectedSpeed.pos % 1600) + 1).ToString();
                    break;

                case SelectedType.Effect:
                    inputNotePos.text = (Mathf.FloorToInt(SelectedEffect.pos % 1600) + 1).ToString();
                    break;
            }
            return;
        }

        notePos = Selected.transform.localPosition;
        switch (selectedType)
        {
            case SelectedType.Normal:
                notePos.y = SelectedNormal.pos;
                notePos.y = (notePos.y % 1600) + (page * 1600.0f);
                SelectedNormal.pos = notePos.y;
                break;

            case SelectedType.Speed:
                notePos.y = SelectedSpeed.pos;
                notePos.y = (notePos.y % 1600) + (page * 1600.0f);
                SelectedSpeed.pos = notePos.y;
                break;

            case SelectedType.Effect:
                notePos.y = SelectedEffect.pos;
                notePos.y = (notePos.y % 1600) + (page * 1600.0f);
                SelectedEffect.pos = notePos.y;
                break;

            default: return;
        }
        Selected.transform.localPosition = notePos;
        print(notePos.y);
        DisplayNoteInfo();
    }
    /*public void btnPos(bool isUp, bool isPage)
    {
        float addPos;
        if (isUp) addPos = 100.0f;
        else addPos = -100.0f;
        if (isPage) addPos *= 16.0f;
        switch (selectedType)
        {
            case SelectedType.Null:
                return;

            case SelectedType.Normal:
                SelectedNormal.pos += addPos;
                if (SelectedNormal.pos <= 0) SelectedNormal.pos = 0;
                else if (SelectedNormal.pos >= 1600.0f * 999) SelectedNormal.pos = 1600.0f * 999;
                break;

            case SelectedType.Speed:
                SelectedSpeed.pos += addPos;
                if (SelectedSpeed.pos <= 0) SelectedSpeed.pos = 0;
                else if (SelectedSpeed.pos >= 1600.0f * 999) SelectedSpeed.pos = 1600.0f * 999;
                break;

            case SelectedType.Effect:
                SelectedEffect.pos += addPos;
                if (SelectedEffect.pos <= 0) SelectedEffect.pos = 0;
                else if (SelectedEffect.pos >= 1600.0f * 999) SelectedEffect.pos = 1600.0f * 999;
                break;
        }
        DisplayNoteInfo();
    }*/
    // NormalNote
    public void inputValueLine()
    {
        int line;
        try
        {
            Vector3 notePos;
            notePos = Selected.transform.localPosition;
            line = Convert.ToInt32(inputNormalLine.text);
            if (Selected.tag == BottomNoteTag)
            {
                if (line == 1)
                {
                    SelectedNormal.line = 5;
                    notePos.x = -200.0f;
                } 
                else if (line == 2)
                {
                    SelectedNormal.line = 6;
                    notePos.x = 200.0f;
                }
                else
                {
                    inputNormalLine.text = (SelectedNormal.line - 4).ToString();
                    return;
                }
            }
            else
            {
                if (line > 4 || line < 1)
                {
                    inputNormalLine.text = (SelectedNormal.line - 4).ToString();
                    return;
                }
                SelectedNormal.line = line;
                notePos.x = -500.0f + line * 200.0f;
            }
            Selected.transform.localPosition = notePos;
        }
        catch
        {
            switch (selectedType)
            {
                case SelectedType.Null:
                    return;

                case SelectedType.Normal:
                    inputNotePos.text = SelectedNormal.line.ToString();
                    break;

                case SelectedType.Speed:
                    inputNotePos.text = "--";
                    break;

                case SelectedType.Effect:
                    inputNotePos.text = "--";
                    break;
            }
        }
        DisplayNoteInfo();
    }
    public void inputValueLegnth()
    {
        int legnth;
        try
        {
            legnth = Convert.ToInt32(inputNormalLegnth.text);
        }
        catch
        {
            inputNormalLegnth.text = SelectedNormal.legnth.ToString();
            return;
        }
        if (legnth <= 0)
        {
            inputNormalLegnth.text = SelectedNormal.legnth.ToString();
            return;
        }

        int line = SelectedNormal.line;
        float[] range = new float[2]{SelectedNormal.pos, SelectedNormal.pos + 100.0f * legnth};
        foreach (NormalNote noraml in NormalNote.normalNotes)
        {
            if (noraml.line == line)
            {
                if (noraml.pos >= range[0] && noraml.pos <= range[1])
                {
                    inputNormalLegnth.text = "overlap";
                    return;
                }
            }
        }
        Vector3 noteScale;
        SelectedNormal.legnth = legnth;
        noteScale = Selected.transform.localScale;
        noteScale.y = 100.0f * legnth;
        DisplayNoteInfo();
    }
    // SpeedNote
    public void inputValueBpm()
    {
        if (Selected == null) return;
        if (selectedType != SelectedType.Speed) return;
        float inputBpm;
        try
        {
            inputBpm = Convert.ToSingle(inputSpeedBpm.text);
            if (inputBpm <= 0) return;
        }
        catch
        {
            inputSpeedBpm.text = SelectedSpeed.bpm.ToString();
            return;
        }
        SelectedSpeed.bpm = inputBpm;
        //ToDo : 텍스트 표시하기
        DisplayNoteInfo();
    }
    public void inputValueMultiply()
    {
        if (Selected == null) return;
        if (selectedType != SelectedType.Speed) return;
        float inputMultiply;
        try
        {
            inputMultiply = Convert.ToSingle(inputSpeedMultiply.text);
            if (inputMultiply <= 0) return;
        }
        catch
        {
            inputSpeedMultiply.text = SelectedSpeed.multiply.ToString();
            return;
        }
        SelectedSpeed.multiply = inputMultiply;
        //ToDo : 텍스트 표시하기
        DisplayNoteInfo();
    }
    // EffectNote
    public void btnTypeChange()
    {
        if (selectedType != SelectedType.Effect) return;
        if (SelectedEffect.isPause)
        {
            // Change to Teleport
            isPauseOnOff[0].SetActive(false);
            isPauseOnOff[1].SetActive(true);
            SelectedEffect.isPause = false;
            Selected.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            // Change to Pause
            isNoteSwitch = true;
            isPauseOnOff[0].SetActive(true);
            isPauseOnOff[1].SetActive(false);
            SelectedEffect.isPause = true;
            Selected.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
    }
    public void inputValueValue()
    {
        if (Selected == null)
        {
            Debug.Log("run");
            return;    
        }
        if (selectedType != SelectedType.Effect) return;
        

        float inputValue;
        try {inputValue = Convert.ToSingle(inputEffectValue.text);}
        catch
        {
            inputEffectValue.text = SelectedEffect.value.ToString();
            return;
        }
        /*if (!SelectedEffect.isPause)
        {
            float[] range = new float[2] { SelectedEffect.pos, SelectedEffect.pos + inputValue };
            foreach (NormalNote normal in NormalNote.normalNotes)
            {
                if (!normal.isPowered)
                {
                    if (normal.pos >= range[0] && normal.pos < range[1])
                    {
                        inputEffectValue.text = SelectedEffect.value.ToString();
                        return;
                    }
                }
            }
            foreach (SpeedNote speed in SpeedNote.speedNotes)
            {
                if (speed.pos >= range[0] && speed.pos < range[1])
                {
                    inputEffectValue.text = SelectedEffect.value.ToString();
                    return;
                }
            }
            foreach (EffectNote effect in EffectNote.effectNotes)
            {
                if (effect.pos >= range[0] && effect.pos < range[1])
                {
                    inputEffectValue.text = SelectedEffect.value.ToString();
                    return;
                }
            }
        }*/
        SelectedEffect.value = inputValue;
        
        Selected.GetComponentInChildren<TextMeshPro>().text = inputValue.ToString();
        Selected.transform.GetChild(0).GetChild(0).localScale = new Vector3(3.25f, inputValue, 1.0f);
        
    }
}
