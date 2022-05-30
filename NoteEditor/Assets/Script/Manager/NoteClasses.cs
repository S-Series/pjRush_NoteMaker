using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteClasses : MonoBehaviour
{
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
}

public class NormalNote
{
    public static List<NormalNote> normalNotes = new List<NormalNote>();
    public GameObject noteObject;
    public int line;
    public int legnth;
    public float ms;
    public float pos;
    public bool isOver;
    public static void Sorting()
    {
        normalNotes.Sort(delegate (NormalNote A, NormalNote B)
        {
            if (A.ms > B.ms) return -1;
            else if (A.ms < B.ms) return +1;
            else
            {
                if (A.line > B.line) return -1;
                else if (A.line < B.line) return +1;
                else Debug.LogError("Note Overlap"); return 0;
            }
        });
    }
    public static NormalNote GetClass(GameObject _noteObject)
    {
        return normalNotes.Find(item => item.noteObject == _noteObject);
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
            if (A.ms > B.ms) return -1;
            else if (A.ms < B.ms) return +1;
            else Debug.LogError("Note Overlap"); return 0;
        });
    }
    public static SpeedNote GetClass(GameObject _noteObject)
    {
        return speedNotes.Find(item => item.noteObject == _noteObject);
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
    public bool type;
    public float value;
    public float ms;
    public float pos;
    public static void Sorting()
    {
        effectNotes.Sort(delegate (EffectNote A, EffectNote B)
        {
            if (A.ms > B.ms) return -1;
            else if (A.ms < B.ms) return +1;
            else Debug.LogError("Note Overlap"); return 0;
        });
    }
    public static EffectNote GetClass(GameObject _noteObject)
    {
        return effectNotes.Find(item => item.noteObject == _noteObject);
    }
    public static void DeleteNote(EffectNote _effectNote)
    {
        effectNotes.RemoveAll(item => item == _effectNote);
    }
}
