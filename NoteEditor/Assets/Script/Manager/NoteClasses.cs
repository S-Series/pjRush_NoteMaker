using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteClasses : MonoBehaviour
{
    [SerializeField] GameObject NoteField;
    private static Transform _Notefield;
    private const string NormalNoteTag = "Noraml";
    private const string BottomNoteTag = "Bottom";
    private const string SpeedNoteTag = "BPM";
    private const string EffectNoteTag = "Effect";
    private void Awake() 
    {
        _Notefield = NoteField.transform;
    }
    public static void ResetNotes()
    {
        foreach(NormalNote normalNote in NormalNote.normalNotes)
        {
            Destroy(normalNote.noteObject);
        }
        foreach(SpeedNote speedNote in SpeedNote.speedNotes)
        {
            Destroy(speedNote.noteObject);
        }
        foreach(EffectNote effectNote in EffectNote.effectNotes)
        {
            Destroy(effectNote.noteObject);
        }
        foreach(LineNote lineNote in LineNote.lineNotes)
        {
            Destroy(lineNote.noteObject);
        }
        NormalNote.normalNotes = new List<NormalNote>();
        SpeedNote.speedNotes = new List<SpeedNote>();
        EffectNote.effectNotes = new List<EffectNote>();
        LineNote.lineNotes = new List<LineNote>();
    }
    public static void SortingNotes()
    {
        NormalNote.Sorting();
        SpeedNote.Sorting();
        EffectNote.Sorting();
    }
    public static void EnableCollider(bool isActive)
    {
        foreach(NormalNote note in NormalNote.normalNotes)
        {
            for (int i = 0; i < 3; i++)
            {
                note.noteObject.transform.GetChild(0).GetChild(i)
                    .GetComponent<BoxCollider2D>().enabled = isActive;
                note.noteObject.transform.GetChild(1).GetChild(i)
                    .GetComponent<BoxCollider2D>().enabled = isActive;
            }
        }
        foreach(SpeedNote note in SpeedNote.speedNotes)
        {
            note.noteObject.GetComponent<BoxCollider2D>().enabled = isActive;
        }
        foreach(EffectNote note in EffectNote.effectNotes)
        {
            note.noteObject.GetComponent<BoxCollider2D>().enabled = isActive;
        }
    }
    public static IEnumerator CalculateNoteMs()
    {
        NormalNote editNormal;
        SpeedNote editSpeed;
        EffectNote editEffect;
        int speedIndex, editMs;
        float editPos, editBpm;

        //* SpeedNote ---------- //
        editMs = 0;
        editPos = 0.0f;
        editBpm = ValueManager.bpm;
        for (int i = 0; i < SpeedNote.speedNotes.Count; i++)
        {
            editSpeed = SpeedNote.speedNotes[i];

            int calMs;
            calMs = Convert.ToInt32(editMs + (150 * (editSpeed.pos - editPos) / editBpm));

            editSpeed.ms = calMs;
            editMs = calMs;
            editPos = editSpeed.pos;
            editBpm = editSpeed.bpm * editSpeed.multiply;
        }
        yield return null;
        //* NormalNote ---------- //
        speedIndex = SpeedNote.speedNotes.Count - 1;
        for (int i = 0; i < NormalNote.normalNotes.Count; i++)
        {
            editNormal = NormalNote.normalNotes[i];

            editMs = 0;
            editPos = 0.0f;
            editBpm = ValueManager.bpm;

            for (int j = speedIndex; j > -1; j--)
            {
                if (SpeedNote.speedNotes[j].pos <= editNormal.pos)
                {
                    editMs = Convert.ToInt32(SpeedNote.speedNotes[j].ms);
                    editPos = SpeedNote.speedNotes[j].pos;
                    editBpm = SpeedNote.speedNotes[j].bpm * SpeedNote.speedNotes[j].multiply;
                    break;
                }
            }
            editNormal.ms = Convert.ToInt32(editMs + (editNormal.pos - editPos) * 150 / editBpm);
        }

        //* EffectNote ---------- //
        editMs = 0;
        editPos = 0.0f;
        editBpm = ValueManager.bpm;
        for (int i = 0; i < EffectNote.effectNotes.Count; i++)
        {
            editEffect = EffectNote.effectNotes[i];
            for (int j = SpeedNote.speedNotes.Count - 1; j > -1; j--)
            {
                editMs = 0;
                editPos = 0.0f;
                editBpm = ValueManager.bpm;
                if (SpeedNote.speedNotes[j].pos <= editEffect.pos)
                {
                    editMs = Convert.ToInt32(SpeedNote.speedNotes[j].ms);
                    editPos = SpeedNote.speedNotes[j].pos;
                    editBpm = SpeedNote.speedNotes[j].bpm * SpeedNote.speedNotes[j].multiply;
                    break;
                }
            }
            editEffect.ms = editMs + Mathf.RoundToInt((editEffect.pos - editPos) * 150 / editBpm);
        }
        editEffect = null;
        editMs = 0;
        editBpm = ValueManager.bpm;
        
        //* Retouching ---------- //
        for (int i = 0; i < EffectNote.effectNotes.Count; i++)
        {
            editEffect = EffectNote.effectNotes[i];
            editMs = 0;
            editBpm = ValueManager.bpm;
            for (int j = speedIndex; j > -1; j--)
            {
                if (SpeedNote.speedNotes[j].pos <= editEffect.pos)
                {
                    editMs = Convert.ToInt32(SpeedNote.speedNotes[j].ms);
                    editBpm = SpeedNote.speedNotes[j].bpm * SpeedNote.speedNotes[j].multiply;
                    break;
                }
            }
            editPos = editEffect.pos;
            editMs = Convert.ToInt32(editEffect.value * 150.0f / editBpm);
            if (!editEffect.isPause) editMs *= -1;

            foreach (NormalNote _normalNote in NormalNote.normalNotes)
            {
                if (_normalNote.pos >= editPos) _normalNote.ms += editMs;
            }
            foreach (SpeedNote _speedNote in SpeedNote.speedNotes)
            {
                if (_speedNote.pos > editPos) _speedNote.ms += editMs;
            }
            foreach (EffectNote _effectNote in EffectNote.effectNotes)
            {
                if (_effectNote.pos > editPos) _effectNote.ms += editMs;
            }
            yield return null;
        }
    
        #region  Debugging ---------- //
        /*
        for (int i = 0; i < NormalNote.normalNotes.Count; i++)
        {
            Debug.Log("NormalNote " + i.ToString() + " : " + (NormalNote.normalNotes[i].ms).ToString());
        }
        */
        /*
        for (int i = 0; i < SpeedNote.speedNotes.Count; i++)
        {
            Debug.Log("SpeedNote " + i.ToString() + " : " + (SpeedNote.speedNotes[i].ms).ToString());
        }
        */
        /*
        for (int i = 0; i < EffectNote.effectNotes.Count; i++)
        {
            Debug.Log("EffectNote " + i.ToString() + " : " + (EffectNote.effectNotes[i].ms).ToString());
        }
        */
        #endregion
    }
}

