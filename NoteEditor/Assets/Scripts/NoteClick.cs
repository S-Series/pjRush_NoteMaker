using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        InputManager.input.isNoteInputAble = false;
        NoteEdit.noteEdit.isNoteEdit = true;
        NoteEdit.noteEdit.Selected = this.gameObject;
        NoteEdit.noteEdit.DisplayNoteInfo();
    }
}
