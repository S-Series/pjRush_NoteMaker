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
        NormalNote.normalNotes = new List<NormalNote>();
        SpeedNote.speedNotes = new List<SpeedNote>();
        EffectNote.effectNotes = new List<EffectNote>();
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
            note.noteObject.GetComponent<BoxCollider2D>().enabled = isActive;
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
        int speedIndex;
        float editMs, editPos, editBpm;

        //* SpeedNote ---------- //
        editMs = 0.0f;
        editPos = 0.0f;
        editBpm = ValueManager.bpm;
        for (int i = 0; i < SpeedNote.speedNotes.Count; i++)
        {
            editSpeed = SpeedNote.speedNotes[i];

            float calMs;
            calMs = editMs + (150 * (editSpeed.pos - editPos) / editBpm);

            editSpeed.ms = calMs;
            editMs = calMs;
            editPos = editSpeed.pos;
            editBpm = editSpeed.bpm * editSpeed.multiply;
            yield return null;
        }

        //* NormalNote ---------- //
        speedIndex = SpeedNote.speedNotes.Count - 1;
        for (int i = 0; i < NormalNote.normalNotes.Count; i++)
        {
            editNormal = NormalNote.normalNotes[i];

            editMs = 0.0f;
            editPos = 0.0f;
            editBpm = ValueManager.bpm;

            for (int j = speedIndex; j > -1; j--)
            {
                if (SpeedNote.speedNotes[j].pos <= editNormal.pos)
                {
                    editMs = SpeedNote.speedNotes[j].ms;
                    editPos = SpeedNote.speedNotes[j].pos;
                    editBpm = SpeedNote.speedNotes[j].bpm * SpeedNote.speedNotes[j].multiply;
                    break;
                }
                yield return null;
            }
            editNormal.ms = editMs + (editNormal.pos - editPos) * 150 / editBpm;
            yield return null;
        }

        //* EffectNote ---------- //
        editMs = 0.0f;
        editPos = 0.0f;
        editBpm = ValueManager.bpm;
        for (int i = 0; i < EffectNote.effectNotes.Count; i++)
        {
            editEffect = EffectNote.effectNotes[i];
            for (int j = SpeedNote.speedNotes.Count - 1; j > -1; j--)
            {
                editMs = 0.0f;
                editPos = 0.0f;
                editBpm = ValueManager.bpm;
                if (SpeedNote.speedNotes[j].pos <= editEffect.pos)
                {
                    editMs = SpeedNote.speedNotes[j].ms;
                    editPos = SpeedNote.speedNotes[j].pos;
                    editBpm = SpeedNote.speedNotes[j].bpm * SpeedNote.speedNotes[j].multiply;
                    break;
                }
            }
            editEffect.ms = editMs + (editEffect.pos - editPos) * 150 / editBpm;
            yield return null;
        }
        editEffect = null;
        editMs = 0;
        editBpm = ValueManager.bpm;
        
        //* Retouching ---------- //
        for (int i = 0; i < EffectNote.effectNotes.Count; i++)
        {
            editEffect = EffectNote.effectNotes[i];
            editMs = 0.0f;
            editBpm = ValueManager.bpm;
            for (int j = speedIndex; j > -1; j--)
            {
                if (SpeedNote.speedNotes[j].pos <= editEffect.pos)
                {
                    editMs = SpeedNote.speedNotes[j].ms;
                    editBpm = SpeedNote.speedNotes[j].bpm * SpeedNote.speedNotes[j].multiply;
                    break;
                }
            }
            editPos = editEffect.pos;
            editMs = editEffect.value * 150.0f / editBpm;
            if (!editEffect.isPause) editMs *= -1.0f;

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
    public enum Status { Noraml, Powered, Simpled };
    public GameObject noteObject;
    public int line;
    public int legnth;
    public float ms;
    public float pos;
    public Status status = Status.Noraml;
    public static void Sorting()
    {
        normalNotes.Sort(delegate (NormalNote A, NormalNote B)
        {
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
    public float bpm;
    public float multiply;
    public float ms;
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
    public bool isPause;
    public float value;
    public float ms;
    public float pos;
    public static void Sorting()
    {
        effectNotes.Sort(delegate (EffectNote A, EffectNote B)
        {
            if (Mathf.Approximately(A.pos, B.pos)) {Debug.Log("Note Overlap"); return 0;}

            if (A.pos > B.pos) {return +1;}
            else if (A.pos < B.pos) {return -1;}
            else {return 0;}
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
