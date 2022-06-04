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
        GameObject copyObject;
        copyObject = Instantiate(NotePrefab[inputIndexValue], NoteField.transform);
        copyObject.transform.localPosition = inputPosValue;

        if (inputIndexValue == 5)
        {
            SpeedNote inputSpeedNote;
            inputSpeedNote = new SpeedNote();
            if (SpeedNote.speedNotes.Exists(value => Mathf.Approximately(value.pos, inputPosValue.y)))
            {
                // 중복값 오류 알람
                return;
            }
        }
        else if (inputIndexValue == 6)
        {
            EffectNote inputEffectNote;
            inputEffectNote = new EffectNote();
            if (EffectNote.effectNotes.Exists(value => Mathf.Approximately(value.pos, inputPosValue.y)))
            {
                // 중복값 오류 알람
                return;
            }
        }
        else
        {
            NormalNote inputNormalNote;
            inputNormalNote = new NormalNote();

            if (inputPosValue.x < -200.0f) inputNormalNote.line = 1;
            else if (inputPosValue.x < -0.0f) inputNormalNote.line = 2;
            else if (inputPosValue.x < +200.0f) inputNormalNote.line = 3;
            else inputNormalNote.line = 4;

            if (NormalNote.normalNotes.Exists(value => Mathf.Approximately(value.pos, inputPosValue.y))
            && NormalNote.normalNotes.Exists(value => value.line == inputNormalNote.line))
            {
                // 중복 노트 오류 알람
                return;
            }
        }
    }
    public void PreviewActivate(int previewIndex, bool isBottom, bool isOthers)
    {
        inputIndexValue = previewIndex;
        isNoteInputAble = true;
        isNoteBottom = isBottom;
        isNoteOther = isOthers;
        inputCollider.SetActive(true);
        for (int i = 0; i < 6; i++)
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
