using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge1 : MonoBehaviour
{
    public List<GameObject> TestPlay1;
    public List<float> TestPlayMs1;
    public List<int> TestPlayLegnth1;

    private GameObject TargetObject;

    private bool isLongJudge;

    private int ms;
    private float judgeMs;
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
        TestPlay1 = new List<GameObject>();
        TestPlayMs1 = new List<float>();
        index = 0;
        ms = 0;
        isLongJudge = false;
    }

    private void OnEnable()
    {
        auto = AutoTest.autoTest;
        TestPlay1 = new List<GameObject>();
        TestPlayMs1 = new List<float>();
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

        ms = TestPlay.testPlay.playMs;

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.M))
        {
            isLongJudge = true;
            JudgeResult(judgeMs);
        }
        else if (Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.M))
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
            LongBlind.transform.localPosition = new Vector3(-1.62f, 0, 0);
        }
    }

    private IEnumerator longKeep()
    {
        wait = 15 / AutoTest.autoTest.bpm;
        yield return new WaitForSeconds(2 * wait);
        if (!Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.M)) isLongJudge = false;
    }

    private void CheckLong()
    {
        if (TestPlayLegnth1[index] != 0)
        {
            StartCoroutine(LongStart(TestPlayLegnth1[index], TestPlay1[index]));
        }
        else
        {
            TestPlay1[index].SetActive(false);
        }
    }

    private void JudgeResult(float judgeMs)
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

    private IEnumerator LongStart(int Legnth, GameObject longObject)
    {
        SpriteRenderer sprite;
        sprite = TestPlay1[index].GetComponentInChildren<SpriteRenderer>();

        wait = 15 / TestPlay.testBpm;
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

            if (!TestPlay.isPlay) break;
        }
        sprite.color = white;
        longObject.SetActive(false);
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

    public void resetMs1()
    {
        index = 0;
        ms = 0;
    }

    public void NoteDataAddTo1(GameObject noteObject, float ms, int legnth)
    {
        TestPlay1.Add(noteObject);
        TestPlayMs1.Add(ms);
        TestPlayLegnth1.Add(legnth);
    }
}
