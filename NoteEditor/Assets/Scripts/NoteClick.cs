using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (AutoTest.autoTest.isTest == false)
        {
            NoteEdit noteEdit;
            noteEdit = NoteEdit.noteEdit;

            if (this.gameObject.tag == "Effect")
            {
                InputManager.input.isNoteInputAble = false;
                noteEdit.isNoteEdit = true;
                noteEdit.SectorSetEffect();
                noteEdit.Selected = this.gameObject;
            }
            else if (this.gameObject.tag == "Bpm")
            {
                InputManager.input.isNoteInputAble = false;
                noteEdit.isNoteEdit = true;
                noteEdit.SectorSetSpeed();
                noteEdit.Selected = this.gameObject;
            }
            else
            {
                InputManager.input.isNoteInputAble = false;
                noteEdit.isNoteEdit = true;
                noteEdit.SectorSetOriginal();
                noteEdit.Selected = this.gameObject;
                noteEdit.DisplayNoteInfo();
            }
        }
    }
}
