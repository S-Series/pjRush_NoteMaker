using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager input;

    [SerializeField]
    GameObject[] NoteField;

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

    public float pos_y;

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
            float pos_x;
            switch (InputNoteData[1]) 
            {
                case 1: 
                    pos_x = -300;
                    break;

                case 2: 
                    pos_x = -100;
                    break;

                case 3:
                    pos_x = +100;
                    break;

                case 4:
                    pos_x = +300;
                    break;

                case 5:
                    pos_x = 0;
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
                InputObject.transform.parent = NoteField[InputNoteData[0]].transform;
                InputObject.transform.localPosition = new Vector3(pos_x, pos_y, 0);
            }
            catch { InputObject = NullObject; return; }
        }
        else
        {
            InputObject = NullObject;
        }
    }
}
