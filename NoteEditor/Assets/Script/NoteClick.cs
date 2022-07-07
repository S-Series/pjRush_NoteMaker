using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteClick : MonoBehaviour
{
    private const string NormalNoteTag = "Normal";
    private const string BottomNoteTag = "Bottom";
    private const string SpeedNoteTag = "Bpm";
    private const string EffectNoteTag = "Effect";

    private void OnMouseDown()
    {
        if (AutoTest.s_isTest) return;
        if (InputManager.isNoteInputAble) return;

        NoteEdit.isNoteEdit = true;
        NoteEdit.Selected = this.gameObject;
        if (tag == NormalNoteTag || tag == BottomNoteTag)
        {
            NoteEdit.SelectedNormal = NormalNote.GetClass(this.gameObject);
            NoteEdit.selectedType = NoteEdit.SelectedType.Normal;
        }
        else if (tag == SpeedNoteTag)
        {
            NoteEdit.SelectedSpeed = SpeedNote.GetClass(this.gameObject);
            NoteEdit.selectedType = NoteEdit.SelectedType.Speed;
        }
        else if (tag == EffectNoteTag)
        {
            NoteEdit.SelectedEffect = EffectNote.GetClass(this.gameObject);
            NoteEdit.selectedType = NoteEdit.SelectedType.Effect;
        }
        NoteEdit.noteEdit.DisplayNoteInfo();
    }
}
