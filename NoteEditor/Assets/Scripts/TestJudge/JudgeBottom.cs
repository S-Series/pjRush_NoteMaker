using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeBottom : MonoBehaviour
{
    public List<GameObject> TestPlay5;
    public List<int> TestPlayMs5;
    public List<int> TestPlayLegnth5;

    private GameObject TargetObject;

    private bool isLongJudge;

    private int ms;
    private int judgeMs;
    private int longNoteJudgeMs;
    private int index;

    float wait;

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
        TestPlay5 = new List<GameObject>();
        TestPlayMs5 = new List<int>();
        index = 0;
        ms = 0;
        isLongJudge = false;
    }

    private void OnEnable()
    {
        auto = AutoTest.autoTest;
        TestPlay5 = new List<GameObject>();
        TestPlayMs5 = new List<int>();
        index = 0;
        ms = 0;
        isLongJudge = false;
    }

    void Update()
    {
        try
        {
            judgeMs = TestPlayMs5[index] - ms;
            TargetObject = TestPlay5[index];
        }
        catch { return; }

        ms = auto.testMs;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isLongJudge = true;
            JudgeResult(judgeMs);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
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
            LongBlind.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    private IEnumerator longKeep()
    {
        wait = 15 / AutoTest.autoTest.bpm;
        yield return new WaitForSeconds(2 * wait);
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) isLongJudge = false;
    }

    private void CheckLong()
    {
        if (TestPlayLegnth5[index] != 0)
        {
            StartCoroutine(LongStart(TestPlayLegnth5[index]));
        }
        else
        {
            TestPlay5[index].SetActive(false);
        }
    }

    private void JudgeResult(int judgeMs)
    {
        if (judgeMs >= -30 && judgeMs <= 30)
        {
            TestPlay.testPlay.Rush[1]++;
            HitEffect.SetTrigger("Rush");
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
            HitEffect.SetTrigger("Rush");
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
            HitEffect.SetTrigger("Step");
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
        else
        {
            HitEffect.SetTrigger("None");
            return;
        }

        index++;
    }

    private IEnumerator LongStart(int Legnth)
    {
        SpriteRenderer sprite;
        sprite = TestPlay5[index].GetComponentInChildren<SpriteRenderer>();

        wait = 15 / AutoTest.autoTest.bpm;
        var delay = new WaitForSeconds(wait);
        var white = new Color32(255, 255, 255, 255);
        var middleGray = new Color32(240, 240, 240, 255);
        var gray = new Color32(225, 225, 225, 255);
        var dark = new Color32(150, 150, 150, 255);

        for (int i = 0; i < Legnth; i++)
        {
            if (isLongJudge)
            {
                HitEffect.SetTrigger("Rush");
                TestPlay.testPlay.Rush[1]++;
                sprite.color = white;
                StartCoroutine(color(sprite, wait / 4, middleGray, gray));
                HitSound[1].Play();
            }
            else
            {
                isLongJudge = false;
                sprite.color = dark;
                TestPlay.testPlay.Lost[1]++;
                //ComboManager.comboManager.resetCombo();
            }

            yield return delay;

            if (!AutoTest.autoTest.isPlay) break;
        }
                TestPlay5[index - 1].SetActive(false);
    }

    private IEnumerator color(SpriteRenderer sprite, float duration, Color32 color1, Color32 color2)
    {
        yield return new WaitForSeconds(duration);
        sprite.color = color1;
        yield return new WaitForSeconds(duration);
        sprite.color = color2;
        yield return new WaitForSeconds(duration);
        sprite.color = color1;
    }

    public void resetMs5()
    {
        ms = 0;
    }
}
