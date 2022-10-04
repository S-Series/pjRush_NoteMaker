using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteFieldMouseOver : MonoBehaviour
{
    private void OnMouseOver()
    {
        if (!InputManager.s_isNoteInputAble) return;

        Vector3 previewPos;
        previewPos = this.transform.localPosition;
        previewPos.y = this.transform.parent.localPosition.y;
        
        //* Bottom Note
        if (InputManager.s_isNoteBottom)
        {
            if (previewPos.x < 0.0f) previewPos.x = -200.0f;
            else previewPos.x = +200.0f;
        }
        //* Effect Note, Speed Note
        else if (InputManager.s_isNoteOther)
        {
            previewPos.x = 0.0f;
        }
        else
        {
            if (previewPos.x > 0.0f)
            {
                if (previewPos.x > 200.0f) previewPos.x = 300.0f;
                else previewPos.x = 100.0f;
            }
            else
            {
                if (previewPos.x < -200.0f) previewPos.x = -300.0f;
                else previewPos.x = -100.0f;
            }
        }

        InputManager.inputPosValue = previewPos;
    }

    private void OnMouseDown()
    {
        if (InputManager.s_isNoteInputAble) { InputManager.input.NoteGenerate(); }
    }
}
