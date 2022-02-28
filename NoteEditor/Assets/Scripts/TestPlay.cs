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
    [SerializeField]
    NoteSavedData noteSaved = new NoteSavedData();
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
    public int[] Rush = new int[3]{ 0, 0, 0 };
    public int[] Step = new int[2]{ 0, 0 };
    public int[] Lost = new int[2]{ 0, 0 };

    public Judge1 judge1;
    public Judge2 judge2;
    public Judge3 judge3;
    public Judge4 judge4;
    public JudgeBottom judgeBottom;

    [SerializeField]
    public  int playMs;
    float bpm;
    float TestSpeedPos;
    float TestSpeedMs;

    public float BlindMovingPos;
    public string FileName;

    public GameObject PlayNoteField;
    public GameObject NoteField;
    [SerializeField]
    private GameObject MovingNoteField;

    [SerializeField]
    TextMeshPro[] TextNum;

    [SerializeField]
    GameObject[] PrefabObject;
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
            TextNum[0].text = string.Format("{0:D4}", (Rush[0] + Rush[1] + Rush[2]));
            TextNum[1].text = string.Format("{0:D4}",Rush[0]);
            TextNum[2].text = string.Format("{0:D4}", Rush[2]);

            TextNum[3].text = string.Format("{0:D4}", (Step[0] + Step[1]));
            TextNum[4].text = string.Format("{0:D4}", Step[0]);
            TextNum[5].text = string.Format("{0:D4}", Step[1]);

            TextNum[6].text = string.Format("{0:D4}", (Lost[0] + Lost[1]));
            TextNum[7].text = string.Format("{0:D4}", Lost[0]);
            TextNum[8].text = string.Format("{0:D4}", Lost[1]);
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
        judge1.resetMs1();
        judge2.resetMs2();
        judge3.resetMs3();
        judge4.resetMs4();
        judgeBottom.resetMs5();
    }

    public IEnumerator TestLoad()
    {
        MovingNoteField.transform.localPosition = new Vector3(0, 0, 0);

        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            Destroy(NoteField.transform.GetChild(i).gameObject);
        }

        autoTest = AutoTest.autoTest;
        Music = autoTest.Music;
        ResetData();
        note = new List<GameObject>();
        notePos = new List<float>();

        if (FileName == "")
        {
            AutoTest.autoTest.ButtonPlayStop();
            yield break;
        }
        try
        {
            string path = Path.Combine(Application.dataPath, FileName + ".json");
            string jsonData = File.ReadAllText(path);
            print(path);
            noteSaved = JsonUtility.FromJson<NoteSavedData>(jsonData);
        }
        catch
        {
            AutoTest.autoTest.ButtonPlayStop();
            yield break;
        }

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            Destroy(NoteField.transform.GetChild(i).gameObject);
        }

        bpm = noteSaved.bpm;
        testBpm = bpm;

        SpeedMs = noteSaved.SpeedMs;
        for (int i = 0; i < noteSaved.SpeedBpm.Count; i++)
        {
            SpeedBpm.Add(noteSaved.SpeedBpm[i] * noteSaved.SpeedNum[i]);
        }
        SpeedPos = noteSaved.SpeedPos;

        GuideGenerate(noteSaved.NoteMs[noteSaved.NoteMs.Count - 1]);
        yield return new WaitForSeconds(1.0f);

        /*for (int i = 0; i < noteSaved.SpeedMs.Count; i++)
        {
            GameObject copy;
            copy = Instantiate(PrefabObject[5], NoteField.transform);
            copy.transform.localPosition = new Vector3(0, noteSaved.SpeedPos[i], 0);
            copy.transform.GetComponentInChildren<TextMeshPro>().text =
                noteSaved.SpeedBpm[i].ToString() + "\nx " + noteSaved.SpeedNum[i].ToString();
            copy.transform.GetChild(0).GetChild(0).localPosition
                = new Vector3(noteSaved.SpeedNum[i], noteSaved.SpeedBpm[i], 0);
        }*/

        for (int i = 0; i < noteSaved.NoteMs.Count; i++)
        {
            GameObject copiedObject;
            Vector3 copiedObjectPos;
            copiedObjectPos = new Vector3(0, 0, 0);

            // 노트파일 해석 시작
            // 노트 X 위치와 노트의 종류 해석
            try
            {
                copiedObjectPos.z = 0;
                if (noteSaved.NoteLine[i] == 5 || noteSaved.NoteLine[i] == 6)
                {
                    if (noteSaved.NoteLine[i] == 5) { copiedObjectPos.x = -100; }
                    else { copiedObjectPos.x = 100; }

                    if (noteSaved.NoteLegnth[i] != 0)
                    {
                        copiedObject = Instantiate(PrefabObject[3], NoteField.transform);
                        copiedObject.transform.localScale = new Vector3(0.75f, noteSaved.NoteLegnth[i] * 100, 1);
                        noteLegnth.Add(noteSaved.NoteLegnth[i]);
                    }
                    else
                    {
                        copiedObject = Instantiate(PrefabObject[2], NoteField.transform);
                        copiedObject.transform.localScale = new Vector3(0.75f, 3, 1);
                        noteLegnth.Add(0);
                    }
                }
                else
                {
                    if (noteSaved.NoteLegnth[i] != 0)
                    {
                        copiedObject = Instantiate(PrefabObject[1], NoteField.transform);
                        copiedObject.transform.localScale = new Vector3(1, noteSaved.NoteLegnth[i] * 100, 1);
                        noteLegnth.Add(noteSaved.NoteLegnth[i]);
                    }
                    else
                    {
                        copiedObject = Instantiate(PrefabObject[0], NoteField.transform);
                        copiedObject.transform.localScale = new Vector3(1, 3, 1);
                        noteLegnth.Add(0);
                    }
                }

                switch (noteSaved.NoteLine[i])
                {
                    case 1:
                        copiedObjectPos.x = -300;
                        judge1.NoteDataAddTo1
                            (copiedObject, noteSaved.NoteMs[i], noteSaved.NoteLegnth[i]);
                        break;

                    case 2:
                        copiedObjectPos.x = -100;
                        judge2.NoteDataAddTo2
                            (copiedObject, noteSaved.NoteMs[i], noteSaved.NoteLegnth[i]);
                        break;

                    case 3:
                        copiedObjectPos.x = +100;
                        judge3.NoteDataAddTo3
                            (copiedObject, noteSaved.NoteMs[i], noteSaved.NoteLegnth[i]);
                        break;

                    case 4:
                        copiedObjectPos.x = +300;
                        judge4.NoteDataAddTo4
                            (copiedObject, noteSaved.NoteMs[i], noteSaved.NoteLegnth[i]);
                        break;

                    case 5:
                        copiedObjectPos.x = -100;
                        judgeBottom.NoteDataAddTo5
                            (copiedObject, noteSaved.NoteMs[i], noteSaved.NoteLegnth[i]);
                        break;

                    case 6:
                        copiedObjectPos.x = +100;
                        judgeBottom.NoteDataAddTo5
                            (copiedObject, noteSaved.NoteMs[i], noteSaved.NoteLegnth[i]);
                        break;
                }

                // 노트의 Y좌표 해석
                // Ms = 150 * PosY / Bpm |=>| PosY = Bpm * Ms / 150
                if (noteSaved.NotePos.Count == 0)
                {
                    float pos;
                    pos = noteSaved.bpm * noteSaved.NoteMs[i] / 150;
                    if (pos % 1 >= .5f) pos = pos - pos % 1 + 1;
                    else pos = pos - pos % 1;
                    copiedObjectPos.y = pos;
                }
                else
                {
                    copiedObjectPos.y = noteSaved.NotePos[i];
                }

                copiedObject.transform.localPosition = copiedObjectPos;
            }
            catch
            {
                break;
            }

            note.Add(copiedObject);
        }

        for (int i = 0; i < noteSaved.SpeedMs.Count; i++)
        {
            if (SpeedBpm[i] <= 0)
            {
                float minusSpeedPos;
                minusSpeedPos = (noteSaved.SpeedPos[i + 1] - noteSaved.SpeedPos[i]) * 2;
                for (int j = 0; j < noteSaved.NoteMs.Count; j++)
                {
                    if (noteSaved.NoteMs[j] >= noteSaved.SpeedMs[i])
                    {
                        for (; j < noteSaved.NoteMs.Count; j++)
                        {
                            Vector3 notePos;
                            notePos = note[j].transform.localPosition;
                            notePos.y -= minusSpeedPos;
                            note[j].transform.localPosition = notePos;
                        }
                        break;
                    }
                }
                for (int j = i + 1; j < SpeedPos.Count; j++)
                {
                    SpeedPos[j] -= minusSpeedPos;
                }
            }

            for (int j = 0; j < GuideLine.Count; j++)
            {
                if (GuideLine[j].transform.localPosition.y >= SpeedPos[i])
                {
                    for (; j < GuideLine.Count - 1; j++)
                    {
                        float guideMs;
                        guideMs = (240000 / bpm ) * j;

                        Vector3 guidePos;
                        guidePos = GuideLine[j + 1].transform.localPosition;

                        guidePos.y = SpeedPos[i] + (SpeedBpm[i] * (guideMs - SpeedMs[i]) / 150);
                        GuideLine[j + 1].transform.localPosition = guidePos;
                    }
                    break;
                }
            }
        }

        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < note.Count; i++)
        {
            notePos.Add(note[i].transform.localPosition.y);
        }

        for (int i = 0; i < GuideLine.Count; i++)
        {
            GuidePos.Add(GuideLine[i].transform.localPosition.y);
        }

        yield return new WaitForSeconds(.5f);
        SpeedSetting(100);
        yield return new WaitForSeconds(.5f);
        PlayButton[0].interactable = true;
    }

    private void ResetData()
    {
        playMs = 0;

        #region JudgeData
        judge1.TestPlay1 = new List<GameObject>();
        judge1.TestPlayMs1 = new List<float>();
        judge1.TestPlayLegnth1 = new List<int>();

        judge2.TestPlay2 = new List<GameObject>();
        judge2.TestPlayMs2 = new List<float>();
        judge2.TestPlayLegnth2 = new List<int>();

        judge3.TestPlay3 = new List<GameObject>();
        judge3.TestPlayMs3 = new List<float>();
        judge3.TestPlayLegnth3 = new List<int>();

        judge4.TestPlay4 = new List<GameObject>();
        judge4.TestPlayMs4 = new List<float>();
        judge4.TestPlayLegnth4 = new List<int>();

        judgeBottom.TestPlay5 = new List<GameObject>();
        judgeBottom.TestPlayMs5 = new List<float>();
        judgeBottom.TestPlayLegnth5 = new List<int>();
        #endregion  

        note = new List<GameObject>();
        effect = new List<GameObject>();
        speed = new List<GameObject>();

        noteSaved.bpm = new float();
        noteSaved.startDelayMs = new int();

        noteSaved.NoteLegnth = new List<int>();
        noteSaved.NoteMs = new List<float>();
        noteSaved.NoteLine = new List<int>();

        noteSaved.EffectMs = new List<float>();
        noteSaved.EffectForce = new List<float>();
        noteSaved.EffectDuration = new List<int>();

        noteSaved.SpeedMs = new List<float>();
        noteSaved.SpeedPos = new List<float>();
        noteSaved.SpeedBpm = new List<float>();
        noteSaved.SpeedNum = new List<float>();

        noteMs = new List<float>();
        notePos = new List<float>();
        noteLine = new List<int>();
        noteLegnth = new List<int>();

        effect = new List<GameObject>();
        EffectMs = new List<float>();
        EffectForce = new List<float>();
        EffectDuration = new List<int>();

        speed = new List<GameObject>();
        SpeedMs = new List<float>();
        SpeedPos = new List<float>();
        SpeedBpm = new List<float>();

        SpeedMs = new List<float>();
        SpeedBpm = new List<float>();
        SpeedPos = new List<float>();

        GuidePos = new List<float>();
    }

    public void SpeedSetting(int getSpeed)
    {
        isPlayReady = false;
        try
        {
            if (getSpeed == 0)
            {
                gameSpeed = Convert.ToInt32(SpeedTextInput.text);
                if (gameSpeed < 50)
                {
                    gameSpeed = 50;
                    SpeedTextInput.text = "50";
                }
            }
            else
            {
                gameSpeed = getSpeed;
            }

            gameSpeedMultiple = gameSpeed / bpm;

            SpeedText[0].text = bpm.ToString();
            SpeedText[1].text = string.Format("{0:F2}", gameSpeedMultiple);
            SpeedText[2].text = gameSpeed.ToString();
        }
        catch
        {
            gameSpeed = 100;
            gameSpeedMultiple = gameSpeed / bpm;
            SpeedText[0].text = bpm.ToString();
            SpeedText[1].text = string.Format("{0:F2}", gameSpeedMultiple);
            SpeedText[2].text = gameSpeed.ToString();
        }

        float multiple;
        multiple = gameSpeed / 100f;
        print(gameSpeed);
        print(multiple);

        for (int i = 0; i < note.Count; i++)
        {
            Vector3 notePosSet;
            notePosSet = note[i].transform.localPosition;
            notePosSet.y = notePos[i] * multiple;
            note[i].transform.localPosition = notePosSet;

            if (noteLegnth[i] != 0)
            {
                Vector3 noteScale;
                noteScale = note[i].transform.localScale;
                noteScale.y = noteLegnth[i] * multiple * 100;
                note[i].transform.localScale = noteScale;
            }
        }

        for (int i = 0; i < GuideLine.Count; i++)
        {
            Vector3 guidePosSet;
            guidePosSet = GuideLine[i].transform.localPosition;
            guidePosSet.y = GuidePos[i] * multiple;
            GuideLine[i].transform.localPosition = guidePosSet;
        }

        isPlayReady = true;
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
                = string.Format("{0:D3}",i + 1);
            GuideLine.Add(copy);
        }
    }
    
    private void ResetNote()
    {
        foreach(GameObject gameObject in note)
        {
            gameObject.SetActive(true);
        }
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
}
