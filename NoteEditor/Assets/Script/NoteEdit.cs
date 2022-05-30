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

    //* ---------------------------------------------
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
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    Vector3 selectPos;
                    selectPos = Selected.transform.localPosition;

                    if (Selected.tag == "chip" || Selected.tag == "long")
                    {
                        switch (Selected.transform.localPosition.x)
                        {
                            case -100:
                                selectPos.x = -300;
                                Selected.transform.localPosition = selectPos;
                                DisplayNoteInfo();
                                break;

                            case +100:
                                selectPos.x = -100;
                                Selected.transform.localPosition = selectPos;
                                DisplayNoteInfo();
                                break;

                            case +300:
                                selectPos.x = +100;
                                Selected.transform.localPosition = selectPos;
                                DisplayNoteInfo();
                                break;

                            default:
                                return;
                        }
                    }
                    else if (Selected.tag == "btChip" || Selected.tag == "btLong")
                    {
                        Vector3 selectScale;
                        selectScale = Selected.transform.localScale;
                        selectScale.x = 0.75f;

                        selectPos.x = -100;
                        Selected.transform.localPosition = selectPos;
                        Selected.transform.localScale = selectScale;

                        DisplayNoteInfo();
                    }
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    Vector3 selectPos;
                    selectPos = Selected.transform.localPosition;

                    if (Selected.tag == "chip" || Selected.tag == "long")
                    {
                        switch (Selected.transform.localPosition.x)
                        {
                            case -300:
                                selectPos.x = -100;
                                Selected.transform.localPosition = selectPos;
                                DisplayNoteInfo();
                                break;

                            case -100:
                                selectPos.x = +100;
                                Selected.transform.localPosition = selectPos;
                                DisplayNoteInfo();
                                break;

                            case +100:
                                selectPos.x = +300;
                                Selected.transform.localPosition = selectPos;
                                DisplayNoteInfo();
                                break;

                            default:
                                return;
                        }
                    }
                    else if (Selected.CompareTag("btChip") || Selected.CompareTag("btLong"))
                    {
                        Vector3 selectScale;
                        selectScale = Selected.transform.localScale;
                        selectScale.x = 0.75f;

                        selectPos.x = 100;
                        Selected.transform.localPosition = selectPos;
                        Selected.transform.localScale = selectScale;

                        DisplayNoteInfo();
                    }
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    bool isUp;
                    isUp = true;

                    Vector3 notePos;
                    notePos = Selected.transform.localPosition;

                    if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        && (Selected.CompareTag("long") || Selected.CompareTag("btLong")))
                    {
                        int length;
                        length = (int)(Selected.transform.localScale.y / 100);
                        LengthNote(length + 1);

                        DisplayNoteInfo();
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
                        && (Selected.CompareTag("long") || Selected.CompareTag("btLong")))
                    {
                        int length;
                        length = (int)(Selected.transform.localScale.y / 100);
                        LengthNote(length - 1);

                        DisplayNoteInfo();
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
    private void ArrowKeyNoteMove(Vector3 pos, bool isUp)
    {
        if (isUp == true)
        {
            if (guidePosSector == 1)
            {
                pos.y = pos.y - pos.y % 1600 + 1600;

                if (pos.y < 0) pos.y = 0;
                Selected.transform.localPosition = pos;
                DisplayNoteInfo();
            }
            else
            {
                int guideNum;

                if (pos.y % 1600 == 0)
                {
                    guideNum = 0;
                }
                else
                {
                    guideNum = (int)((pos.y % 1600) / (1600 / guidePosSector));
                }

                if (guideNum == 0)
                {
                    pos.y = pos.y - pos.y % 1600 + (1600 / guidePosSector);
                }
                else if (guideNum + 1 == guidePosSector)
                {
                    pos.y = pos.y - pos.y % 1600 + 1600;
                }
                else
                {
                    pos.y = pos.y - pos.y % 1600 + (1600 / guidePosSector) * (guideNum + 1);
                }

                if (pos.y < 0) pos.y = 0;
                Selected.transform.localPosition = pos;
                DisplayNoteInfo();
            }
        }
        else
        {
            if (guidePosSector == 1)
            {
                pos.y = pos.y - pos.y % 1600 - 1600;

                if (pos.y < 0) pos.y = 0;
                Selected.transform.localPosition = pos;
                DisplayNoteInfo();
            }
            else
            {
                int guideNum;

                if (pos.y / 1600 == 0)
                {
                    guideNum = 0;
                }
                else
                {
                    guideNum = (int)((pos.y % 1600) / (1600 / guidePosSector));
                }

                if (guideNum == 0)
                {
                    pos.y = pos.y - pos.y % 1600 - (1600 / guidePosSector);
                }
                else if (guideNum - 1 == 0)
                {
                    pos.y -= pos.y % 1600;
                }
                else
                {
                    pos.y = pos.y - pos.y % 1600 + (1600 / guidePosSector) * (guideNum - 1);
                }

                if (pos.y < 0) pos.y = 0;
                Selected.transform.localPosition = pos;
                DisplayNoteInfo();
            }
        }
        //* NormalNote
        if (SelectedNormal != null
                && SelectedNormal.noteObject == Selected)
        { SelectedNormal.pos = pos.y; }
        //* SpeedNote
        if (SelectedSpeed != null
                && SelectedSpeed.noteObject == Selected)
        { SelectedSpeed.pos = pos.y; }
        //* EffectNote
        if (SelectedEffect != null
                && SelectedEffect.noteObject == Selected)
        { SelectedEffect.pos = pos.y; }
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
    private void DeleteNote()
    {
        if (SelectedNormal != null) NormalNote.DeleteNote(SelectedNormal);
        if (SelectedSpeed != null) SpeedNote.DeleteNote(SelectedSpeed);
        if (SelectedEffect != null) EffectNote.DeleteNote(SelectedEffect);
        Destroy(Selected);
    }
    IEnumerator _SpeedNote()
    {
        float speed;
        try
        {
            speed = Convert.ToSingle(inputSpeedBpm[1].text);
            if (speed == 0)
            {
                speed = ValueManager.bpm;
                inputSpeedBpm[1].text = ValueManager.bpm.ToString();
            }
            Vector3 posChild;
            posChild = Selected.transform.GetChild(0).GetChild(0).localPosition;
            posChild.x = speed;
            Selected.transform.GetChild(0).GetChild(0).localPosition = posChild;
            Selected.transform.GetComponentInChildren<TextMeshPro>().text =
                posChild.y.ToString() + "\nx " + posChild.x.ToString();
        }
        catch { yield break; }

        Vector3 pos;
        pos = Selected.transform.localPosition;

        float posX;
        posX = Selected.transform.parent.localPosition.x;

        GameObject NoteField;
        NoteField = PageSystem.pageSystem.NoteField;

        GameObject Parent;
        Parent = Selected.transform.parent.gameObject;
        /*
        if (Selected != null)
        {
            Destroy(Selected);
        }
        else { Debug.LogError("삭제 대상을 찾지 못함"); yield break; }*/

        yield return new WaitForSeconds(.1f);

        mirror = new List<GameObject>(5);

        mirror.Add(PageSystem.pageSystem.NoteField);

        for (int i = 0; i < 4; i++)
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
            Vector3 pagePos;
            pagePos = PageSystem.pageSystem.NoteField.transform.localPosition;

            Destroy(PageSystem.pageSystem.NoteField);
            PageSystem.pageSystem.NoteField =
                Instantiate(Parent, NoteFieldParent.transform);
            mirror[0] = PageSystem.pageSystem.NoteField;
            mirror[0].transform.localPosition = pagePos;
            mirror[0].name = "NoteField";
            InputManager.input.inputNoteFieldSet(mirror[0]);
        }
    }
    IEnumerator SpeedNoteBpm()
    {
        float speedBpm;
        try
        {
            speedBpm = Convert.ToSingle(inputSpeedBpm[0].text);
            if (speedBpm < 0)
            {
                speedBpm = ValueManager.bpm;
                inputSpeedBpm[0].text = ValueManager.bpm.ToString();
            }

            Vector3 posChild;
            posChild = Selected.transform.GetChild(0).GetChild(0).localPosition;
            posChild.y = speedBpm;
            Selected.transform.GetChild(0).GetChild(0).localPosition = posChild;
            Selected.transform.GetComponentInChildren<TextMeshPro>().text =
                posChild.y.ToString() + "\nx " + posChild.x.ToString();

        }
        catch { yield break; }

        Vector3 pos;
        pos = Selected.transform.localPosition;

        float posX;
        posX = Selected.transform.parent.localPosition.x;

        GameObject NoteField;
        NoteField = PageSystem.pageSystem.NoteField;

        GameObject Parent;
        Parent = Selected.transform.parent.gameObject;
        /*
        if (Selected != null)
        {
            Destroy(Selected);
        }
        else { Debug.LogError("삭제 대상을 찾지 못함"); yield break; }*/

        yield return new WaitForSeconds(.1f);

        mirror = new List<GameObject>(5);

        mirror.Add(PageSystem.pageSystem.NoteField);

        for (int i = 0; i < 4; i++)
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
            Vector3 pagePos;
            pagePos = PageSystem.pageSystem.NoteField.transform.localPosition;

            Destroy(PageSystem.pageSystem.NoteField);
            PageSystem.pageSystem.NoteField =
                Instantiate(Parent, NoteFieldParent.transform);
            mirror[0] = PageSystem.pageSystem.NoteField;
            mirror[0].transform.localPosition = pagePos;
            mirror[0].name = "NoteField";
            InputManager.input.inputNoteFieldSet(mirror[0]);
        }
    }

    // these fuction is triggered by button
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
        }
        catch { return; }
    }
    public void ButtonPos()
    {
        float posInput;
        Vector3 SelectedPos;
        try
        {
            posInput = Convert.ToSingle(inputPosY.text);
            Debug.Log(posInput);
            SelectedPos = Selected.transform.localPosition;
            if (posInput < 0 || posInput >= 1600)
            {
                DisplayNoteInfo();
                return;
            }
            SelectedPos.y = SelectedPos.y - SelectedPos.y % 1600 + posInput;

            Selected.transform.localPosition = SelectedPos;
        }
        catch { return; }
    }
    public void ButtonLegnth()
    {
        try
        {
            LengthNote(Convert.ToInt32(inputLegnth.text));
        }
        catch
        {   
            if (Selected == null) inputLegnth.text = "--";
            else inputLegnth.text = (Selected.transform.localScale.y / 100).ToString();
        }
    }
    public void ButtonEffect()
    {
        Vector3 forcePos;
        try
        {
            forcePos = Selected.transform.localPosition;
            forcePos.x = Convert.ToSingle(inputEffectForce.text);
            Selected.transform.GetChild(0).localPosition = forcePos;
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
        }
        catch { return; }
    }

    // Speed Bpm = 
    //      SpeedNote.child.child.transform.localposition.y
    public void ButtonSpeed()
    {
        //! StartCoroutine(SpeedNote());
    }
    public void ButtonSpeedBpm()
    {
        StartCoroutine(SpeedNoteBpm());
    }
    public void ToggleDouble()
    {
        if (Selected.tag == "chip")
        {
            Selected.transform.GetChild(0).GetChild(0)
            .GetComponent<SpriteRenderer>().enabled = isDoubleToggle.isOn;
        }
    }
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
}
