using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoTest : MonoBehaviour
{
    //*Static -----------------------------------------------*//
    public static AutoTest autoTest = new AutoTest();
    public static bool isTest = false;
    public static List<NormalNote> autoTestNotes = new List<NormalNote>();
    public static List<NormalNote> autoTestNotesView = new List<NormalNote>();
    public static List<SpeedNote> autoTestSpeedNotes = new List<SpeedNote>();
    public static List<EffectNote> autoTestEffectNotes = new List<EffectNote>();
    
    //*Public -----------------------------------------------*//
    public AudioSource autoMusic;
    
    //*SerializeField -----------------------------------------------*//
    [SerializeField] private GameObject autoNoteField;
    [SerializeField] private GameObject autoViewField;
    
    //*Private -----------------------------------------------*//
    private Button[] autoTestButton;
    
    //*Private -----------------------------------------------*//
    private void Awake()
    {
        autoTest = this;    
    }
    private void Update()
    {
        
    }
    private void Test(float startPos)
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
    //*Public -----------------------------------------------*//

    //*IEnumerator -----------------------------------------------*//
    private IEnumerator StartTest(float startPos)
    {
        yield return CalculateNoteMs();
        Test(startPos);
    }
    private IEnumerator CalculateNoteMs()
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
            editSpeed.ms = editMs + (editSpeed.pos - editPos) * 150 / editBpm;

            editMs = editSpeed.ms;
            editPos = editSpeed.pos;
            editBpm = editSpeed.bpm * editSpeed.multiply;

            yield return null;
        }

        //* NormalNote ---------- //
        speedIndex = SpeedNote.speedNotes.Count - 1;
        editMs = 0.0f;
        editPos = 0.0f;
        editBpm = ValueManager.bpm;
        for (int i = 0; i < NormalNote.normalNotes.Count; i++)
        {
            editNormal = NormalNote.normalNotes[i];
            for (int j = speedIndex; j > -1; j--)
            {
                if (SpeedNote.speedNotes[j].pos >= editNormal.pos)
                {
                    editMs = SpeedNote.speedNotes[j].ms;
                    editPos = SpeedNote.speedNotes[j].pos;
                    editBpm = SpeedNote.speedNotes[j].bpm * SpeedNote.speedNotes[j].multiply;
                    break;
                }
                if (j == 0)
                {
                    editMs = 0.0f;
                    editPos = 0.0f;
                    editBpm = ValueManager.bpm;
                }
            }
            editNormal.ms = editMs + (editNormal.pos - editPos) * 150 / editBpm;
        }

        //* EffectNote ---------- //
        speedIndex = SpeedNote.speedNotes.Count - 1;
        editMs = 0.0f;
        editPos = 0.0f;
        editBpm = ValueManager.bpm;
        for (int i = 0; i < EffectNote.effectNotes.Count; i++)
        {
            editEffect = EffectNote.effectNotes[i];
            for (int j = speedIndex; j > -1; j--)
            {
                if (SpeedNote.speedNotes[j].pos >= editEffect.pos)
                {
                    editMs = SpeedNote.speedNotes[j].ms;
                    editPos = SpeedNote.speedNotes[j].pos;
                    editBpm = SpeedNote.speedNotes[j].bpm * SpeedNote.speedNotes[j].multiply;
                    break;
                }
                if (j == 0)
                {
                    editMs = 0.0f;
                    editPos = 0.0f;
                    editBpm = ValueManager.bpm;
                }
            }
            editEffect.ms = editMs + (editEffect.pos - editPos) * 150 / editBpm;
        }
        editEffect = null;
        editMs = 0;
        editBpm = ValueManager.bpm;
        for (int i = 0; i < EffectNote.effectNotes.Count; i++)
        {
            editEffect = EffectNote.effectNotes[i];
            for (int j = speedIndex; j > -1; j--)
            {
                if (SpeedNote.speedNotes[j].pos >= editEffect.pos)
                {
                    editMs = SpeedNote.speedNotes[j].ms;
                    editBpm = SpeedNote.speedNotes[j].bpm * SpeedNote.speedNotes[j].multiply;
                    break;
                }
                if (j == 0)
                {
                    editMs = 0.0f;
                    editPos = 0.0f;
                    editBpm = ValueManager.bpm;
                }
            }
            editMs = editEffect.value * 150 / editBpm;

            foreach (NormalNote _normalNote in NormalNote.normalNotes)
            {
                if ()
            }
        }
    }
    private IEnumerator StartingTest(float musicDelay)
    {
        isTest = true;
        autoMusic.time = musicDelay;

        yield return new WaitForSeconds(ValueManager.delay);

        autoMusic.Play();
    }

    //*Buttons -----------------------------------------------*//
    public void ButtonTest()
    {
        if (!isTest)
        {
            StartCoroutine(StartTest(0.0f));
        }
        else
        {

        }
    }
    public void ButtonTestDelay()
    {
        if (!isTest)
        {
            StartCoroutine(StartTest(0.0f));
        }
        else
        {

        }
    }
}
