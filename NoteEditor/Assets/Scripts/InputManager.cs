using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager input;

    [SerializeField]
    GameObject NoteField;

    [SerializeField]
    GameObject[] PreviewNoteField;

    [SerializeField]
    GameObject[] PreviewNote;

    [SerializeField]
    GameObject[] NotePrefab;

    [SerializeField]
    GameObject NullObject;

    public GameObject InputObject;

    public bool isNoteInputAble;

    // { Field Number (0 ~ 4), Line Number (1 ~ 5), Prefab Number (1 ~ 4) }
    public int[] InputNoteData;

    public float posY;

    private void Awake()
    {
        input = this;
    }

    private void Start()
    {
        isNoteInputAble = true;
        InputNoteData = new int[3]{ 1, 1, 0 };
    }

    private void Update()
    {
        if (isNoteInputAble == true)
        {
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
            for (int i = 0; i < 4; i++)
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
            }

            if (Input.GetMouseButtonDown(0))
            {
                GameObject copy;
                copy = Instantiate(NotePrefab[InputNoteData[2]], NoteField.transform);

                float copiedPosX;
                switch (InputNoteData[1])
                {
                    case 1:
                        copiedPosX = -300.0f;
                        break;

                    case 2:
                        copiedPosX = -100.0f;
                        break;

                    case 3:
                        copiedPosX = +100.0f;
                        break;

                    case 4:
                        copiedPosX = +300.0f;
                        break;

                    case 5:
                        copiedPosX = 0.0f;
                        break;

                    default:
                        Debug.LogError("Out of Range");
                        return;
                }

                float copiedPosY;
                copiedPosY = (PageSystem.pageSystem.firstPage - 1) * 1600
                    + InputNoteData[0] * 4800 + posY;

                copy.transform.localPosition = new Vector3(copiedPosX, copiedPosY, 0);

                PageSystem.pageSystem.PageSet(PageSystem.pageSystem.firstPage);
            }
        }
        else
        {
            InputObject = NullObject;
        }
    }
}
