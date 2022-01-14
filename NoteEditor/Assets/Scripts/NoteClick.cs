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
                switch (noteEdit.Selected.gameObject.tag)
                {
                    case "chip":
                    case "long":
                        noteEdit.Selected.GetComponentInChildren<SpriteRenderer>()
                            .color = new Color32(230, 230, 230, 255);
                        break;

                    case "btChip":
                        noteEdit.Selected.GetComponentInChildren<SpriteRenderer>()
                            .color = new Color32(255, 255, 255, 255);
                        break;

                    case "btLong":
                        noteEdit.Selected.GetComponentInChildren<SpriteRenderer>()
                            .color = new Color32(255, 255, 255, 150);
                        break;

                    default:
                        break;
                }
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
                    switch (noteEdit.Selected.tag)
                    {
                        case "chip":
                            noteEdit.Selected.GetComponentInChildren<SpriteRenderer>()
                                .color = new Color32(0, 255, 128, 255);
                            break;

                        case "long":
                            noteEdit.Selected.GetComponentInChildren<SpriteRenderer>()
                                .color = new Color32(0, 255, 128, 230);
                            break;

                        case "btChip":
                            noteEdit.Selected.GetComponentInChildren<SpriteRenderer>()
                                .color = new Color32(0, 255, 255, 255);
                            break;

                        case "btLong":
                            noteEdit.Selected.GetComponentInChildren<SpriteRenderer>()
                                .color = new Color32(0, 255, 255, 150);
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }
    }
}
