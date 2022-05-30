using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteFieldMouseOver : MonoBehaviour
{
    private void OnMouseOver()
    {
        if (!InputManager.isNoteInputAble) return;

        Vector3 previewPos;
        previewPos = new Vector3(0, 0, 0);
        
        //* Bottom Note
        if (InputManager.isNoteBottom)
        {

        }
        //* Effect Note, Speed Note
        else if (InputManager.isNoteOther)
        {

        }
        else
        {
            
        }

        InputManager.inputPosValue = previewPos;
    }

    private void OnMouseDown()
    {
        if (InputManager.isNoteInputAble) { InputManager.input.NoteGenerate(); }
    }
}
