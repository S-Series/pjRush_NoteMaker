using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestPlay : MonoBehaviour
{
    public static TestPlay testPlay;
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

    #region PlaySetting
    int gameSpeed;
    bool isDisplayBottom;
    float gameSpeedMultiple;

    [SerializeField]
    TextMeshPro[] SpeedText;

    [SerializeField]
    TMP_InputField SpeedTextInput;

    [SerializeField]
    GameObject[] NotePrefab;

    int SpeedIndex;
    float testBpm;
    [SerializeField]
    private List<float> SpeedMs;
    [SerializeField]
    private List<float> SpeedBpm;
    [SerializeField]
    private List<float> SpeedPos;
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

    public List<int> testEffectMs;
    public List<float> testEffectForce;
    public List<int> testEffectDuration;

    public List<int> testSpeedMs;
    public List<float> testSpeedBpm;
    public List<int> testSpeedRetouch;

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
    #endregion


    private void Awake()
    {
        gameSpeed = 100;
        gameSpeedMultiple = 1.00f;
        testPlay = this;
        isDisplayBottom = true;
        MovingNoteField = NoteField.transform.parent.gameObject;
        ResetJudge();
        autoTest = AutoTest.autoTest;
    }

    private void FixedUpdate()
    {
        if (AutoTest.autoTest.isPlay)
        {
            playMs++;
        }
        else { playMs = 0; }
    }

    private void Update()
    {
        if (AutoTest.autoTest.isPlay)
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
            posY = TestSpeedPos + ((playMs - TestSpeedMs) * testBpm / 150);
            MovingNoteField.transform.localPosition = new Vector3(0, -posY, 0);

            BlindMovingPos = bpm / 150 * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Escape))
            {

            }

            #region Judge
            TextNum[0].text = (Rush[0] + Rush[1] + Rush[2]).ToString();
            TextNum[1].text = Rush[0].ToString();
            TextNum[2].text = Rush[2].ToString();

            TextNum[3].text = (Step[0] + Step[1]).ToString();
            TextNum[4].text = Step[0].ToString();
            TextNum[5].text = Step[1].ToString();

            TextNum[6].text = (Lost[0] + Lost[1]).ToString();
            TextNum[7].text = Lost[0].ToString();
            TextNum[8].text = Lost[1].ToString();
            #endregion
        }

        if (AutoTest.autoTest.isPlayReady)
        {
            if (Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown(KeyCode.Space))
            {
                AutoTest.autoTest.isPlayReady = false;
                AutoTest.autoTest.isPlay = true;
            }
        }
    }

    public void ResetJudge()
    {
        Rush = new int[3] { 0, 0, 0 };
        Step = new int[2] { 0, 0 };
        Lost = new int[2] { 0, 0 };
    }

    public void ResetList()
    {
        playMs = 0;

        testEffectMs = new List<int>();
        testEffectForce = new List<float>();
        testEffectDuration = new List<int>();

        testSpeedMs = new List<int>();
        testSpeedBpm = new List<float>();
        testSpeedRetouch = new List<int>();

        judge1.TestPlay1 = new List<GameObject>();
        judge1.TestPlayMs1 = new List<float>();
        judge1.TestPlayLegnth1 = new List<int>();
        judge1.resetMs1();

        judge2.TestPlay2 = new List<GameObject>();
        judge2.TestPlayMs2 = new List<float>();
        judge2.TestPlayLegnth2 = new List<int>();
        judge2.resetMs2();

        judge3.TestPlay3 = new List<GameObject>();
        judge3.TestPlayMs3 = new List<float>();
        judge3.TestPlayLegnth3 = new List<int>();
        judge3.resetMs3();

        judge4.TestPlay4 = new List<GameObject>();
        judge4.TestPlayMs4 = new List<float>();
        judge4.TestPlayLegnth4 = new List<int>();
        judge4.resetMs4();

        judgeBottom.TestPlay5 = new List<GameObject>();
        judgeBottom.TestPlayMs5 = new List<float>();
        judgeBottom.TestPlayLegnth5 = new List<int>();
        judgeBottom.resetMs5();
    }

    public IEnumerator TestLoad()
    {
        if (FileName == "")
        {
            AutoTest.autoTest.ButtonPlayStop();
            yield break;
        }
        try
        {
            string path = Path.Combine(Application.dataPath, FileName + ".json");
            string jsonData = File.ReadAllText(path);
            noteSaved = JsonUtility.FromJson<NoteSavedData>(jsonData);
        }
        catch
        {
            AutoTest.autoTest.ButtonPlayStop();
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            Destroy(NoteField.transform.GetChild(i).gameObject);
        }

        SpeedMs = new List<float>();
        SpeedBpm = new List<float>();
        SpeedPos = new List<float>();

        bpm = noteSaved.bpm;
        testBpm = bpm;

        int speedLoadIndex = 0;
        for (int i = 0; i < noteSaved.NoteMs.Count; i++)
        {
            GameObject copy;

            Vector3 notePos;
            notePos = new Vector3(0, 0, 0);

            if (noteSaved.NoteLegnth[i] == 0)
            {
                if (noteSaved.NoteLine[i] >= 5)
                {
                    copy = Instantiate(NotePrefab[2],NoteField.transform);
                }
                else
                {
                    copy = Instantiate(NotePrefab[0], NoteField.transform);
                }
                copy.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                if (noteSaved.NoteLine[i] >= 5)
                {
                    copy = Instantiate(NotePrefab[3], NoteField.transform);
                }
                else
                {
                    copy = Instantiate(NotePrefab[1], NoteField.transform);
                }
                copy.transform.localScale = new Vector3(1, noteSaved.NoteLegnth[i] * 100, 1);
            }
            
            for (int j = speedLoadIndex; j < noteSaved.SpeedMs.Count; j++)
            {
                if (noteSaved.SpeedMs[j] < noteSaved.NoteMs[i])
                {
                    speedLoadIndex = j;
                }
                else
                {
                    break;
                }
            }

            float minusPosRetouch = 0;
            for (int j = 0; j < speedLoadIndex; j++)
            {
                if (noteSaved.SpeedBpm[j] < 0)
                {
                    minusPosRetouch += noteSaved.SpeedBpm[j]
                        * (noteSaved.SpeedMs[j + 1] - noteSaved.SpeedMs[j]) / 75;
                }
            }

            notePos.y = noteSaved.SpeedPos[speedLoadIndex] - minusPosRetouch
                + noteSaved.SpeedBpm[speedLoadIndex] 
                * (noteSaved.NoteMs[i] - noteSaved.SpeedMs[speedLoadIndex]) / 150;

            switch (noteSaved.NoteLine[i])
            {
                case 1:
                    notePos.x = -300;
                    copy.transform.localPosition = notePos;
                    judge1.TestPlay1.Add(copy);
                    judge1.TestPlayMs1.Add(noteSaved.NoteMs[i]);
                    judge1.TestPlayLegnth1.Add(noteSaved.NoteLegnth[i]);
                    break;

                case 2:
                    notePos.x = -100;
                    copy.transform.localPosition = notePos;
                    judge2.TestPlay2.Add(copy);
                    judge2.TestPlayMs2.Add(noteSaved.NoteMs[i]);
                    judge2.TestPlayLegnth2.Add(noteSaved.NoteLegnth[i]); 
                    break;

                case 3:
                    notePos.x = +100;
                    copy.transform.localPosition = notePos;
                    judge3.TestPlay3.Add(copy);
                    judge3.TestPlayMs3.Add(noteSaved.NoteMs[i]);
                    judge3.TestPlayLegnth3.Add(noteSaved.NoteLegnth[i]); 
                    break;

                case 4:
                    notePos.x = +300;
                    copy.transform.localPosition = notePos;
                    judge4.TestPlay4.Add(copy);
                    judge4.TestPlayMs4.Add(noteSaved.NoteMs[i]);
                    judge4.TestPlayLegnth4.Add(noteSaved.NoteLegnth[i]); 
                    break;

                case 5:
                    if (isDisplayBottom)
                    {
                        notePos.x = -100;
                        Vector3 scale;
                        scale = copy.transform.localScale;
                        scale.x = 0.75f;
                        copy.transform.localScale = scale;
                    }
                    copy.transform.localPosition = notePos;
                    judgeBottom.TestPlay5.Add(copy);
                    judgeBottom.TestPlayMs5.Add(noteSaved.NoteMs[i]);
                    judgeBottom.TestPlayLegnth5.Add(noteSaved.NoteLegnth[i]);
                    break;

                case 6:
                    if (isDisplayBottom)
                    {
                        notePos.x = +100;
                        Vector3 scale;
                        scale = copy.transform.localScale;
                        scale.x = 0.75f;
                        copy.transform.localScale = scale;
                    }
                    copy.transform.localPosition = notePos;
                    judgeBottom.TestPlay5.Add(copy);
                    judgeBottom.TestPlayMs5.Add(noteSaved.NoteMs[i]);
                    judgeBottom.TestPlayLegnth5.Add(noteSaved.NoteLegnth[i]);
                    break;
            }
        }

        yield return new WaitForSeconds(0.5f);

        SpeedMs = noteSaved.SpeedMs;
        SpeedPos = noteSaved.SpeedPos;

        for (int i = 0; i < noteSaved.SpeedMs.Count; i++)
        {
            SpeedBpm.Add(noteSaved.SpeedBpm[i] * noteSaved.SpeedNum[i]);
        }

        yield return new WaitForSeconds(0.5f);

        AutoTest.autoTest.isPlayReady = true;
    }

    #region SpeedSetting
    public void SpeedSetting()
    {
        try
        {
            gameSpeed = Convert.ToInt32(SpeedTextInput.text);
            gameSpeedMultiple = bpm / gameSpeed;

            if (gameSpeedMultiple < 0.1f)
            {
                gameSpeedMultiple = 0.1f;
                SpeedTextInput.text = (bpm * 0.1f).ToString();

                SpeedText[0].text = gameSpeedMultiple.ToString();
                SpeedText[1].text = gameSpeed.ToString();
            }
        }
        catch
        {
            gameSpeed = 100;
            SpeedTextInput.text = "100";
        }
    }
    #endregion
}
