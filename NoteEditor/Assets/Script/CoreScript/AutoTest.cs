using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoTest : MonoBehaviour
{
    public static AutoTest autoTest = new AutoTest();
    public static bool isTest = false;
    public static List<NormalNote> autoTestNotes = new List<NormalNote>();
    public static List<NormalNote> autoTestNotesView = new List<NormalNote>();
    public static List<SpeedNote> autoTestSpeedNotes = new List<SpeedNote>();
    public static List<EffectNote> autoTestEffectNotes = new List<EffectNote>();
    //*-----------------------------------------------------------------------------*//
    public AudioSource autoMusic;
    //*-----------------------------------------------------------------------------*//
    [SerializeField] private GameObject autoNoteField;
    [SerializeField] private GameObject autoViewField;
    //*-----------------------------------------------------------------------------*//
    private Button[] autoTestButton;
    //*-----------------------------------------------------------------------------*//
    private void Awake()
    {
        autoTest = this;    
    }
    private void Update()
    {
        
    }
    public void ButtonTest()
    {
        
    }
    public void ButtonTestDelay()
    {

    }
    public void Test(float startPos)
    {
        autoTestNotes = new List<NormalNote>();
        autoTestNotesView = new List<NormalNote>();
        autoTestSpeedNotes = new List<SpeedNote>();
        autoTestEffectNotes = new List<EffectNote>();
        for (int i = 0; i < autoNoteField.transform.childCount; i++)
        {
            Destroy(autoNoteField.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < autoViewField.transform.childCount; i++)
        {
            Destroy(autoViewField.transform.GetChild(i).gameObject);
        }
        //*-------------------------------------------------------------------
        for (int i = 0; i < NormalNote.normalNotes.Count; i++)
        {
            NormalNote normalNote = NormalNote.normalNotes[i];
            NormalNote autoNormalNote = new NormalNote();
            autoNormalNote.ms = normalNote.ms;
            autoNormalNote.pos = normalNote.pos;
            autoNormalNote.line = normalNote.line;
            autoNormalNote.isOver = normalNote.isOver;
            autoNormalNote.legnth = normalNote.legnth;
            autoNormalNote.noteObject = Instantiate(normalNote.noteObject, autoNoteField.transform);

            //ToDO : 마지막으로 작성하던 부분
        }
    }
}
