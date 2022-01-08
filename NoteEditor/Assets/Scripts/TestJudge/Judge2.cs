using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge2 : MonoBehaviour
{
    public List<GameObject> TestPlay2;
    public List<int> TestPlayMs2;
    public List<int> TestPlayLegnth2;

    private GameObject TargetObject;

    private bool isLongJudge;

    private int ms;
    private int judgeMs;
    private int longNoteJudgeMs;
    private int index;

    [SerializeField]
    Animator HitEffect;

    [SerializeField]
    AudioSource[] HitSound;

    AutoTest auto;

    [SerializeField]
    private GameObject LongBlind;

    private void Start()
    {
        auto = AutoTest.autoTest;
        TestPlay2 = new List<GameObject>();
        TestPlayMs2 = new List<int>();
        index = 0;
        ms = 0;
        isLongJudge = false;
    }

    private void OnEnable()
    {
        auto = AutoTest.autoTest;
        TestPlay2 = new List<GameObject>();
        TestPlayMs2 = new List<int>();
        index = 0;
        ms = 0;
        isLongJudge = false;
    }

    void Update()
    {
        try
        {
            judgeMs = TestPlayMs2[index] - ms;
            TargetObject = TestPlay2[index];
        }
        catch { return; }

        ms = auto.testMs;

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Comma))
        {
            isLongJudge = true;
            JudgeResult(judgeMs);
        }
        else if (Input.GetKeyUp(KeyCode.X) || Input.GetKeyUp(KeyCode.Comma))
        {
            StartCoroutine(longKeep());
        }
        else if (judgeMs < -85)
        {
            isLongJudge = false;
            TestPlay.testPlay.Lost[1]++;
            //ComboManager.comboManager.resetCombo();
            CheckLong();
            index++;
        }

        if (!isLongJudge)
        {
            LongBlind.transform.localPosition -= new Vector3(0, TestPlay.testPlay.BlindMovingPos, 0);
        }
        else
        {
            LongBlind.transform.localPosition = new Vector3(-0.54f, 0, 0);
        }
    }

    private IEnumerator longKeep()
    {
        yield return new WaitForSeconds(.5f);
        if (!Input.GetKey(KeyCode.X) && !Input.GetKey(KeyCode.Comma)) isLongJudge = false;
    }
    
    private void CheckLong()
    {
        if (TestPlayLegnth2[index] != 0)
        {
            StartCoroutine(LongStart(TestPlayLegnth2[index]));
        }
        else
        {
            TestPlay2[index].SetActive(false);
        }
    }

    private void JudgeResult(int judgeMs)
    {
        if (judgeMs >= -30 && judgeMs <= 30)
        {
            TestPlay.testPlay.Rush[1]++;
            HitEffect.SetTrigger("Play");
            HitSound[0].Play();
            CheckLong();
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
            HitSound[0].Play();
            CheckLong();
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
            HitEffect.SetTrigger("Play");
            HitSound[0].Play();
            CheckLong();
        }
        else if (judgeMs > 85 && judgeMs <= 100)
        {
            isLongJudge = false;
            TestPlay.testPlay.Lost[0]++;
            //ComboManager.comboManager.resetCombo();
            CheckLong();
        }
        else { return; }

        index++;
    }

    private IEnumerator LongStart(int Legnth)
    {
        var delay = new WaitForSeconds(15 / AutoTest.autoTest.bpm);

        for (int i = 0; i < Legnth; i++)
        {
            if (isLongJudge)
            {
                HitEffect.SetTrigger("Play");
                TestPlay.testPlay.Rush[1]++;
                HitSound[1].Play();
            }
            else
            {
                isLongJudge = false;
                TestPlay.testPlay.Lost[1]++;
                //ComboManager.comboManager.resetCombo();
            }

            yield return delay;

            if (!AutoTest.autoTest.isPlay) break;
        }
            TestPlay2[index - 1].SetActive(false);
    }

    public void resetMs2()
    {
        ms = 0;
    }
}
