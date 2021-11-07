﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutoTest : MonoBehaviour
{
    public static AutoTest autoTest;

    private bool isPause;

    [SerializeField]
    private bool isTest;

    public float bpm;

    public float delay;

    [SerializeField]
    AudioSource Music;

    AudioSource HitSound;
    AudioSource LongHitSound;

    [SerializeField]
    private int testMs;
    private int bpmIndex;

    private float LongDelay;

    private List<GameObject> note;
    private List<int> noteMs;
    private List<float> notePos;
    private List<int> noteLine;
    private List<int> noteLegnth;

    [SerializeField]
    private int NoteIndex;

    private List<GameObject> effect;
    private List<int> EffectMs;
    private List<float> EffectPos;
    private List<float> EffectForce;
    private List<int> EffectDuration;

    private int EffectIndex;

    private List<GameObject> speed;
    [SerializeField]
    private List<int> SpeedMs;
    private List<float> SpeedPos;
    [SerializeField]
    private List<float> SpeedBpm;
    private List<int> SpeedRetouch;

    private int SpeedIndex;

    [SerializeField]
    Animator[] HitEffect;

    [SerializeField]
    GameObject NoteField;

    [SerializeField]
    GameObject MirrorField;

    [SerializeField]
    TMP_InputField inputBpm;

    [SerializeField]
    TMP_InputField inputStartDelay;

    float TestSpeedPos;
    float TestSpeedMs;

    private void Awake()
    {
        autoTest = this;
    }

    private void Start()
    {
        MirrorField.SetActive(true);
        isPause = false;
        HitSound = GetComponent<AudioSource>();
        LongHitSound = transform.GetChild(0).GetComponent<AudioSource>();
        ResetTest();

        bpm = 120;
        inputBpm.text = "120";
        delay = 0;
        inputStartDelay.text = "0";

        NoteIndex = 0;
        EffectIndex = 0;
        SpeedIndex = 0;

        LongDelay = 0.125f;

        TestSpeedPos = 0;
        TestSpeedMs = 0;
    }

    private void FixedUpdate()
    {
        if (isTest == true)
        {
            testMs++;
        }
    }

    private void Update()
    {
        if (isTest == true)
        {
            try
            {
                if (SpeedMs[bpmIndex] <= testMs)
                {
                    bpm = SpeedBpm[bpmIndex];
                    bpmIndex++;
                }
            }
            catch { }

            float posY;
            posY = TestSpeedPos + ((testMs - TestSpeedMs) * bpm / 150);
            NoteField.transform.localPosition = new Vector3(0, -posY, 0);

            // ----------------------------------------------------------------------------
            try
            {
                if (noteMs[NoteIndex] <= testMs)
                {
                    if (noteLegnth[NoteIndex] < 2)
                    {
                        HitEffect[noteLine[NoteIndex] - 1].SetTrigger("Play");
                        HitSound.Play();
                    }
                    else
                    {
                        StartCoroutine(LongStart(noteLine[NoteIndex], noteLegnth[NoteIndex]));
                    }
                    NoteIndex++;
                }
            }
            catch { }
            // ----------------------------------------------------------------------------
            try
            {
                if (EffectMs[EffectIndex] <= testMs)
                {

                }
            }
            catch { }
            // ----------------------------------------------------------------------------
            try
            {
                if (SpeedMs[SpeedIndex] <= testMs)
                {
                    bpm = SpeedBpm[SpeedIndex];
                    TestSpeedMs = SpeedMs[SpeedIndex];
                    TestSpeedPos = SpeedPos[SpeedIndex];
                }
                SpeedIndex++;
            }
            catch { }
        }
    }

    private IEnumerator LongStart(int Line, int Legnth)
    {
        var delay = new WaitForSeconds(LongDelay);

        for (int i = 0; i < Legnth; i++)
        {
            HitEffect[Line - 1].SetTrigger("Play");
            LongHitSound.Play();
            yield return delay;

            if (isTest == false) break;
        }
    }

    private void TestStop()
    {
        isTest = false;
        MirrorField.SetActive(true);
        NoteField.transform.localPosition = new Vector3(0, 0, 0);
        testMs = 0;
        bpmIndex = 0;
        EffectIndex = 0;
        SpeedIndex = 0;
        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            NoteField.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void ResetTest()
    {
        TestStop();

        note = new List<GameObject>();
        noteMs = new List<int>();
        notePos = new List<float>();
        noteLine = new List<int>();
        noteLegnth = new List<int>();

        effect = new List<GameObject>();
        EffectMs = new List<int>();
        EffectForce = new List<float>();
        EffectDuration = new List<int>();

        speed = new List<GameObject>();
        SpeedMs = new List<int>();
        SpeedPos = new List<float>();
        SpeedBpm = new List<float>();
        SpeedRetouch = new List<int>();

        NoteIndex = 0;
        EffectIndex = 0;
        SpeedIndex = 0;
    }

    public void ButtonBpm()
    {
        try
        {
            bpm = Convert.ToSingle(inputBpm.text);
            if (bpm < 0)
            {
                bpm = 120;
                inputBpm.text = "120";
            }
        }
        catch { return; }
    }

    public void ButtonDelay()
    {
        try
        {
            delay = Convert.ToSingle(inputStartDelay.text);
        }
        catch { return; }
    }

    public void ButtonTest()
    {
        LongDelay = 15 / bpm;

        if (isPause == false)
        {
            isPause = true;
            StartCoroutine(Test());
        }
        else
        {
            isPause = false;
            Music.Stop();
            StopCoroutine(Test());
            ResetTest();
        }
    }

    public void ButtonDelayTest()
    {
        LongDelay = 15 / bpm;

        if (isPause == false)
        {
            isPause = true;
            StartCoroutine(DelayTest());
        }
        else
        {
            isPause = false;
            Music.Stop();
            StopCoroutine(DelayTest());
            ResetTest();
        }
    }

    public IEnumerator Test()
    {
        ResetTest();

        Music.time = 0.0f;

        MirrorField.SetActive(false);

        NoteField = PageSystem.pageSystem.NoteField;

        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            GameObject targetNote;
            targetNote = NoteField.transform.GetChild(i).gameObject;

            // Add to List -----------------
            if (targetNote.tag == "Effect") effect.Add(targetNote);
            else if (targetNote.tag == "Bpm") speed.Add(targetNote);
            else note.Add(targetNote);
        }

        note.Sort(delegate (GameObject A, GameObject B)
        {
            if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
            else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
            return 0;
        });

        effect.Sort(delegate (GameObject A, GameObject B)
        {
            if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
            else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
            return 0;
        });

        speed.Sort(delegate (GameObject A, GameObject B)
        {
            if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
            else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
            return 0;
        });

        if (speed.Count >= 1)
        {
            // for int i = 0 start
            int speedRetouchMs;

            // Add to List -----------------
            SpeedPos.Add(speed[0].transform.localPosition.y);

            int onceMs;
            onceMs = (int)(speed[0].transform.localPosition.y * 150 / bpm);
            speedRetouchMs = onceMs;

            // Add to List -----------------
            SpeedMs.Add(onceMs);
            SpeedBpm.Add(speed[0].transform.GetChild(0).GetChild(0).transform.localPosition.y);
            SpeedRetouch.Add(onceMs);
            // for int i = 0 end

            for (int i = 1; i < speed.Count; i++)
            {
                GameObject childObject;
                childObject = speed[i].transform.GetChild(0).GetChild(0).gameObject;

                // Add to List -----------------
                SpeedPos.Add(speed[i].transform.localPosition.y);

                float targetSpeedBpm;
                targetSpeedBpm = childObject.transform.localPosition.y;

                float posY;
                posY = SpeedPos[i] - SpeedPos[i - 1];
                int ms;
                ms = (int)(posY * 150 / targetSpeedBpm) + speedRetouchMs;
                speedRetouchMs = ms;

                // Add to List -----------------
                SpeedMs.Add(ms);
                SpeedBpm.Add(targetSpeedBpm);
                SpeedRetouch.Add(ms);
            }
        }

        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < note.Count; i++)
        {
            // Add to List -----------------
            notePos.Add(note[i].transform.localPosition.y);
        }

        int SpeedNoteinfoIndex;
        SpeedNoteinfoIndex = 0;
        for (int i = 0; i < note.Count; i++)
        {
            GameObject targetNote;
            targetNote = note[i];

            float notePosY;
            notePosY = notePos[i];

            for (int j = SpeedNoteinfoIndex; j < SpeedMs.Count; j++)
            {
                if (SpeedPos[j] < notePosY)
                {
                    SpeedNoteinfoIndex = j;
                }
                else { break; }
            }

            int ms;
            try
            {
                float posDif;
                posDif = notePosY - SpeedPos[SpeedNoteinfoIndex];
                ms = (int)(posDif * 150 / SpeedBpm[SpeedNoteinfoIndex]) + SpeedMs[SpeedNoteinfoIndex];
            }
            catch
            {
                ms = (int)(notePosY * 150 / bpm);
            }

            // Add to List -----------------
            noteMs.Add(ms);
            noteLegnth.Add((int)(note[i].transform.localScale.y / 100));

            // Add to List -----------------
            switch (targetNote.transform.localPosition.x)
            {
                case -300:
                    noteLine.Add(1);
                    break;

                case -100:
                    noteLine.Add(2);
                    break;

                case +100:
                    noteLine.Add(3);
                    break;

                case +300:
                    noteLine.Add(4);
                    break;

                case 0:
                    noteLine.Add(5);
                    break;
            }
        }

        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < effect.Count; i++)
        {
            // Add to List -----------------
            EffectPos.Add(effect[i].transform.localPosition.y);
        }

        SpeedNoteinfoIndex = 0;
        for (int i = 0; i < effect.Count; i++)
        {
            GameObject targetObject;
            targetObject = effect[i];

            GameObject childObject;
            childObject = targetObject.transform.GetChild(0).GetChild(0).gameObject;

            float EffectPosY;
            EffectPosY = EffectPos[i];

            for (int j = SpeedNoteinfoIndex; j < SpeedMs.Count; j++)
            {
                if (SpeedPos[j] < EffectPosY)
                {
                    SpeedNoteinfoIndex = j;
                }
                else { break; }
            }

            float posDif;
            posDif = EffectPosY - SpeedPos[SpeedNoteinfoIndex];

            int ms;
            ms = (int)(posDif * 150 / SpeedBpm[SpeedNoteinfoIndex]) + SpeedMs[SpeedNoteinfoIndex];

            // Add to List -----------------
            EffectMs.Add(ms);
            EffectForce.Add(childObject.transform.localPosition.x);
            EffectDuration.Add((int)(childObject.transform.localScale.y));
        }

        yield return new WaitForSeconds(.5f);

        isTest = true;

        yield return new WaitForSeconds(delay / 1000);
        Music.Play();
    }


    public IEnumerator DelayTest()
    {
        ResetTest();

        int startPage;
        float start;
        startPage = PageSystem.pageSystem.firstPage - 1;
        if (startPage < 1) startPage = 1;
        start = startPage * 240 / bpm;

        Music.time = start;

        testMs = (int)(startPage * 1600 * 150 / bpm);

        MirrorField.SetActive(false);

        NoteField = PageSystem.pageSystem.NoteField;

        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            GameObject targetNote;
            targetNote = NoteField.transform.GetChild(i).gameObject;
            if (targetNote.transform.localPosition.y >= (startPage + 1) * 1600) note.Add(targetNote);
            else targetNote.SetActive(false);
        }

        note.Sort(delegate (GameObject A, GameObject B)
        {
            if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
            else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
            return 0;
        });

        for (int i = 0; i < note.Count; i++)
        {
            int targetNoteMs;
            int targetLegnth;
            targetNoteMs = (int)(note[i].transform.localPosition.y * 150 / bpm);
            targetLegnth = (int)(note[i].transform.localScale.y / 100);
            noteMs.Add(targetNoteMs);
            noteLegnth.Add(targetLegnth);

            switch (note[i].transform.localPosition.x)
            {
                case -300:
                    noteLine.Add(1);
                    break;

                case -100:
                    noteLine.Add(2);
                    break;

                case +100:
                    noteLine.Add(3);
                    break;

                case +300:
                    noteLine.Add(4);
                    break;

                case 0:
                    noteLine.Add(5);
                    break;
            }

        }
        yield return new WaitForSeconds(.5f);

        isTest = true;

        yield return new WaitForSeconds(delay / 1000);
        Music.Play();
    }
}
