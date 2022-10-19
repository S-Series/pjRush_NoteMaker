using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoTest : MonoBehaviour
{
    private const string AnimateTrigger = "Play";
    public const float testSpeed = 3.0f;

    //*Static -----------------------------------------------*//
    public static AutoTest autoTest;
    public static int s_testMs = 0;
    public static float s_testBpm = 120.0f;
    public static bool s_isTest = false, s_isPause = false;
    public static List<NormalNote> autoTestNormalNotes = new List<NormalNote>();
    public static List<SpeedNote> autoTestSpeedNotes = new List<SpeedNote>();
    public static List<EffectNote> autoTestEffectNotes = new List<EffectNote>();
    private List<float> autoTestEffectBpm = new List<float>();
    
    //*Public -----------------------------------------------*//
    public AudioSource autoMusic;
    
    //*SerializeField -----------------------------------------------*//
    [SerializeField] private GameObject autoNoteField;
    [SerializeField] private GameObject[] autoTestViewObject;
    [SerializeField] private Transform[] MovingField;
    [SerializeField] private Animator[] autoTestAnimator;
    [SerializeField] private Animator[] autoTestPreviewAnimator;
    [SerializeField] private AudioSource[] autoTestJudgeSound;
    [SerializeField] private GameObject GuideLine;
    [SerializeField] private Transform GuideLineBox;
    [SerializeField] private GameObject[] testLoadingBlock;
    [SerializeField] private TMP_Dropdown testSpeedDropdown;
    
    //*Private -----------------------------------------------*//
    private Vector3 MovingPos = new Vector3(0, 0, 0);
    private Button[] autoTestButton;
    private int testStartPage = 1, firstLine = 1;
    private int[] testIndex = new int[3]{0, 0, 0};
    private bool[] isTesting = new bool[3]{true, true, true};
    private bool isOnEffect = false;
    private bool isPauseAble = false;
    private float autoTestMultiply = 1.0f;
    private float SpeedMs = 0.0f, SpeedPos = 0.0f;
    private float EffectMs = 0.0f, EffectPos = 0.0f;
    private float autoTestEffectPos = 0.0f;
    private NormalNote autoTestNormal;
    private SpeedNote autoTestSpeed;
    private EffectNote autoTestEffect;
    private NoteOption noteOption;
    private List<GameObject> GuideLines = new List<GameObject>();
    
    //*Private -----------------------------------------------*//
    private void Awake()
    {
        autoTest = this;
    }
    private void FixedUpdate()
    {
        if (!s_isTest) return;
        if (s_isPause) return;
        s_testMs ++;
    }
    private void Update()
    {
        if (!s_isTest) return;
        if (Input.GetKeyDown(KeyCode.Space) && isPauseAble)
        {
            if (s_isPause) 
            {
                autoMusic.time 
                    = ((s_testMs * autoTestMultiply) - EffectMs - ValueManager.delay) / 1000.0f;
                autoMusic.Play();
            }
            else { autoMusic.Pause(); }
            s_isPause = !s_isPause;
        }
        if (s_isPause) return;
        //* NormalNote
        if (isTesting[0])
        {
            if (autoTestNormal.ms <= s_testMs * autoTestMultiply)
            {
                if (autoTestNormal == null) {autoTestNormal = autoTestNormalNotes[testIndex[0]];}

                if (autoTestNormal.legnth == 0) 
                    {autoJudgeEffect(autoTestNormal.line, isPowered:autoTestNormal.isPowered);}
                else {StartCoroutine(LongNoteEffect(autoTestNormal.line, autoTestNormal.legnth, autoTestNormal.noteObject));}

                testIndex[0]++;
                ScoreManager.ApplyJudge(0);
                ScoreManager.ApplyCombo(true);
                if (testIndex[0] >= autoTestNormalNotes.Count) {isTesting[0] = false;}
                else {autoTestNormal = autoTestNormalNotes[testIndex[0]];}
            }
        }
        //* SppedNote
        if (isTesting[1])
        {
            if (autoTestSpeed.ms <= s_testMs * autoTestMultiply)
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
            if (autoTestEffect.ms <= s_testMs * autoTestMultiply)
            {
                if (autoTestEffect == null) {autoTestEffect = autoTestEffectNotes[testIndex[2]];}

                StartCoroutine(EffectNoteStart(autoTestEffect.pos, autoTestEffect.ms, autoTestEffect.value, autoTestEffectBpm[testIndex[2]], autoTestEffect.isPause));

                testIndex[2]++;
                if (testIndex[2] >= autoTestEffectNotes.Count) {isTesting[2] = false;}
                else {autoTestEffect = autoTestEffectNotes[testIndex[2]];}
            }
        }

        if (isOnEffect) {MovingPos.y = autoTestEffectPos;}
        else { MovingPos.y = EffectPos + SpeedPos + (((s_testMs * autoTestMultiply) - SpeedMs - EffectMs) * s_testBpm) / 150; }

        MovingField[0].localPosition = - MovingPos;
        MovingField[1].localPosition = -3.0f * MovingPos * testSpeed;
    }
    private void Test(float startPos)
    {
        isPauseAble = false;
        s_testBpm = ValueManager.bpm;
        autoTestViewObject[0].SetActive(true);
        autoTestViewObject[1].SetActive(false);
        autoTestNormalNotes = new List<NormalNote>();
        autoTestSpeedNotes = new List<SpeedNote>();
        autoTestEffectNotes = new List<EffectNote>();
        autoTestEffectBpm = new List<float>();
        ScoreManager.ResetGamePlay();
        for (int i = 0; i < autoNoteField.transform.childCount; i++)
        {
            Destroy(autoNoteField.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < GuideLines.Count; i++)
        {
            Destroy(GuideLines[i]);
        }
        GuideLines = new List<GameObject>();
        for (int i = 0; i < 1000; i++)
        {
            GameObject copyObject;
            copyObject = Instantiate(GuideLine, GuideLineBox);
            GuideLine.GetComponentInChildren<TextMeshPro>().text = string.Format("{0:D3}", i);
            GuideLine.transform.localPosition = new Vector3(0.0f, 1600.0f * i * testSpeed, 0.0f);
            GuideLines.Add(copyObject);
        }
        //*-------------------------------------------------------------------
        int _fullCombo = 0;
        for (int i = 0; i < NormalNote.normalNotes.Count; i++)
        {
            NormalNote normalNote = NormalNote.normalNotes[i];
            NormalNote autoNormalNote = new NormalNote();

            GameObject _copyObject;
            _copyObject = Instantiate(normalNote.noteObject, autoNoteField.transform);
            NoteOption _noteOption;
            _noteOption = _copyObject.GetComponent<NoteOption>();

            autoNormalNote.ms = normalNote.ms;
            autoNormalNote.pos = normalNote.pos;
            autoNormalNote.line = normalNote.line;
            autoNormalNote.isPowered = normalNote.isPowered;
            autoNormalNote.legnth = normalNote.legnth;
            autoNormalNote.noteObject = _copyObject;
            _copyObject.transform.GetComponent<NoteOption>().ToLongNote(autoNormalNote.legnth, 300);

            if (normalNote.legnth == 0) { _fullCombo++; }
            else { _fullCombo += normalNote.legnth; }

            for (int j = 0; j < 3; j++)
            {
                autoNormalNote.noteObject.transform.GetChild(0).GetChild(j)
                    .GetComponent<BoxCollider2D>().enabled = false;
                autoNormalNote.noteObject.transform.GetChild(1).GetChild(j)
                    .GetComponent<BoxCollider2D>().enabled = false;
            }

            Vector3 pos = autoNormalNote.noteObject.transform.localPosition;
            if (autoNormalNote.line < 5)
            {
                pos.x = ((firstLine - 1) * -200.0f) + ((autoNormalNote.line * 200.0f) - 500.0f); 
                if (pos.x < -350.0f) { pos.x += 600.0f; }
            }
            else 
            {
                if (autoNormalNote.isPowered)
                {
                    if (autoNormalNote.line == 5) { firstLine++; }
                    else { firstLine--; }
                    if (firstLine < 1) { firstLine += 4; }
                    else if (firstLine > 4) { firstLine -= 4; }
                } 
            }
            pos.y *= testSpeed;
            autoNormalNote.noteObject.transform.localPosition = pos;

            autoTestNormalNotes.Add(autoNormalNote);
        }
        ScoreManager.SetGameInfo(_fullCombo);
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
        int startEffectMs = 0;
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
            if (autoEffectNote.pos <= startPos)
            {
                if (autoEffectNote.isPause) { startEffectMs += Convert.ToInt32(150 * autoEffectNote.value / effectBpm); }
            }
            autoTestEffectBpm.Add(effectBpm);
            /*foreach(GameObject guide in GuideLines)
            {

                if (guide.transform.localPosition.y > autoEffectNote.pos)
                {
                    Vector3 _pos;
                    _pos = guide.transform.localPosition;
                    if (autoEffectNote.isPause)
                    {
                        if (_pos.y - autoEffectNote.value <= autoEffectNote.pos) _pos.y = autoEffectNote.pos * testSpeed;
                        else _pos.y -= autoEffectNote.pos * testSpeed;
                    }
                    else { _pos.y += autoEffectNote.pos * testSpeed; }
                    guide.transform.localPosition = _pos;
                }
            }*/
        }
        testIndex = new int[3]{0, 0, 0};
        float delayBpm = ValueManager.bpm;
        float calculateMs;
        calculateMs = (150.0f * startPos / delayBpm) / autoTestMultiply;
        int delayMs = Convert.ToInt32(calculateMs);
        EffectMs = startEffectMs;
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
                    delayMs = Convert.ToInt32(retouchSpeed.ms + 150 * (startPos - retouchSpeed.pos) / delayBpm / autoTestMultiply);
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
        ScoreManager.ResetGamePlay();
        NoteClasses.EnableCollider(true);
    }
    private void autoJudgeEffect(int line, bool isPowered = false, bool isLong = false)
    {
        autoTestAnimator[line - 1].SetTrigger(AnimateTrigger);
        autoTestPreviewAnimator[line - 1].SetTrigger(AnimateTrigger);
        if (isLong) { autoTestJudgeSound[1].Play(); }
        else if (isPowered) { autoTestJudgeSound[2].Play(); }
        else { autoTestJudgeSound[0].Play(); }
    }
    private void BottomPowered(bool _isLeft)
    {

    }
    //*Public -----------------------------------------------*//
    public void testSpeedDropDownChange()
    {
        s_testMs = Convert.ToInt32(s_testMs * autoTestMultiply);
        // EffectMs = Convert.ToInt32(EffectMs * autoTestMultiply);
        autoTestMultiply = Convert.ToSingle(testSpeedDropdown.options[testSpeedDropdown.value].text);
        s_testMs = Convert.ToInt32(s_testMs / autoTestMultiply);
        // EffectMs = Convert.ToInt32(EffectMs / autoTestMultiply);
        autoMusic.pitch = autoTestMultiply;
        autoMusic.time = ((s_testMs * autoTestMultiply) - ValueManager.delay) / 1000.0f;
    }
    //*IEnumerator -----------------------------------------------*//
    private IEnumerator StartTest(float startPos)
    {
        testSpeedDropdown.interactable = false;
        testLoadingBlock[0].SetActive(true);
        testIndex = new int[3]{0, 0, 0};
        isTesting = new bool[3]{true,true,true};
        s_testMs = 0;
        SpeedMs = 0;
        SpeedPos = 0;
        EffectMs = 0;
        EffectPos = 0;
        NoteClasses.SortingNotes();
        NoteClasses.EnableCollider(false);
        yield return NoteClasses.CalculateNoteMs();
        Test(startPos);
    }
    private IEnumerator StartingTest(int delayMs)
    {
        testLoadingBlock[0].SetActive(false);
        testLoadingBlock[1].SetActive(true);
        yield return new WaitForSeconds(1.0f);
        s_isTest = true;
        s_isPause = false;
        s_testMs = delayMs;
        float waitMs = ValueManager.delay / 1000.0f;
        try { autoMusic.time = ((s_testMs + 0) * autoTestMultiply - ValueManager.delay) / 1000.0f; }
        catch { TestEnd(); }
        autoMusic.pitch = autoTestMultiply;
        yield return new WaitForSeconds(waitMs / autoTestMultiply);
        autoMusic.time = ((s_testMs + 0) * autoTestMultiply - ValueManager.delay) / 1000.0f;
        autoMusic.Play();
        testSpeedDropdown.interactable = true;
        isPauseAble = true;
    }
    private IEnumerator LongNoteEffect(int line, int legnth, GameObject note)
    {
        note.GetComponent<Animator>().enabled = true;
        note.GetComponent<Animator>().SetTrigger("Catch");
        for (int i = 0; i < legnth - 1; i ++)
        {
            while(s_isPause) { yield return null; }
            autoTestAnimator[line - 1].SetTrigger(AnimateTrigger);
            autoTestPreviewAnimator[line - 1].SetTrigger(AnimateTrigger);
            autoTestJudgeSound[1].Play();
            ScoreManager.ApplyJudge(0);
                ScoreManager.ApplyCombo(true);
            yield return new WaitForSeconds(15 / s_testBpm / autoTestMultiply);
        }
        note.GetComponent<Animator>().SetTrigger("Exit");
        note.GetComponent<Animator>().enabled = false;
    }
    private IEnumerator EffectNoteStart(float startPos, float startMs, float value,  float bpm, bool isPause)
    {
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
