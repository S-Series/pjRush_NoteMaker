using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        testPlay = this;
        ResetJudge();
    }

    public void ResetJudge()
    {
        Rush = new int[3] { 0, 0, 0 };
        Step = new int[2] { 0, 0 };
        Lost = new int[2] { 0, 0 };
    }
}
