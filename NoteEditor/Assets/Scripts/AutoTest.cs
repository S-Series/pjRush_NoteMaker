using System;
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

    [SerializeField]
    private int testMs;
    private int index;

    private float LongDelay;

    private List<GameObject> note;
    private List<int> noteMs;
    private List<int> noteLine;
    private List<int> noteLegnth;

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

    private void Awake()
    {
        autoTest = this;
    }

    private void Start()
    {
        MirrorField.SetActive(true);
        isPause = false;
        HitSound = GetComponent<AudioSource>();
        ResetTest();

        bpm = 120;
        inputBpm.text = "120";
        delay = 0;
        inputStartDelay.text = "0";

        LongDelay = 0.125f;
    }

    private void FixedUpdate()
    {
        if (isTest == true)
        {
            testMs++;
            float posY;
            posY = bpm * testMs / 150;
            NoteField.transform.localPosition = new Vector3(0, -posY, 0);
        }
    }

    private void Update()
    {
        if (isTest == true)
        {
            try
            {
                if (noteMs[index] <= testMs)
                {
                    if (noteLegnth[index] < 2)
                    {
                        HitEffect[noteLine[index] - 1].SetTrigger("Play");
                        HitSound.Play();
                    }
                    else
                    {
                        StartCoroutine(LongStart(noteLine[index], noteLegnth[index]));
                    }
                    index++;
                }
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
            HitSound.Play();
            yield return delay;
        }
    }

    private void TestStop()
    {
        isTest = false;
        MirrorField.SetActive(true);
        NoteField.transform.localPosition = new Vector3(0, 0, 0);
        testMs = 0;
        index = 0;
        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            NoteField.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void ResetTest()
    {
        note = new List<GameObject>();
        noteMs = new List<int>();
        noteLine = new List<int>();
        noteLegnth = new List<int>();
    }

    public void ButtonBpm()
    {
        try
        {
            bpm = Convert.ToSingle(inputBpm.text);
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
            TestStop();
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
            TestStop();
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
            note.Add(targetNote);
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
