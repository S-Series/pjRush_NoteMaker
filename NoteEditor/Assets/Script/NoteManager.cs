using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public static List<Note> normalNote;
    public static List<SpeedNote> speedNote;
    public static List<EffectNote> effectNote;
    private void Awake() {
        normalNote = new List<Note>();
        speedNote = new List<SpeedNote>();
        effectNote = new List<EffectNote>();
    }
    public static void SortNote(){
        normalNote.Sort(delegate (Note A, Note B)
        {
            if (A.pos > B.pos) return 1;
            else if (A.pos < B.pos) return -1;
            else{
                if (A.line > B.line) return 1;
                else if (A.line < B.line) return -1;
            }
            return 0;
        });
    }
    public static void SortSpeedNote(){
        speedNote.Sort(delegate (SpeedNote A, SpeedNote B)
        {
            if (A.pos > B.pos) return 1;
            else if (A.pos < B.pos) return -1;
            return 0;
        });
    }
    public static void SortEffectNote(){
        
    }
}
public class Note{
    public int line;
    public int legnth;
    public float pos;
    public float ms;
    public bool isDouble;
    public GameObject noteObject;
}
public class SpeedNote{
    public float ms;
    public float bpm;
    public float pos;
    public float speed;
    public GameObject speedObject;
}
public class EffectNote{
    public float ms;
    public float duration;
    public int type;
    public GameObject effectObject;
}