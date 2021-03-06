using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoTest : MonoBehaviour
{
    private const string AnimateTrigger = "Play";

    //*Static -----------------------------------------------*//
    public static AutoTest autoTest;
    public static float s_testMs = 0;
    public static float s_testBpm = 120.0f;
    public static bool s_isTest = false;
    public static List<NormalNote> autoTestNormalNotes = new List<NormalNote>();
    public static List<SpeedNote> autoTestSpeedNotes = new List<SpeedNote>();
    public static List<EffectNote> autoTestEffectNotes = new List<EffectNote>();
    private List<float> autoTestEffectBpm = new List<float>();
    
    //*Public -----------------------------------------------*//
    public AudioSource autoMusic;
    
    //*SerializeField -----------------------------------------------*//
    [SerializeField] private GameObject[] autoNoteField;
    [SerializeField] private GameObject[] autoTestViewObject;
    [SerializeField] private Transform[] MovingField;
    [SerializeField] private Animator[] autoTestAnimator;
    [SerializeField] private Animator[] autoTestPreviewAnimator;
    [SerializeField] private AudioSource[] autoTestJudgeSound;
    
    //*Private -----------------------------------------------*//
    private Vector3 MovingPos = new Vector3(0, 0, 0);
    private Button[] autoTestButton;
    private int testStartPage = 1;
    private int[] testIndex = new int[3]{0, 0, 0};
    private bool[] isTesting = new bool[3]{true, true, true};
    private bool isOnEffect = false;
    private float autoTestMultiply = 1.0f;
    private float SpeedMs = 0.0f;
    private float SpeedPos = 0.0f;
    private float EffectMs = 0.0f;
    private float EffectPos = 0.0f;
    private float autoTestEffectPos = 0.0f;
    private NormalNote autoTestNormal;
    private SpeedNote autoTestSpeed;
    private EffectNote autoTestEffect;
    [SerializeField] private GameObject[] testLoadingBlock;
    
    //*Private -----------------------------------------------*//
    private void Awake()
    {
        autoTest = this;
    }
    private void FixedUpdate()
    {
        if (!s_isTest) return;
        s_testMs+=autoTestMultiply;
    }
    private void Update()
    {
        if (!s_isTest) return;
        //* NormalNote
        if (isTesting[0])
        {
            if (autoTestNormal.ms <= s_testMs)
            {
                if (autoTestNormal == null) {autoTestNormal = autoTestNormalNotes[testIndex[0]];}

                if (autoTestNormal.legnth == 0) {autoJudgeEffect(autoTestNormal.line);}
                else {StartCoroutine(LongNoteEffect(autoTestNormal.line, autoTestNormal.legnth));}

                testIndex[0]++;
                ScoreManager.scoreManager.AutoTestComboAdd();
                if (testIndex[0] >= autoTestNormalNotes.Count) {isTesting[0] = false;}
                else {autoTestNormal = autoTestNormalNotes[testIndex[0]];}
            }
        }
        //* SppedNote
        if (isTesting[1])
        {
            if (autoTestSpeed.ms <= s_testMs)
            {
                if (autoTestSpeed == null) {autoTestSpeed = autoTestSpeedNotes[testIndex[1]];}

                s_testBpm = autoTestSpeed.bpm * autoTestSpeed.multiply;
                SpeedMs = autoTestSpeed.ms;
                SpeedPos = autoTestSpeed.pos;
                EffectMs = 0;
                EffectPos = 0;

                testIndex[1]++;
                if (testIndex[1] >= autoTestSpeedNotes.Count) {isTesting[1] = false;}
                else {autoTestSpeed = autoTestSpeedNotes[testIndex[1]];}
            }
        }
        //* EffectNote
        if (isTesting[2])
        {
            print("testing");
            if (autoTestEffect.ms <= s_testMs)
            {
                print("test Run");
                if (autoTestEffect == null) {autoTestEffect = autoTestEffectNotes[testIndex[2]];}

                StartCoroutine(EffectNoteStart(autoTestEffect.pos, autoTestEffect.ms, autoTestEffect.value, autoTestEffectBpm[testIndex[2]], autoTestEffect.isPause));

                testIndex[2]++;
                if (testIndex[2] >= autoTestEffectNotes.Count) {isTesting[2] = false;}
                else {autoTestEffect = autoTestEffectNotes[testIndex[2]];}
            }
        }

        if (isOnEffect) {MovingPos.y = autoTestEffectPos;}
        else {MovingPos.y = EffectPos + SpeedPos + ((s_testMs - SpeedMs - EffectMs) * s_testBpm) / 150;}

        MovingField[0].localPosition = - MovingPos;
        MovingField[1].localPosition = -3.0f * MovingPos * 4;
    }
    private void Test(float startPos)
    {
        s_testBpm = ValueManager.bpm;
        autoTestViewObject[0].SetActive(true);
        autoTestViewObject[1].SetActive(false);
        autoTestNormalNotes = new List<NormalNote>();
        autoTestSpeedNotes = new List<SpeedNote>();
        autoTestEffectNotes = new List<EffectNote>();
        autoTestEffectBpm = new List<float>();
        for (int i = 0; i < autoNoteField[0].transform.childCount; i++)
        {
            Destroy(autoNoteField[0].transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < autoNoteField[1].transform.childCount; i++)
        {
            Destroy(autoNoteField[1].transform.GetChild(i).gameObject);
        }
        //*-------------------------------------------------------------------
        for (int i = 0; i < NormalNote.normalNotes.Count; i++)
        {
            NormalNote normalNote = NormalNote.normalNotes[i];
            NormalNote autoNormalNote = new NormalNote();
            autoNormalNote.ms = normalNote.ms;
            autoNormalNote.pos = normalNote.pos;
            autoNormalNote.line = normalNote.line;
            autoNormalNote.isPowered = normalNote.isPowered;
            autoNormalNote.legnth = normalNote.legnth;
            if (autoNormalNote.isRight)
                { autoNormalNote.noteObject = Instantiate(normalNote.noteObject, autoNoteField[1].transform); }
            else { autoNormalNote.noteObject = Instantiate(normalNote.noteObject, autoNoteField[0].transform); }
            autoNormalNote.noteObject.GetComponent<BoxCollider2D>().enabled = false;
            Vector3 pos = autoNormalNote.noteObject.transform.localPosition;
            pos.y *= 4;
            autoNormalNote.noteObject.transform.localPosition = pos;
            if (autoNormalNote.legnth != 0)
            {
                Vector3 scale = autoNormalNote.noteObject.transform.localScale;
                scale.y *= 4;
                autoNormalNote.noteObject.transform.localScale = scale;
            }
            autoTestNormalNotes.Add(autoNormalNote);
        }
        for (int i = 0; i < SpeedNote.speedNotes.Count; i++)
        {
            SpeedNote speedNote = SpeedNote.speedNotes[i];
            SpeedNote autoSpeedNote = new SpeedNote();
            autoSpeedNote.ms = speedNote.ms;
            autoSpeedNote.pos = speedNote.pos;
            autoSpeedNote.bpm = speedNote.bpm;
            autoSpeedNote.multiply = speedNote.multiply;
            autoSpeedNote.noteObject = null;
            autoTestSpeedNotes.Add(autoSpeedNote);
        }
        for (int i = 0; i < EffectNote.effectNotes.Count; i++)
        {
            EffectNote effectNote = EffectNote.effectNotes[i];
            EffectNote autoEffectNote = new EffectNote();
            autoEffectNote.ms = effectNote.ms;
            autoEffectNote.pos = effectNote.pos;
            autoEffectNote.isPause = effectNote.isPause;
            autoEffectNote.value = effectNote.value;
            autoEffectNote.noteObject = null;
            autoTestEffectNotes.Add(autoEffectNote);

            float effectBpm = ValueManager.bpm;
            for (int j = autoTestSpeedNotes.Count - 1; j >= 0; j--)
            {
                if (autoTestSpeedNotes[j].pos <= autoEffectNote.pos)
                {
                    effectBpm = autoTestSpeedNotes[j].bpm * autoTestSpeedNotes[j].multiply;
                    break;
                }
            }
            autoTestEffectBpm.Add(effectBpm);
        }

        testIndex = new int[3]{0, 0, 0};
        float delayBpm = ValueManager.bpm;
        float delayMs = 150 * startPos / delayBpm;
        if (!Mathf.Approximately(startPos, 0.0f))
        {
            for (int i = autoTestEffectNotes.Count - 1; i > -1; i--)
            {
                if (autoTestEffectNotes[i].pos <= startPos)
                {
                    EffectNote retouchEffect = autoTestEffectNotes[i];
                    if (retouchEffect.pos + retouchEffect.value >= startPos)
                    {
                        startPos = (Mathf.FloorToInt(retouchEffect.pos / 1600.0f) - 1) * 1600.0f;
                        if (startPos < 0.0f) { startPos = 0.0f; }
                    }
                    break;
                }
            }
        }
        if (!Mathf.Approximately(startPos, 0.0f))
        {
            for (int i = 0; i < autoTestNormalNotes.Count; i++)
            {
                if (autoTestNormalNotes[i].pos >= startPos)
                {
                    testIndex[0] = i;
                    break;
                }
            }
            for (int i = autoTestSpeedNotes.Count - 1; i > -1; i--)
            {
                if (autoTestSpeedNotes[i].pos <= startPos)
                {
                    SpeedNote retouchSpeed = autoTestSpeedNotes[i];
                    delayBpm = retouchSpeed.bpm;
                    delayMs = retouchSpeed.ms + 150 * (startPos - retouchSpeed.pos) / delayBpm;
                    testIndex[1] = i;
                    break;
                }
            }
            for (int i = 0; i < autoTestEffectNotes.Count; i++)
            {
                if (autoTestEffectNotes[i].pos >= startPos)
                {
                    testIndex[2] = i;
                    break;
                }
            }
        }

        if (autoTestNormalNotes.Count == 0) {isTesting[0] = false;}
        else {autoTestNormal = autoTestNormalNotes[testIndex[0]];}

        if (autoTestSpeedNotes.Count == 0) {isTesting[1] = false;}
        else {autoTestSpeed = autoTestSpeedNotes[testIndex[1]];}

        if (autoTestEffectNotes.Count == 0) {isTesting[2] = false;}
        else {autoTestEffect = autoTestEffectNotes[testIndex[2]];}
        
        print(delayMs);
        StartCoroutine(StartingTest(delayMs));
    }
    private void TestEnd()
    {
        testLoadingBlock[0].SetActive(false);
        testLoadingBlock[1].SetActive(false);
        autoTestViewObject[0].SetActive(false);
        autoTestViewObject[1].SetActive(true);
        s_isTest = false;
        s_testMs = 0;
        autoMusic.Stop();
        MovingPos = new Vector3(0, 1600 * PageSystem.nowOnPage, 0);
        MovingField[0].transform.localPosition = new Vector3(0, 0, 0);
        MovingField[1].localPosition = MovingPos;
        ScoreManager.scoreManager.AutoTestComboReset();
        NoteClasses.EnableCollider(true);
    }
    private void autoJudgeEffect(int line, bool isLong = false)
    {
        autoTestAnimator[line - 1].SetTrigger(AnimateTrigger);
        autoTestPreviewAnimator[line - 1].SetTrigger(AnimateTrigger);
        if (!isLong) {autoTestJudgeSound[0].Play();}
        else {autoTestJudgeSound[1].Play();}
    }
    
    //*Public -----------------------------------------------*//

    //*IEnumerator -----------------------------------------------*//
    private IEnumerator StartTest(float startPos)
    {
        testLoadingBlock[0].SetActive(true);
        testIndex = new int[3]{0, 0, 0};
        isTesting = new bool[3]{true,true,true};
        s_testMs = 0;
        SpeedMs = 0;
        SpeedPos = 0;
        EffectPos = 0;
        NoteClasses.SortingNotes();
        NoteClasses.EnableCollider(false);
        yield return NoteClasses.CalculateNoteMs();
        Test(startPos);
    }
    private IEnumerator StartingTest(float delayMs)
    {
        testLoadingBlock[0].SetActive(false);
        testLoadingBlock[1].SetActive(true);
        yield return new WaitForSeconds(1.0f);
        s_isTest = true;
        s_testMs = delayMs;
        float waitMs = ValueManager.delay / 1000.0f;
        try { autoMusic.time = delayMs / 1000.0f; }
        catch { TestEnd(); }
        autoMusic.pitch = autoTestMultiply;
        yield return new WaitForSeconds(autoTestMultiply * waitMs);
        autoMusic.Play();
    }
    private IEnumerator LongNoteEffect(int line, int legnth)
    {
        for (int i = 0; i < legnth; i ++)
        {
            autoTestAnimator[line - 1].SetTrigger(AnimateTrigger);
            autoTestPreviewAnimator[line - 1].SetTrigger(AnimateTrigger);
            autoTestJudgeSound[1].Play();
            ScoreManager.scoreManager.AutoTestComboAdd();
            yield return new WaitForSeconds(15 / s_testBpm);
        }
    }
    private IEnumerator EffectNoteStart(float startPos, float startMs, float value,  float bpm, bool isPause)
    {
        print("run effect");
        //* Pause Effect
        if (isPause)
        {
            isOnEffect = true;
            autoTestEffectPos = startPos;
            yield return new WaitForSeconds( (3.0f * value) / (bpm * 20.0f));
            EffectMs += 150 * value / bpm; 
            isOnEffect = false;
        }
        //* Teleport Effect
        else {EffectPos += value;}
    }

    //*Buttons -----------------------------------------------*//
    public void ButtonTest()
    {
        
        if (SaveLoad.s_isWorking){print("A"); return;}
        if (!s_isTest)
        {
            StartCoroutine(StartTest(0.0f));
        }
        else
        {
            TestEnd();
        }
    }
    public void ButtonTestDelay()
    {
        if (SaveLoad.s_isWorking) return;
        if (!s_isTest)
        {
            if ((PageSystem.nowOnPage - 2) * 1600 <= 0) {ButtonTest(); return;}
            StartCoroutine(StartTest((PageSystem.nowOnPage - 2) * 1600));
        }
        else
        {
            TestEnd();
        }
    }
}
