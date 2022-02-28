using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteFieldMouseOver : MonoBehaviour
{
    // InputManager input;
    // public int[] InputNoteData;
    // { Field Number (0 ~ 4), Line Number (1 ~ 5), Prefab Number (1 ~ 4) }

    private void OnMouseOver()
    {
        InputManager input = InputManager.input;

        input.posY = transform.parent.parent.localPosition.y;

        switch (transform.parent.parent.localPosition.x)
        {
            case 0.0f:
                input.InputNoteData[0] = 0;
                break;

            case 925.926f:
                input.InputNoteData[0] = 1;
                break;

            case 1851.852f:
                input.InputNoteData[0] = 2;
                break;

            case 2777.778f:
                input.InputNoteData[0] = 3;
                break;

            case 3703.704f:
                input.InputNoteData[0] = 4;
                break;

            default:
                input.isNoteInputAble = false;
                break;
        }

        if (input.isNoteBottom == true)
        {
            switch (this.transform.parent.gameObject.name)
            {
                case "1":
                case "2":
                    input.InputNoteData[1] = 2;
                    break;

                case "3":
                case "4":
                    input.InputNoteData[1] = 3;
                    break;

                default:
                    input.isNoteInputAble = false;
                    break;
            }
        }
        else
        {
            switch (this.transform.parent.gameObject.name)
            {
                case "1":
                    input.InputNoteData[1] = 1;
                    break;

                case "2":
                    input.InputNoteData[1] = 2;
                    break;

                case "3":
                    input.InputNoteData[1] = 3;
                    break;

                case "4":
                    input.InputNoteData[1] = 4;
                    break;

                default:
                    input.isNoteInputAble = false;
                    break;
            }
        }
    }

    private void OnMouseDown()
    {
        InputManager.input.NoteGenerate();
    }
}
