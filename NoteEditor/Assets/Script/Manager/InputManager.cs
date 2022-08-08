using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputManager : MonoBehaviour
{
    public static InputManager input;
    public static bool isNoteInputAble;
    public static bool isNoteBottom;
    public static bool isNoteOther;
    public static Vector3 inputPosValue = new Vector3(0, 0, 0);
    [SerializeField] GameObject NoteField;
    [SerializeField] GameObject EffectField;
    [SerializeField] GameObject[] PreviewNoteField;
    [SerializeField] public GameObject[] PreviewNote;
    [SerializeField] GameObject[] NotePrefab;
    [SerializeField] GameObject NullObject;
    [SerializeField] GameObject inputCollider;
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
    }
    private void Start()
    {
        isNoteInputAble = false;
        isNoteBottom = false;
    }
    private void Update()
    {
        if (isNoteInputAble == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isNoteInputAble = false;
                NoteTool.disableFrame();
                NoteEdit.noteEdit.SectorSetOriginal();
                for (int i = 0; i < 6; i++)
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
            SpeedNote.speedNotes.Add(inputSpeedNote);

            NoteEdit.Selected = copyObject;
            NoteEdit.SelectedSpeed = inputSpeedNote;
            NoteEdit.selectedType = NoteEdit.SelectedType.Speed;
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

            NoteEdit.Selected = copyObject;
            NoteEdit.SelectedEffect = inputEffectNote;
            NoteEdit.selectedType = NoteEdit.SelectedType.Effect;
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
            if (inputIndexValue == 1 || inputIndexValue == 3) {inputNormalNote.legnth = 4;}
            else inputNormalNote.legnth = 0;
            if (inputIndexValue == 6) {inputNormalNote.status = NormalNote.Status.Simpled;}
            else { inputNormalNote.status = NormalNote.Status.Noraml; }
            inputNormalNote.noteObject = copyObject;
            NormalNote.normalNotes.Add(inputNormalNote);

            NoteEdit.Selected = copyObject;
            NoteEdit.SelectedNormal = inputNormalNote;
            NoteEdit.selectedType = NoteEdit.SelectedType.Normal;
        }
        NoteEdit.noteEdit.DisplayNoteInfo();
    }
    public void PreviewActivate(int previewIndex, bool isBottom, bool isOthers)
    {
        inputIndexValue = previewIndex;
        isNoteInputAble = true;
        isNoteBottom = isBottom;
        isNoteOther = isOthers;
        inputCollider.SetActive(true);
        for (int i = 0; i < 7; i++)
        {
            if (i == previewIndex) 
            {
                PreviewNote[i].SetActive(true);
                InputObject = PreviewNote[i];
            }
            else PreviewNote[i].SetActive(false);
        }
    }
}
