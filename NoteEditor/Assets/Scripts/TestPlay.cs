using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestPlay : MonoBehaviour
{
    public static TestPlay testPlay;

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

    int playMs;
    float bpm;
    float TestSpeedPos;
    float TestSpeedMs;

    public float BlindMovingPos;

    public GameObject PlayNoteField;
    public GameObject MovingNoteField;

    [SerializeField]
    TextMeshPro[] TextNum;

    private void Awake()
    {
        testPlay = this;
        ResetJudge();
    }

    private void FixedUpdate()
    {
        if (AutoTest.autoTest.isPlay)
        {
            playMs = AutoTest.autoTest.testMs;
        }
        else { playMs = 0; }
    }

    private void Update()
    {
        if (AutoTest.autoTest.isPlay)
        {
            float posY;
            posY = TestSpeedPos + ((playMs - TestSpeedMs) * bpm / 150);
            MovingNoteField.transform.localPosition = new Vector3(0, -posY, 0);
        }

        BlindMovingPos = bpm / 150 * Time.deltaTime;

        TextNum[0].text = (Rush[0] + Rush[1] + Rush[2]).ToString();
        TextNum[1].text = Rush[0].ToString();
        TextNum[2].text = Rush[2].ToString();

        TextNum[3].text = (Step[0] + Step[1]).ToString();
        TextNum[4].text = Step[0].ToString();
        TextNum[5].text = Step[1].ToString();

        TextNum[6].text = (Lost[0] + Lost[1]).ToString();
        TextNum[7].text = Lost[0].ToString();
        TextNum[8].text = Lost[1].ToString();
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
        judge1.TestPlayMs1 = new List<int>();
        judge1.TestPlayLegnth1 = new List<int>();
        judge1.resetMs1();

        judge2.TestPlay2 = new List<GameObject>();
        judge2.TestPlayMs2 = new List<int>();
        judge2.TestPlayLegnth2 = new List<int>();
        judge2.resetMs2();

        judge3.TestPlay3 = new List<GameObject>();
        judge3.TestPlayMs3 = new List<int>();
        judge3.TestPlayLegnth3 = new List<int>();
        judge3.resetMs3();

        judge4.TestPlay4 = new List<GameObject>();
        judge4.TestPlayMs4 = new List<int>();
        judge4.TestPlayLegnth4 = new List<int>();
        judge4.resetMs4();

        judgeBottom.TestPlay5 = new List<GameObject>();
        judgeBottom.TestPlayMs5 = new List<int>();
        judgeBottom.TestPlayLegnth5 = new List<int>();
        judgeBottom.resetMs5();
    }

    public void getInfo()
    {
        bpm = AutoTest.autoTest.bpm;
    }
}
