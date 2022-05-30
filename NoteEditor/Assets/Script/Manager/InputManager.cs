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
        }
        else
        {
            InputObject = NullObject;
            inputCollider.SetActive(false);
        }
    }
    public void NoteGenerate()
    {
        Vector3 generatePos;
        generatePos = inputPosValue;
    }
    public void inputNoteFieldSet(GameObject gameObject)
    {
        NoteField = gameObject;
    }
    private void PreviewView()
    {
        
    }
}
