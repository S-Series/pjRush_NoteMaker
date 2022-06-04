using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (AutoTest.isTest) return;
        if (InputManager.isNoteInputAble) return;

        NoteEdit.isNoteEdit = true;
        NoteEdit.Selected = this.gameObject;
        NoteEdit.SelectedNormal = NormalNote.GetClass(this.gameObject);
        NoteEdit.SelectedSpeed = SpeedNote.GetClass(this.gameObject);
        NoteEdit.SelectedEffect = EffectNote.GetClass(this.gameObject);
        NoteEdit.noteEdit.DisplayNoteInfo();
    }
}