public class NormalNote
{
    public static List<NormalNote> normalNotes = new List<NormalNote>();
    public GameObject noteObject;
    public int ms, line, legnth;
    public float pos;
    public bool isPowered;
    
    public static void Sorting()
    {
        normalNotes.Sort(delegate (NormalNote A, NormalNote B)
        {
            if (Mathf.Approximately(A.pos, B.pos))
            {
                if (!A.isPowered && B.isPowered) return +1;
                else if (A.isPowered && !B.isPowered) return -1;
            }
            if (A.pos > B.pos) return +1;
            else if (A.pos < B.pos) return -1;
            else
            {
                if (A.line > B.line) return +1;
                else if (A.line < B.line) return -1;
                else return 0;
            }
        });
    }
    public static NormalNote GetClass(GameObject _noteObject)
    {
        return normalNotes.Find(item => item.noteObject == _noteObject);
    }
    public static NormalNote copyData(NormalNote _data, bool _isCopyObject = false)
    {
        if (_data == null) { return null; }
        
        NormalNote _newData = new NormalNote();
        _newData.line = _data.line;
        _newData.legnth = _data.legnth;
        _newData.ms = _data.ms;
        _newData.pos = _data.pos;
        _newData.isPowered = _data.isPowered;
        if (!_isCopyObject) { _newData.noteObject = null; }
        else { _newData.noteObject = _data.noteObject; }

        return _newData;
    }
    public static bool CheckNote(GameObject _noteObject)
    {
        return normalNotes.Contains(GetClass(_noteObject));
    }
    public static void DeleteNote(NormalNote _normalNote)
    {
        normalNotes.RemoveAll(item => item == _normalNote);
    }
}

