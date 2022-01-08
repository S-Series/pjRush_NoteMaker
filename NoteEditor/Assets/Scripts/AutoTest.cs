using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutoTest : MonoBehaviour
{
    public static AutoTest autoTest;

    private bool isPause;

    public bool isTest;
    public bool isPlay;

    public float bpm;
    public float testBpm;

    public float delay;

    [SerializeField]
    AudioSource Music;

    AudioSource HitSound;
    AudioSource LongHitSound;

    public int testMs;

    private float LongDelay;

    private List<GameObject> note;
    [SerializeField]
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
    [SerializeField]
    private List<float> SpeedPos;
    [SerializeField]
    private List<float> SpeedBpm;

    private int SpeedIndex;

    [SerializeField]
    Animator[] HitEffect;

    [SerializeField]
    GameObject NoteField;
    GameObject NoteFieldParent;

    [SerializeField]
    GameObject MirrorField;

    [SerializeField]
    TMP_InputField inputBpm;

    [SerializeField]
    TMP_InputField inputStartDelay;

    float TestSpeedPos;
    float TestSpeedMs;

    [SerializeField]
    GameObject[] TestPlayObject;

    [SerializeField]
    GameObject TestPlayParent;

    [SerializeField]
    GameObject[] ActiveObject;

    private TestPlay test;

    private void Awake()
    {
        NoteFieldParent = NoteField.transform.parent.gameObject;
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

        TestPlayObject[0].SetActive(true);
        TestPlayObject[1].SetActive(false);

        test = TestPlayObject[1].GetComponent<TestPlay>();
    }

    private void FixedUpdate()
    {
        if (isTest == true || isPlay == true)
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
                if (SpeedMs[SpeedIndex] <= testMs)
                {
                    testBpm = SpeedBpm[SpeedIndex];
                    TestSpeedMs = SpeedMs[SpeedIndex];
                    TestSpeedPos = SpeedPos[SpeedIndex];
                    SpeedIndex++;
                }
            }
            catch { }

            float posY;
            posY = TestSpeedPos + ((testMs - TestSpeedMs) * testBpm / 150);
            NoteFieldParent.transform.localPosition = new Vector3(0, -posY, 0);

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
        }
    }

    public void autoNoteFieldSet(GameObject gameObject)
    {
        NoteField = gameObject;
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
        ActiveObject[0].SetActive(true);
        ActiveObject[1].SetActive(false);
        NoteFieldParent.transform.localPosition = new Vector3(0, 0, 0);
        testMs = 0;
        EffectIndex = 0;
        SpeedIndex = 0;
        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            NoteField.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void PlayStop()
    {
        TestPlayObject[0].SetActive(true);
        TestPlayObject[1].SetActive(false);
        isPlay = false;
        MirrorField.SetActive(true);
        NoteFieldParent.transform.localPosition = new Vector3(0, 0, 0);
        testMs = 0;
        EffectIndex = 0;
        SpeedIndex = 0;
        Music.Stop();
        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            NoteField.transform.GetChild(i).gameObject.SetActive(true);
        }
        test.ResetList();
        test.ResetJudge();
    }

    private void ResetTest()
    {
        NoteField = PageSystem.pageSystem.NoteField;

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

        NoteIndex = 0;
        EffectIndex = 0;
        SpeedIndex = 0;
    }

    private void ResetPlay()
    {
        NoteField = PageSystem.pageSystem.NoteField;

        PlayStop();

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
            ResetTest();
            StopCoroutine(Test());
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

    public void ButtonPlayTest()
    {
        LongDelay = 15 / bpm;
        StartCoroutine(TestPlay());
    }

    public void ButtonPlayStop()
    {
        ResetPlay();
    }

    public IEnumerator Test()
    {
        testBpm = bpm;

        ResetTest();

        Music.time = 0.0f;

        MirrorField.SetActive(false);

        NoteField = PageSystem.pageSystem.NoteField;

        ActiveObject[0].SetActive(false);
        ActiveObject[1].SetActive(true);

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
            /// <summary>
            /// 
            /// 인자값은 var로 선언은 되지만, 적용은 되지 않는다
            /// GameObject test = new GameObject();
            ///
            /// var GetPos = transform.localPosition;
            ///
            /// float testPosA;
            /// testPosA = test.GetPos.y;
            /// testPosA = test.transform.localPosition.y;
            /// 
            /// </summary>

            // Add to List -----------------
            SpeedPos.Add(0);
            SpeedBpm.Add(bpm);
            for (int i = 0; i < speed.Count; i++)
            {
                SpeedPos.Add(speed[i].transform.localPosition.y);
                SpeedBpm.Add(speed[i].transform.GetChild(0).GetChild(0).transform.localPosition.y);
            }

            // Add to List -----------------
            int newMs;
            SpeedMs.Add(0);
            newMs = (int)(speed[0].transform.localPosition.y * 150 / bpm);
            SpeedMs.Add(newMs);
            for (int i = 1; i < speed.Count; i++)
            {
                newMs += (int)((SpeedPos[i + 1] - SpeedPos[i]) * 150 / SpeedBpm[i]);
                SpeedMs.Add(newMs);
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

            for (int j = SpeedNoteinfoIndex; j < SpeedMs.Count; j++)
            {
                if (SpeedPos[j] < notePos[i])
                {
                    SpeedNoteinfoIndex = j;
                }
                else
                {
                    break;
                }
            }

            int ms;
            try
            {
                float posDif;
                posDif = notePos[i] - SpeedPos[SpeedNoteinfoIndex];
                ms = (int)(posDif * 150 / SpeedBpm[SpeedNoteinfoIndex]) + SpeedMs[SpeedNoteinfoIndex];
            }
            catch
            {
                ms = (int)(notePos[i] * 150 / bpm);
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

            for (int j = SpeedNoteinfoIndex; j < SpeedMs.Count - 1; j++)
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

    public IEnumerator TestPlay()
    {

        for (int i = 0; i < test.MovingNoteField.transform.childCount; i++)
        {
            Destroy(test.MovingNoteField.transform.GetChild(i).gameObject);
        }

        SaveLoad.saveLoad.ButtonSave();

        TestPlayObject[0].SetActive(false);
        TestPlayObject[1].SetActive(true);

        test.ResetList();
        test.ResetJudge();

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

        for (int i = 0; i < noteMs.Count; i++)
        {
            GameObject clone;
            clone = Instantiate(note[i], test.MovingNoteField.transform);
            Vector3 pos;
            pos = note[i].transform.localPosition;
            clone.transform.localPosition = new Vector3(pos.x, pos.y, 0);
            clone.transform.localScale = note[i].transform.localScale;
            clone.transform.localRotation = Quaternion.Euler(0, 0, 0);

            switch (noteLine[i])
            {
                case 1:
                    var play1 = test.judge1;
                    play1.TestPlay1.Add(clone);
                    play1.TestPlayMs1.Add(noteMs[i]);
                    play1.TestPlayLegnth1.Add(noteLegnth[i]);
                    break;

                case 2:
                    var play2 = test.judge2;
                    play2.TestPlay2.Add(clone);
                    play2.TestPlayMs2.Add(noteMs[i]);
                    play2.TestPlayLegnth2.Add(noteLegnth[i]);
                    break;

                case 3:
                    var play3 = test.judge3;
                    play3.TestPlay3.Add(clone);
                    play3.TestPlayMs3.Add(noteMs[i]);
                    play3.TestPlayLegnth3.Add(noteLegnth[i]);
                    break;

                case 4:
                    var play4 = test.judge4;
                    play4.TestPlay4.Add(clone);
                    play4.TestPlayMs4.Add(noteMs[i]);
                    play4.TestPlayLegnth4.Add(noteLegnth[i]);
                    break;

                case 5:
                    var play5 = test.judgeBottom;
                    play5.TestPlay5.Add(clone);
                    play5.TestPlayMs5.Add(noteMs[i]);
                    play5.TestPlayLegnth5.Add(noteLegnth[i]);
                    break;
            }
        }

        test.testEffectMs = EffectMs;
        test.testEffectForce = EffectForce;
        test.testEffectDuration = EffectDuration;

        test.testSpeedMs = SpeedMs;
        test.testSpeedBpm = SpeedBpm;

        yield return new WaitForSeconds(.5f);

        test.getInfo();

        yield return new WaitForSeconds(.5f);
        
        isPlay = true;

        yield return new WaitForSeconds(delay / 1000);
        Music.Play();
    }

    public IEnumerator DelayTest()
    {
        List<GameObject> DelayEffect = new List<GameObject>();
        List<GameObject> DelayNote = new List<GameObject>();

        testBpm = bpm;

        ResetTest();

        MirrorField.SetActive(false);
        ActiveObject[0].SetActive(false);
        ActiveObject[1].SetActive(true);

        NoteField = PageSystem.pageSystem.NoteField;

        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            GameObject targetNote;
            targetNote = NoteField.transform.GetChild(i).gameObject;

            // Add to List -----------------
            if (targetNote.tag == "Effect") DelayEffect.Add(targetNote);
            else if (targetNote.tag == "Bpm") speed.Add(targetNote);
            else DelayNote.Add(targetNote);
        }

        speed.Sort(delegate (GameObject A, GameObject B)
        {
            if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
            else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
            return 0;
        });

        if (speed.Count >= 1)
        {
            /// <summary>
            /// 
            /// 인자값은 var로 선언은 되지만, 적용은 되지 않는다
            /// GameObject test = new GameObject();
            ///
            /// var GetPos = transform.localPosition;
            ///
            /// float testPosA;
            /// testPosA = test.GetPos.y;
            /// testPosA = test.transform.localPosition.y;
            /// 
            /// </summary>

            // Add to List -----------------
            SpeedPos.Add(0);
            SpeedBpm.Add(bpm);
            for (int i = 0; i < speed.Count; i++)
            {
                SpeedPos.Add(speed[i].transform.localPosition.y);
                SpeedBpm.Add(speed[i].transform.GetChild(0).GetChild(0).transform.localPosition.y);
            }

            // Add to List -----------------
            int newMs;
            SpeedMs.Add(0);
            newMs = (int)(speed[0].transform.localPosition.y * 150 / bpm);
            SpeedMs.Add(newMs);
            for (int i = 1; i < speed.Count; i++)
            {
                newMs += (int)((SpeedPos[i + 1] - SpeedPos[i]) * 150 / SpeedBpm[i]);
                SpeedMs.Add(newMs);
            }
        }

        int startPage = PageSystem.pageSystem.firstPage;
        float delayStartPos = startPage * 1600;

        yield return new WaitForSeconds(.5f);

        int startMs = 0;
        float startPos = 0.0f;
        float startBpm = bpm;

        if (speed.Count >= 1)
        {
            for (int i = 0; i < SpeedMs.Count; i++)
            {
                if (SpeedPos[i] < delayStartPos)
                {
                    startMs = SpeedMs[i];
                    startPos = SpeedPos[i];
                    startBpm = SpeedBpm[i];
                }
                else { break; }
            }
        }

        print(startMs);
        print(startBpm);
        print(((startPage - 1) * 1600 - startPos) * 150 / startBpm);
        startMs += (int)(((startPage - 1) * 1600 - startPos) * 150 / startBpm);
        testMs = startMs;
        print(testMs);
        testBpm = startBpm;
        if (testMs - delay <= 1000)
        {
            StartCoroutine(Test());
            yield break;
        }
        Music.time = (testMs - delay) / 1000;

        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < DelayNote.Count; i++)
        {
            if (DelayNote[i].transform.localPosition.y >= delayStartPos) 
            { 
                note.Add(DelayNote[i]); 
            }
        }

        for (int i = 0; i < DelayEffect.Count; i++)
        {
            if (DelayEffect[i].transform.localPosition.y >= delayStartPos) 
            { 
                effect.Add(DelayEffect[i]); 
            }
        }

        note.Sort(delegate (GameObject A, GameObject B)
        {
            if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
            else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
            return 0;
        });

        yield return new WaitForSeconds(.5f);

        effect.Sort(delegate (GameObject A, GameObject B)
        {
            if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
            else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
            return 0;
        });

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

            for (int j = SpeedNoteinfoIndex; j < SpeedMs.Count; j++)
            {
                if (SpeedPos[j] < notePos[i])
                {
                    SpeedNoteinfoIndex = j;
                }
                else{ break; }
            }

            int ms;
            try
            {
                float posDif;
                posDif = notePos[i] - SpeedPos[SpeedNoteinfoIndex];
                ms = (int)(posDif * 150 / SpeedBpm[SpeedNoteinfoIndex]) + SpeedMs[SpeedNoteinfoIndex];
            }
            catch
            {
                ms = (int)(notePos[i] * 150 / bpm);
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

            for (int j = SpeedNoteinfoIndex; j < SpeedMs.Count - 1; j++)
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
        Music.Play();
    }

    /*public IEnumerator TestA()
    {
        var wait = new WaitForSeconds(2.0f);

        for (int i = 0; i < 10; i++)
        {
            yield return wait;
        }
    }*/
}
