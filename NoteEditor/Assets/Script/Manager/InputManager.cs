using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputManager : MonoBehaviour
{
    public static InputManager input;
    private static int s_noteInputLegnth = 4;
    private static bool s_isInputLong = false;
    public static bool s_isNoteInputAble;
    public static bool s_isNoteBottom;
    public static bool s_isNoteOther;
    public static Vector3 inputPosValue = new Vector3(0, 0, 0);
    [SerializeField] GameObject NoteField;
    [SerializeField] GameObject EffectField;
    [SerializeField] GameObject[] PreviewNoteField;
    [SerializeField] public GameObject[] PreviewNote;
    [SerializeField] GameObject[] NotePrefab;
    [SerializeField] GameObject NullObject;
    [SerializeField] GameObject inputCollider;
    [SerializeField] TMP_InputField noteLegnthInput;
    [SerializeField] Toggle toggleLong;
    public GameObject InputObject;
    private int inputIndexValue = 0;
    public float posY;
    private void Awake()
    {
        input = this;
        foreach (GameObject gameObject in PreviewNote)
        {
            gameObject.SetActive(false);
        }
        ToggleChange();
        NoteLegnthChange();
    }
    private void Start()
    {
        s_isNoteInputAble = false;
        s_isNoteBottom = false;
    }
    private void Update()
    {
        if (s_isNoteInputAble == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                s_isNoteInputAble = false;
                NoteTool.disableFrame();
                NoteEdit.noteEdit.SectorSetOriginal();
                for (int i = 0; i < 5; i++)
                {
                    PreviewNote[i].SetActive(false);
                }
            }
            InputObject.transform.localPosition = inputPosValue;
        }
        else
        {
            InputObject = NullObject;
            inputCollider.SetActive(false);

            if (Input.GetKeyDown(KeyCode.Tab)) 
                { s_isInputLong = !s_isInputLong; ToggleChange(); }
        }
    }
    public void NoteLegnthChange()
    {
        try
        {
            s_noteInputLegnth = Convert.ToInt32(noteLegnthInput.text);
        }
        catch { noteLegnthInput.text = s_noteInputLegnth.ToString(); return; }

        NoteOption _option;
        if (s_noteInputLegnth < 0)
        {
            Vector3 _pos;
            Vector3 _scale;
            for (int i = 0; i < 3; i++)
            {
                _option = PreviewNote[i].GetComponent<NoteOption>();
            }
        }
        else
        {
            Vector3 _pos;
            Vector3 _scale;
            for (int i = 0; i < 3; i++)
            {
                _option = PreviewNote[i].GetComponent<NoteOption>();
            }
        }
    }
    public void ToggleChange()
    {
        NoteOption _option;
        s_isInputLong = toggleLong.isOn;
        for (int i = 0; i < 3; i++)
        {
            _option = PreviewNote[i].GetComponent<NoteOption>();
        }
    }
    public void NoteGenerate()
    {
        print(inputIndexValue);

        NoteEdit.CheckSelect();
        GameObject copyObject;
        copyObject = Instantiate(NotePrefab[inputIndexValue], NoteField.transform);
        Vector3 generatePos = inputPosValue;
        generatePos.y += PageSystem.nowOnPage * 1600.0f;
        copyObject.transform.localPosition = generatePos;

        if (inputIndexValue == 4)
        {
            SpeedNote inputSpeedNote;
            inputSpeedNote = new SpeedNote();
            inputSpeedNote.ms = 0.0f;
            inputSpeedNote.pos = generatePos.y;
            inputSpeedNote.bpm = ValueManager.bpm;
            inputSpeedNote.multiply = 1.00f;
            inputSpeedNote.noteObject = copyObject;
            NoteEdit.noteEdit.DisplaySpeedText(inputSpeedNote);
            SpeedNote.speedNotes.Add(inputSpeedNote);
        }
        else if (inputIndexValue == 5)
        {
            EffectNote inputEffectNote;
            inputEffectNote = new EffectNote();
            inputEffectNote.ms = 0.0f;
            inputEffectNote.pos = generatePos.y;
            inputEffectNote.isPause = false;
            inputEffectNote.value = 400.0f;
            inputEffectNote.noteObject = copyObject;
            EffectNote.effectNotes.Add(inputEffectNote);
        }
        else
        {
            NormalNote inputNormalNote;
            inputNormalNote = new NormalNote();

            if (Mathf.Approximately(generatePos.x, +200.0f)) inputNormalNote.line = 6;
            else if (Mathf.Approximately(generatePos.x, -200.0f)) inputNormalNote.line = 5;
            else if (Mathf.Approximately(generatePos.x, +300.0f)) inputNormalNote.line = 4;
            else if (Mathf.Approximately(generatePos.x, +100.0f)) inputNormalNote.line = 3;
            else if (Mathf.Approximately(generatePos.x, -100.0f)) inputNormalNote.line = 2;
            else inputNormalNote.line = 1;

            inputNormalNote.ms = 0.0f;
            inputNormalNote.pos = generatePos.y;

            if (s_isInputLong)
            { 
                inputNormalNote.legnth = s_noteInputLegnth;

                Vector3 _scale;
                _scale = copyObject.transform.GetChild(0).localScale;
                _scale.y = s_noteInputLegnth / 4.0f;
                copyObject.transform.GetChild(0).localScale = _scale;
            }
            else
            {
                inputNormalNote.legnth = s_noteInputLegnth;

                Vector3 _scale;
                _scale = copyObject.transform.GetChild(0).localScale;
                _scale.y = 0.0f;
                copyObject.transform.GetChild(0).localScale = _scale;
            }

            if (inputIndexValue == 6) { inputNormalNote.isPowered = true; }
            else { inputNormalNote.isPowered = false; }
            inputNormalNote.noteObject = copyObject;
            NormalNote.normalNotes.Add(inputNormalNote);
        }
        NoteEdit.noteEdit.DisplayNoteInfo();
    }
    public void PreviewActivate(int _prefabIndex, bool _isBottom = false, bool _isEffect = false)
    {
        inputIndexValue = _prefabIndex;
        s_isNoteInputAble = true;
        s_isNoteBottom = _isBottom;
        s_isNoteOther = _isEffect;
        inputCollider.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            if (i == _prefabIndex) 
            {
                PreviewNote[i].SetActive(true);
                InputObject = PreviewNote[i];
            }
            else PreviewNote[i].SetActive(false);
        }
    
        if (!_isEffect)
        {
            Vector3 _scale;
            _scale = PreviewNote[_prefabIndex].transform.GetChild(0).localScale;
        }
    }
}