public class SpeedNote
{
    public static List<SpeedNote> speedNotes = new List<SpeedNote>();
    public GameObject noteObject;
    public int ms;
    public float bpm;
    public float multiply;
    public float pos;
    public static void Sorting()
    {
        speedNotes.Sort(delegate (SpeedNote A, SpeedNote B)
        {
            if (A.pos > B.pos) return +1;
            else if (A.pos < B.pos) return -1;
            else {Debug.LogError("Note Overlap"); return 0;}
        });
    }
    public static SpeedNote GetClass(GameObject _noteObject)
    {
        return speedNotes.Find(item => item.noteObject == _noteObject);
    }
    public static bool CheckNote(GameObject _noteObject)
    {
        return speedNotes.Contains(GetClass(_noteObject));
    }
    public static void DeleteNote(SpeedNote _speedNote)
    {
        speedNotes.RemoveAll(item => item == _speedNote);
    }
}

public class EffectNote
{
    public static List<EffectNote> effectNotes = new List<EffectNote>();
    public GameObject noteObject;
    public int ms;
    public bool isPause;
    public float value;
    public float pos;
    public static void Sorting()
    {
        effectNotes.Sort(delegate (EffectNote A, EffectNote B)
        {
            //if (Mathf.Approximately(A.pos, B.pos)) {Debug.Log("Note Overlap"); return 0;}

            if (A.pos > B.pos) { return +1; }
            else if (A.pos < B.pos) { return -1; }
            else
            {
                if (A.isPause && !B.isPause) { return +1; }
                else if (!A.isPause && B.isPause) { return -1; }
                else return 0;
            }
        });
    }
    public static EffectNote GetClass(GameObject _noteObject)
    {
        return effectNotes.Find(item => item.noteObject == _noteObject);
    }
    public static bool CheckNote(GameObject _noteObject)
    {
        return effectNotes.Contains(GetClass(_noteObject));
    }
    public static void DeleteNote(EffectNote _effectNote)
    {
        effectNotes.RemoveAll(item => item == _effectNote);
    }
}

public class LineNote
{
    public static List<LineNote> lineNotes = new List<LineNote>();
    public GameObject noteObject;
    public int power, noteMs, durationPower;
    public float pos, duration;
    public bool isHasDuration = false;
    public static void Sorting()
    {
        lineNotes.Sort(delegate (LineNote A, LineNote B)    
        {
            if (A.pos > B.pos) return +1;
            else if (A.pos < B.pos) return -1;
            else {Debug.LogError("Note Overlap"); return 0;}
        });
    }
    public static bool IsNoteOverlap(float _pos)
    {
        if (lineNotes.Exists(item => item.pos == _pos)) { return true; }
        else { return false; }
    }
    public static LineNote GetLineNote(GameObject _object)
    {
        return lineNotes.Find(item => item.noteObject == _object);
    }
    public static LineNote CopyLineNoteDate(LineNote _targetNote, bool _isCopyNoteObject)
    {
        LineNote _newNote = new LineNote();

        if (!_isCopyNoteObject) { _newNote.noteObject = null; }
        else { _newNote.noteObject = _targetNote.noteObject; }
        _newNote.noteMs = _targetNote.noteMs;
        _newNote.pos = _targetNote.pos;
        _newNote.power = _targetNote.power;
        _newNote.duration = _targetNote.duration;
        _newNote.isHasDuration = _targetNote.isHasDuration;

        return _newNote;
    }
    public static void DeleteNote(LineNote _note)
    {
        lineNotes.RemoveAll(item => item == _note);
    }
}
