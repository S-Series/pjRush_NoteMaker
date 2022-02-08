using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutoTest : MonoBehaviour
{
    #region properties
    public static AutoTest autoTest;

    private bool isPause;
    private int testResetPage;

    public bool isTest;
    public bool isPlayReady;
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
    #endregion

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

        testResetPage = 1;

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
                    LongDelay = 15 / SpeedBpm[SpeedIndex];
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
                        HitEffect[noteLine[NoteIndex] - 1].SetTrigger("Rush");
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
            HitEffect[Line - 1].SetTrigger("Rush");
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
        NoteField.transform.localPosition = new Vector3(0, (testResetPage - 1) * -1600, 0);
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
        isPlayReady = false;
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
        isPlayReady = true;
        LongDelay = 15 / bpm;
        TestPlayObject[0].SetActive(false);
        TestPlayObject[1].SetActive(true);
        StartCoroutine(TestPlay.testPlay.TestLoad());
    }

    public void ButtonPlayStop()
    {
        ResetPlay();
    }

    public IEnumerator Test()
    {
        testResetPage = PageSystem.pageSystem.firstPage;

        testBpm = bpm;

        ResetTest();

        Music.time = 0.0f;

        MirrorField.SetActive(false);

        NoteField = PageSystem.pageSystem.NoteField;
        NoteField.transform.localPosition = new Vector3(0, 0, 0);

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
                float speedNum;
                speedNum = speed[i].transform.GetChild(0).GetChild(0).localPosition.x
                    * speed[i].transform.GetChild(0).GetChild(0).localPosition.y;
                SpeedPos.Add(speed[i].transform.localPosition.y);
                SpeedBpm.Add(speedNum);
            }

            // Add to List -----------------
            int newMs;
            SpeedMs.Add(0);
            newMs = (int)(speed[0].transform.localPosition.y * 150 / bpm);
            SpeedMs.Add(newMs);
            for (int i = 1; i < speed.Count; i++)
            {
                newMs += (int)(Mathf.Abs(SpeedPos[i + 1] - SpeedPos[i]) * 150 / Mathf.Abs(SpeedBpm[i]));
                SpeedMs.Add(newMs);
            }

            for (int i = 0; i < speed.Count; i++)
            {
                float speedNum;
                speedNum = SpeedBpm[i];
                if (speedNum < 0)
                {
                    try
                    {
                        SpeedPos[i] = (SpeedPos[i] + (SpeedPos[i + 1] - SpeedPos[i]) * 2);
                    }
                    catch
                    {
                        Debug.LogError("속도값이 잘못된 SpeedNote가 존재합니다");
                    }
                }
                else if (speedNum == 0)
                {
                    Debug.LogError("속도값이 0인 SpeedNote가 존재합니다.");
                }
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
            if (targetNote.tag == "chip" || targetNote.tag == "long")
            {
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
                }
            }
            else
            {
                switch (targetNote.transform.localPosition.x)
                {
                    case -100:
                        noteLine.Add(5);
                        break;

                    case +100:
                        noteLine.Add(5);
                        break;
                }
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

    public IEnumerator DelayTest()
    {
        testResetPage = PageSystem.pageSystem.firstPage;
        float nowPos;
        int delayMs = 0;

        try
        {
            nowPos = NoteField.transform.localPosition.y + 1600;
            print(-nowPos);
        }
        catch { yield break; }

        List<GameObject> DelayEffect = new List<GameObject>();
        List<GameObject> DelayNote = new List<GameObject>();
        List<float> DelayNotePos = new List<float>();
        List<int> DelayNoteMs = new List<int>();
        List<int> DelayNoteLegnth = new List<int>();
        List<int> DelayNoteLine = new List<int>();

        testBpm = bpm;

        ResetTest();

        MirrorField.SetActive(false);

        NoteField = PageSystem.pageSystem.NoteField;
        NoteField.transform.localPosition = new Vector3(0, 0, 0); 

        ActiveObject[0].SetActive(false);
        ActiveObject[1].SetActive(true);

        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            GameObject targetNote;
            targetNote = NoteField.transform.GetChild(i).gameObject;

            // Add to List -----------------
            if (targetNote.tag == "Effect") DelayEffect.Add(targetNote);
            else if (targetNote.tag == "Bpm") speed.Add(targetNote);
            else DelayNote.Add(targetNote);
        }

        DelayNote.Sort(delegate (GameObject A, GameObject B)
        {
            if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
            else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
            return 0;
        });

        DelayEffect.Sort(delegate (GameObject A, GameObject B)
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
                float speedNum;
                speedNum = speed[i].transform.GetChild(0).GetChild(0).localPosition.x
                    * speed[i].transform.GetChild(0).GetChild(0).localPosition.y;
                SpeedPos.Add(speed[i].transform.localPosition.y);
                SpeedBpm.Add(speedNum);
            }

// Add to List -----------------
            int newMs;
            SpeedMs.Add(0);
            newMs = (int)(speed[0].transform.localPosition.y * 150 / bpm);
            SpeedMs.Add(newMs);
            for (int i = 1; i < speed.Count; i++)
            {
                newMs += (int)(Mathf.Abs(SpeedPos[i + 1] - SpeedPos[i]) * 150 / Mathf.Abs(SpeedBpm[i]));
                SpeedMs.Add(newMs);
            }

// 시작 변수 설정하는곳
            #region Start Setting

            if (SpeedPos.Count >= 1)
            {
                if (-nowPos < SpeedPos[0])
                {
                    testBpm = bpm;
                    delayMs = (int)(-nowPos * 150 / testBpm);
                }
                else
                {
                    for (int i = 1; i < SpeedPos.Count; i++)
                    {
                        if (-nowPos <= SpeedPos[i])
                        {
                            testBpm = SpeedBpm[i - 1];
                            delayMs = (int)(SpeedMs[i - 1] + (-nowPos - SpeedPos[i - 1]) * 150 / testBpm);
                            print(delayMs);
                            break;
                        }
                        else {  }
                    }
                }
            }
            else 
            { 
                delayMs = (int)(-nowPos * 150 / testBpm);
                testBpm = bpm; 
            }

            if (delayMs <= 0) { StartCoroutine(Test()); yield break; }
            #endregion

            for (int i = 0; i < speed.Count; i++)
            {
                float speedNum;
                speedNum = SpeedBpm[i];
                if (speedNum < 0)
                {
                    try
                    {
                        SpeedPos[i] = (SpeedPos[i] + (SpeedPos[i + 1] - SpeedPos[i]) * 2);
                    }
                    catch
                    {
                        Debug.LogError("속도값이 잘못된 SpeedNote가 존재합니다");
                    }
                }
                else if (speedNum == 0)
                {
                    Debug.LogError("속도값이 0인 SpeedNote가 존재합니다.");
                }
            }
        }

        yield return new WaitForSeconds(.5f);


        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < DelayNote.Count; i++)
        {
            // Add to List -----------------
            DelayNotePos.Add(DelayNote[i].transform.localPosition.y);
        }

        int SpeedNoteinfoIndex;
        SpeedNoteinfoIndex = 0;

        for (int i = 0; i < DelayNote.Count; i++)
        {
            GameObject targetNote;
            targetNote = DelayNote[i];

            for (int j = SpeedNoteinfoIndex; j < SpeedMs.Count; j++)
            {
                if (SpeedPos[j] < DelayNotePos[i])
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
                posDif = DelayNotePos[i] - SpeedPos[SpeedNoteinfoIndex];
                ms = (int)(posDif * 150 / SpeedBpm[SpeedNoteinfoIndex]) + SpeedMs[SpeedNoteinfoIndex];
            }
            catch
            {
                ms = (int)(DelayNotePos[i] * 150 / bpm);
            }

            // Add to List -----------------
            DelayNoteMs.Add(ms);
            DelayNoteLegnth.Add((int)(DelayNote[i].transform.localScale.y / 100));

            // Add to List -----------------
            if (targetNote.tag == "chip" || targetNote.tag == "long")
            {
                switch (targetNote.transform.localPosition.x)
                {
                    case -300:
                        DelayNoteLine.Add(1);
                        break;

                    case -100:
                        DelayNoteLine.Add(2);
                        break;

                    case +100:
                        DelayNoteLine.Add(3);
                        break;

                    case +300:
                        DelayNoteLine.Add(4);
                        break;
                }
            }
            else
            {
                switch (targetNote.transform.localPosition.x)
                {
                    case -100:
                        DelayNoteLine.Add(5);
                        break;

                    case +100:
                        DelayNoteLine.Add(5);
                        break;
                }
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

        for (int i = 0; i < DelayNoteMs.Count; i++)
        {
            if (DelayNoteMs[i] >= delayMs)
            {
                note.Add(DelayNote[i]);
                noteMs.Add(DelayNoteMs[i]);
                notePos.Add(DelayNotePos[i]);
                noteLine.Add(DelayNoteLine[i]);
                noteLegnth.Add(DelayNoteLegnth[i]);
            }
        }

        Music.time = (delayMs - delay) / 1000;
        testMs = delayMs;
        NoteFieldParent.transform.localPosition = new Vector3(0, delayMs * testBpm / 150, 0);

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
