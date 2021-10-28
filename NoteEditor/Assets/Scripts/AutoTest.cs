using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutoTest : MonoBehaviour
{
    private bool isPause;

    [SerializeField]
    private bool isTest;

    public float bpm;

    private float delay;

    [SerializeField]
    AudioSource Music;

    AudioSource HitSound;

    [SerializeField]
    private int testMs;
    private int[] index;

    private List<GameObject> note;
    private List<int> noteMs;
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

    private void Start()
    {
        MirrorField.SetActive(true);
        isPause = false;
        HitSound = GetComponent<AudioSource>();
        index = new int[5];
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

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HitEffect[0].SetTrigger("Play");
            Debug.Log("애니메이션 테스트");
        }
    }

    private void TestPause()
    {
        isTest = false;
        MirrorField.SetActive(true);
        MirrorField.transform.localPosition = new Vector3(0, 0, 0);
        testMs = 0;
    }

    private void ResetTest()
    {
        note = new List<GameObject>();
        noteMs = new List<int>();
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
            TestPause();
        }
    }

    public IEnumerator Test()
    {
        ResetTest();

        MirrorField.SetActive(false);
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
            targetNoteMs = (int)(note[i].transform.localPosition.y * bpm / 150);
            targetLegnth = (int)(note[i].transform.localScale.y / 100);
            noteMs.Add(targetNoteMs);
            noteMs.Add(targetLegnth);
        }
        yield return new WaitForSeconds(.5f);

        isTest = true;

        yield return new WaitForSeconds(delay);
        Music.Play();
    }
}
