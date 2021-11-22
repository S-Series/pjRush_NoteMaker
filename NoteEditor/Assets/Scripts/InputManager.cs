using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputManager : MonoBehaviour
{
    public static InputManager input;

    [SerializeField]
    GameObject NoteField;

    [SerializeField]
    GameObject EffectField;

    [SerializeField]
    GameObject[] PreviewNoteField;

    [SerializeField]
    public GameObject[] PreviewNote;

    [SerializeField]
    GameObject[] NotePrefab;

    [SerializeField]
    GameObject NullObject;

    [SerializeField]
    GameObject inputCollider;

    public GameObject InputObject;

    public bool isNoteInputAble;

    public bool isNoteBottom;

    // 0. InputNoteData [Field Number (0 ~ 4) || 1. Line Number (1 ~ 5) || 2. Prefab Number (1 ~ 6)]
    // Prefab Number 1 == Chip || Prefab Number 2 == Long
    // Prefab Number 3 == BtChip || Prefab Number 4 == BtLong
    // Prefab Number 5 == Effect || Prefab Number 6 == Bpm
    public int[] InputNoteData;

    public float posY;

    private void Awake()
    {
        input = this;
    }

    private void Start()
    {
        isNoteInputAble = false;
        isNoteBottom = false;
        InputNoteData = new int[3]{ 1, 1, 0 };
    }

    private void Update()
    {
        if (isNoteInputAble == true)
        {
            inputCollider.SetActive(true);

            float posX;

            switch (InputNoteData[1]) 
            {
                case 1:
                    posX = -300;
                    break;

                case 2:
                    posX = -100;
                    break;

                case 3:
                    posX = +100;
                    break;

                case 4:
                    posX = +300;
                    break;

                case 5:
                    posX = 0;
                    break;

                default:
                    InputObject = NullObject;
                    return;
            }
            for (int i = 0; i < 6; i++)
            {
                if (i != InputNoteData[2])
                {
                    PreviewNote[i].SetActive(false);
                }
                else PreviewNote[i].SetActive(true);
            }
            try
            {
                InputObject = PreviewNote[InputNoteData[2]];
                InputObject.transform.parent = PreviewNoteField[InputNoteData[0]].transform;
                InputObject.transform.localPosition = new Vector3(posX, posY, 0);
            }
            catch { InputObject = NullObject; return; }

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
        if (isNoteInputAble == true)
        {
            GameObject CopyObject;
            CopyObject = Instantiate(NotePrefab[InputNoteData[2]], NoteField.transform);

            float copiedPosX;
            float copiedPosZ;
            switch (InputNoteData[1])
            {
                case 1:
                    copiedPosX = -300.0f;
                    switch (InputNoteData[2])
                    {
                        case 1:
                            copiedPosZ = +0.000f;
                            break;

                        case 2:
                            copiedPosZ = +0.001f;
                            break;

                        case 3:
                            copiedPosZ = +0.002f;
                            break;

                        case 4:
                            copiedPosZ = +0.003f;
                            break;

                        default:
                            copiedPosZ = +0.000f;
                            break;
                    }
                    break;

                case 2:
                    copiedPosX = -100.0f;
                    switch (InputNoteData[2])
                    {
                        case 1:
                            copiedPosZ = +0.000f;
                            break;

                        case 2:
                            copiedPosZ = +0.001f;
                            break;

                        case 3:
                            copiedPosZ = +0.002f;
                            break;

                        case 4:
                            copiedPosZ = +0.003f;
                            break;

                        default:
                            copiedPosZ = +0.000f;
                            break;
                    }
                    break;

                case 3:
                    copiedPosX = +100.0f;
                    switch (InputNoteData[2])
                    {
                        case 1:
                            copiedPosZ = +0.000f;
                            break;

                        case 2:
                            copiedPosZ = +0.001f;
                            break;

                        case 3:
                            copiedPosZ = +0.002f;
                            break;

                        case 4:
                            copiedPosZ = +0.003f;
                            break;

                        default:
                            copiedPosZ = +0.000f;
                            break;
                    }
                    break;

                case 4:
                    copiedPosX = +300.0f;
                    switch (InputNoteData[2])
                    {
                        case 1:
                            copiedPosZ = +0.000f;
                            break;

                        case 2:
                            copiedPosZ = +0.001f;
                            break;

                        case 3:
                            copiedPosZ = +0.002f;
                            break;

                        case 4:
                            copiedPosZ = +0.003f;
                            break;

                        default:
                            copiedPosZ = +0.000f;
                            break;
                    }
                    break;

                case 5:
                    copiedPosX = 0.0f;
                    switch (InputNoteData[2])
                    {
                        case 1:
                            copiedPosZ = +0.000f;
                            break;

                        case 2:
                            copiedPosZ = +0.001f;
                            break;

                        case 3:
                            copiedPosZ = +0.002f;
                            break;

                        case 4:
                            copiedPosZ = +0.003f;
                            break;

                        default:
                            copiedPosZ = +0.000f;
                            break;
                    }
                    break;

                default:
                    Debug.LogError("Out of Range");
                    return;
            }

            float copiedPosY;
            copiedPosY = (PageSystem.pageSystem.firstPage - 1) * 1600
                + InputNoteData[0] * 4800 + posY;

            CopyObject.transform.localPosition = new Vector3(copiedPosX, copiedPosY, copiedPosZ);

            PageSystem.pageSystem.PageSet(PageSystem.pageSystem.firstPage);

            if (CopyObject.tag == "Bpm")
            {
                CopyObject.transform.GetChild(0).GetChild(0).localPosition = new Vector3(0, AutoTest.autoTest.bpm, 0);
                CopyObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = (AutoTest.autoTest.bpm).ToString();
            }
        }
    }

    public void inputNoteFieldSet(GameObject gameObject)
    {
        NoteField = gameObject;
    }
}
