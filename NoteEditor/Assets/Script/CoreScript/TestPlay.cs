using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestPlay : MonoBehaviour
{
    public static TestPlay testPlay;
    [SerializeField] NoteSavedData noteSaved = new NoteSavedData();
    /// <summary>
    /// public class NoteSavedData
    /// {
    ///     public float bpm;
    ///     public int difficulty;
    ///     public int startDelayMs;
    ///
    ///     public List<int> NoteLegnth;
    ///     public List<float> NoteMs;
    ///     public List<int> NoteLine;
    ///
    ///     public List<int> EffectMs;
    ///     public List<float> EffectForce;
    ///     public List<int> EffectDuration;
    ///
    ///     public List<int> SpeedMs;
    ///     public List<float> SpeedPos;
    ///     public List<float> SpeedBpm;
    ///     public List<float> SpeedNum;
    /// }
    /// </summary>

    AutoTest autoTest;
    public static ScoreManager scoreManager;
    public static bool isPlay;
    public static bool isPlayReady;

    #region PlaySetting 
    bool isDisplayBottom;
    int gameSpeed;
    float gameSpeedMultiple;

    [SerializeField]
    TextMeshPro[] SpeedText;

    [SerializeField]
    TMP_InputField SpeedTextInput;

    [SerializeField]
    GameObject[] NotePrefab;

    int SpeedIndex;
    public static float testBpm;

    [SerializeField]
    private List<GameObject> note;
    private List<float> noteMs;
    private List<float> notePos;
    private List<int> noteLine;
    private List<int> noteLegnth;
    private List<bool> noteDouble;

    private List<GameObject> effect;
    private List<float> EffectMs;
    private List<float> EffectPos;
    private List<float> EffectForce;
    private List<int> EffectDuration;

    private List<GameObject> speed;
    private List<float> SpeedMs;
    private List<float> SpeedPos;
    private List<float> SpeedBpm;

    private List<float> GuidePos;
    #endregion
    #region PlayData
    // { Rush, Step, Miss }
    public int[] Rush = new int[3] { 0, 0, 0 };
    public int[] Step = new int[2] { 0, 0 };
    public int[] Lost = new int[2] { 0, 0 };

    public JudgeSystem[] judge;
    public static int playMs;
    float bpm;
    float TestSpeedPos;
    float TestSpeedMs;

    public float BlindMovingPos;
    public string FileName;

    public GameObject PlayNoteField;
    public GameObject NoteField;
    [SerializeField] private GameObject MovingNoteField;

    [SerializeField] TextMeshPro[] TextNum;

    [SerializeField] GameObject[] PrefabObject;
    #endregion

    AudioSource Music;

    [SerializeField]
    GameObject PlayGuide;
    [SerializeField]
    GameObject PlayGuideParent;

    [SerializeField]
    Button[] PlayButton;
    // 0 = Start
    // 1 = ReStart
    // 2 = Stop

    List<GameObject> GuideLine;
    private void Awake()
    {
        gameSpeed = 100;
        gameSpeedMultiple = 1.00f;
        testPlay = this;
        isDisplayBottom = true;
        MovingNoteField = NoteField.transform.parent.gameObject;
        ResetJudge();

        PlayButton[0].interactable = false;
        PlayButton[1].interactable = false;
        PlayButton[2].interactable = false;

        scoreManager = GetComponentInChildren<ScoreManager>();
    }
    private void FixedUpdate()
    {
        if (isPlay)
        {
            playMs++;
        }
        else { playMs = 0; }
    }
    private void Update()
    {
        if (isPlay)
        {
            try
            {
                if (SpeedMs[SpeedIndex] <= playMs)
                {
                    testBpm = SpeedBpm[SpeedIndex];
                    //LongDelay = 15 / SpeedBpm[SpeedIndex];
                    TestSpeedMs = SpeedMs[SpeedIndex];
                    TestSpeedPos = SpeedPos[SpeedIndex];
                    SpeedIndex++;
                }
            }
            catch { }

            float posY;
            posY = (TestSpeedPos + ((playMs - TestSpeedMs) * testBpm / 150)) / 3;
            MovingNoteField.transform.localPosition = new Vector3(0, -posY * gameSpeed / 100, 0);

            BlindMovingPos = bpm / 150 * Time.deltaTime;

            #region Judge

            #endregion

            if (Input.GetKeyDown(KeyCode.F5))
            {
                ButtonPlayReStart();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ButtonPlayStop();
            }
        }

        if (isPlayReady)
        {
            if (Input.GetKeyDown(KeyCode.Return)) ButtonPlayStart();

            if (Input.GetKeyDown(KeyCode.Keypad1)) SpeedSetting(100);
            if (Input.GetKeyDown(KeyCode.Keypad2)) SpeedSetting(200);
            if (Input.GetKeyDown(KeyCode.Keypad3)) SpeedSetting(300);
            if (Input.GetKeyDown(KeyCode.Keypad4)) SpeedSetting(400);
            if (Input.GetKeyDown(KeyCode.Keypad5)) SpeedSetting(500);
            if (Input.GetKeyDown(KeyCode.Keypad6)) SpeedSetting(600);
            if (Input.GetKeyDown(KeyCode.Keypad7)) SpeedSetting(700);
            if (Input.GetKeyDown(KeyCode.Keypad8)) SpeedSetting(800);
            if (Input.GetKeyDown(KeyCode.Keypad9)) SpeedSetting(900);
        }
    }
    public void ResetJudge()
    {
        Rush = new int[3] { 0, 0, 0 };
        Step = new int[2] { 0, 0 };
        Lost = new int[2] { 0, 0 };
    }
    public void TestLoad()
    {

    }
    public void SpeedSetting(int getSpeed)
    {
        
    }
    private void GuideGenerate(float num)
    {
        for (int i = 0; i < PlayGuideParent.transform.childCount; i++)
        {
            Destroy(PlayGuideParent.transform.GetChild(0).gameObject);
        }

        GuideLine = new List<GameObject>();
        long count;
        // ms = 150 * 1600 / bpm
        count = Mathf.CeilToInt(num * bpm / 240000) + 2;

        for (int i = 0; i < count; i++)
        {
            GameObject copy;
            copy = Instantiate(PlayGuide, PlayGuideParent.transform);
            copy.transform.localPosition = new Vector3(0, 1600 * i, 0);
            copy.transform.GetChild(0).GetComponent<TextMeshPro>().text
                = string.Format("{0:D3}", i + 1);
            GuideLine.Add(copy);
        }
    }
    private void ResetNote()
    {
        foreach (GameObject gameObject in note)
        {
            gameObject.SetActive(true);
        }
        for (int i = 0; i < note.Count; i++)
        {
            if (noteDouble[i])
            {
                note[i].transform.GetChild(0).GetChild(0).
                    GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
    private void AddNote(int index, GameObject note, float ms, int Legnth, bool isDouble)
    {
        judge[index].NoteDataAddTo(note, ms, Legnth, isDouble);
    }
    IEnumerator PlayMusic()
    {
        yield return new WaitForSeconds(noteSaved.startDelayMs / 1000);
        Music.Play();
    }
    public void ButtonPlayStart()
    {
        isPlay = true;
        isPlayReady = false;
        StartCoroutine(PlayMusic());
        PlayButton[0].interactable = false;
        PlayButton[1].interactable = true;
        PlayButton[2].interactable = true;
        foreach (JudgeSystem judgeSystem in judge)
        {
            judgeSystem.reStart();
        }
        scoreManager.ResetScore();
    }
    public void ButtonPlayReStart()
    {
        MovingNoteField.transform.localPosition = new Vector3(0, 0, 0);
        playMs = 0;
        TestSpeedMs = 0;
        SpeedIndex = 0;
        TestSpeedPos = 0;
        bpm = noteSaved.bpm;
        testBpm = bpm;
        Music.Stop();
        ResetJudge();
        ResetNote();
        StartCoroutine(PlayMusic());
        foreach (JudgeSystem judgeSystem in judge)
        {
            judgeSystem.reStart();
        }
        scoreManager.ResetScore();
    }
    public void ButtonPlayStop()
    {
        isPlay = false;
        isPlayReady = true; MovingNoteField.transform.localPosition = new Vector3(0, 0, 0);

        playMs = 0;
        TestSpeedMs = 0;
        SpeedIndex = 0;
        TestSpeedPos = 0;
        bpm = noteSaved.bpm;
        testBpm = bpm;
        Music.Stop();
        ResetNote();
        ResetJudge();
        PlayButton[0].interactable = true;
        PlayButton[1].interactable = false;
        PlayButton[2].interactable = false;
    }
    public void JudgeDisplay()
    {
        TextNum[0].text = string.Format("{0:D4}", (Rush[0] + Rush[1] + Rush[2]));
        TextNum[1].text = string.Format("{0:D4}", Rush[0]);
        TextNum[2].text = string.Format("{0:D4}", Rush[2]);

        TextNum[3].text = string.Format("{0:D4}", (Step[0] + Step[1]));
        TextNum[4].text = string.Format("{0:D4}", Step[0]);
        TextNum[5].text = string.Format("{0:D4}", Step[1]);

        TextNum[6].text = string.Format("{0:D4}", (Lost[0] + Lost[1]));
        TextNum[7].text = string.Format("{0:D4}", Lost[0]);
        TextNum[8].text = string.Format("{0:D4}", Lost[1]);
    }
}
