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

            if (noteEdit.Selected != null) 
            {

            }

            switch (this.gameObject.tag)
            {
                case "Effect":
                    InputManager.input.isNoteInputAble = false;
                    noteEdit.isNoteEdit = true;
                    noteEdit.SectorSetEffect();
                    noteEdit.Selected = this.gameObject;
                    break;

                case "Bpm":
                    InputManager.input.isNoteInputAble = false;
                    noteEdit.isNoteEdit = true;
                    noteEdit.SectorSetSpeed();
                    noteEdit.Selected = this.gameObject;
                    noteEdit.inputSpeedBpm.text
                        = this.transform.GetChild(0).GetChild(0).localPosition.y.ToString();
                    break;

                default:
                    InputManager.input.isNoteInputAble = false;
                    noteEdit.isNoteEdit = true;
                    noteEdit.SectorSetOriginal();
                    noteEdit.Selected = this.gameObject;
                    noteEdit.DisplayNoteInfo();
                    break;
            }
        }
    }
}
