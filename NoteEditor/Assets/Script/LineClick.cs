using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineClick : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    private void OnMouseDown()
    {
        float _pos;
        _pos = transform.parent.localPosition.y - (PageSystem.nowOnPage * 1600);
        if (CompareTag("Finish")) { _pos += transform.localPosition.y; }

        if (_pos >= 14400) { canvas.worldCamera = LineEdit.s_lineEdit.lineCamera[3]; }
        else if (_pos >= 9600) { canvas.worldCamera = LineEdit.s_lineEdit.lineCamera[2]; }
        else if (_pos >= 4800) { canvas.worldCamera = LineEdit.s_lineEdit.lineCamera[1]; }
        else { canvas.worldCamera = LineEdit.s_lineEdit.lineCamera[0]; }
        
        NoteEdit.DeselectNote();
        NoteEdit.SectorNull();
        NoteClasses.EnableCollider(false);
        LineEdit.SelectLineNote(transform.parent.gameObject);
        LineEdit.SectorActivate(true);
    }
}
