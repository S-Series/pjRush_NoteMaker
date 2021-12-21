using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge1 : MonoBehaviour
{
    public List<GameObject> TestPlay1;
    public List<int> TestPlayMs1;
    public List<int> TestPlayLegnth1;

    private GameObject TargetObject;

    private bool isLongJudge;

    private int ms;
    private int judgeMs;
    private int longNoteJudgeMs;
    private int index;

    [SerializeField]
    Animator HitEffect;

    AutoTest auto;

    private void Awake()
    {
        auto = AutoTest.autoTest;
        TestPlay1 = new List<GameObject>();
        TestPlayMs1 = new List<int>();
        index = 0;
        ms = 0;
        isLongJudge = false;
    }

    void Update()
    {
        try
        {
            judgeMs = TestPlayMs1[index] - ms;
            TargetObject = TestPlay1[index];
        }
        catch { return; }

        ms = auto.testMs;

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.M))
        {
            isLongJudge = true;
            JudgeResult(judgeMs);
        }
        else if (Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.M))
        {
            isLongJudge = false;
        }
        else if (TestPlayLegnth1[index] != 0)
        {
            int num;
            num = judgeMs - longNoteJudgeMs;

            if (isLongJudge == true)
            {
                if (num <= 30 && num >= -85)
                {
                    TestPlay.testPlay.Rush[0]++;
                    longNoteJudgeMs += 50;
                }
            }
            else if (num < -85)
            {
                TestPlay.testPlay.Lost[1]++;
                //ComboManager.comboManager.resetCombo();
                longNoteJudgeMs += 50;
            }

            if (longNoteJudgeMs >= TestPlayLegnth1[index])
            {
                longNoteJudgeMs = 0;
                TestPlay1[index].SetActive(false);
                index++;
            }
        }
        else if (judgeMs < -85)
        {
            isLongJudge = false;
            TestPlay.testPlay.Lost[1]++;
            //ComboManager.comboManager.resetCombo();
            TestPlay1[index].SetActive(false);
            index++;
        }
    }

    private void JudgeResult(int judgeMs)
    {
        if (judgeMs >= -30 && judgeMs <= 30)
        {
            TestPlay.testPlay.Rush[1]++;
            HitEffect.SetTrigger("Play");
        }
        else if (judgeMs >= -55 && judgeMs <= 55)
        {
            if (judgeMs > 0)
            {
                TestPlay.testPlay.Rush[0]++;
            }
            else
            {
                TestPlay.testPlay.Rush[2]++;
            }
            HitEffect.SetTrigger("Play");
        }
        else if (judgeMs >= -85 && judgeMs <= 85)
        {
            if (judgeMs > 0)
            {
                TestPlay.testPlay.Step[0]++;
            }
            else
            {
                TestPlay.testPlay.Step[1]++;
            }
        }
        else if (judgeMs > 85 && judgeMs <= 100)
        {
            isLongJudge = false;
            TestPlay.testPlay.Lost[0]++;
            //ComboManager.comboManager.resetCombo();
        }
        else { return; }

        TestPlay1[index].SetActive(false);
        index++;
    }
}
