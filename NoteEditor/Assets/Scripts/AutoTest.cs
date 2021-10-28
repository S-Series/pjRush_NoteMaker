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
                    HitEffect[noteLine[index] - 1].SetTrigger("Play");
                    HitSound.Play();
                    index++;
                }
            }
            catch { }
        }
    }

    private void TestStop()
    {
        isTest = false;
        MirrorField.SetActive(true);
        MirrorField.transform.localPosition = new Vector3(0, 0, 0);
        testMs = 0;
        index = 0;
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

    public IEnumerator Test()
    {
        ResetTest();

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
}
