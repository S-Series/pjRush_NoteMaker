using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public static List<NormalNote> normalNote;
    public static List<SpeedNote> speedNote;
    public static List<EffectNote> effectNote;
    private void Awake()
    {
        normalNote = new List<NormalNote>();
        speedNote = new List<SpeedNote>();
        effectNote = new List<EffectNote>();
    }
    public static void SortNote()
    {
        normalNote.Sort(delegate (NormalNote A, NormalNote B)
        {
            if (A.pos > B.pos) return 1;
            else if (A.pos < B.pos) return -1;
            else
            {
                if (A.line > B.line) return 1;
                else if (A.line < B.line) return -1;
            }
            return 0;
        });
    }
    public static void SortSpeedNote()
    {
        speedNote.Sort(delegate (SpeedNote A, SpeedNote B)
        {
            if (A.pos > B.pos) return 1;
            else if (A.pos < B.pos) return -1;
            return 0;
        });
    }
    public static void SortEffectNote()
    {

    }
    public static int CalculateNoteMs(int posValue)
    {
        NoteClasses.SortingNotes();

        SpeedNote _target;
        _target = null;
        for (int i = SpeedNote.speedNotes.Count - 1; i > -1; i--)
        {
            if (SpeedNote.speedNotes[i].pos <= posValue) 
            {
                _target = SpeedNote.speedNotes[i];
                break;
            }
        }
        if (_target == null) 
            { return Mathf.RoundToInt(posValue * 150 / ValueManager.bpm); }
        else
        {
            return Mathf.RoundToInt(_target.ms + (
                posValue - _target.pos) * 150 / (_target.bpm * _target.multiply));
        }
    }
}
