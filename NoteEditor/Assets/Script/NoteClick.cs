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
        if (InputManager.s_isNoteInputAble) return;

        GameObject _noteObject;
        _noteObject = this.transform.parent.parent.gameObject;

        NoteEdit.CheckSelect();
        NoteEdit.isNoteEdit = true;
        NoteEdit.Selected = _noteObject;
        if (_noteObject.tag == NormalNoteTag || _noteObject.tag == BottomNoteTag)
        {
            NoteEdit.SelectedNormal = NormalNote.GetClass(_noteObject);
            NoteEdit.selectedType = NoteEdit.SelectedType.Normal;
            for (int i = 0; i < 3; i++)
            {
                _noteObject.transform.GetChild(0).GetChild(i)
                    .GetComponent<Collider2D>().enabled = false;
                _noteObject.transform.GetChild(1).GetChild(i)
                    .GetComponent<Collider2D>().enabled = false;
            }
        }
        else if (tag == SpeedNoteTag)
        {
            NoteEdit.SelectedSpeed = SpeedNote.GetClass(_noteObject);
            NoteEdit.selectedType = NoteEdit.SelectedType.Speed;
        }
        else if (tag == EffectNoteTag)
        {
            NoteEdit.SelectedEffect = EffectNote.GetClass(_noteObject);
            NoteEdit.selectedType = NoteEdit.SelectedType.Effect;
        }
        
        NoteEdit.noteEdit.DisplayNoteInfo();
    }
}
