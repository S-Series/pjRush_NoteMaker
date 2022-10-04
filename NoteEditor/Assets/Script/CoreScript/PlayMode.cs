using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMode : MonoBehaviour
{
    private static PlayMode s_PM;
    public static bool s_isPlay;
    public static JudgeSystem[] s_judgeSystems = new JudgeSystem[4];
    public static BottomJudgeSystem[] s_bottomJudgeSystems = new BottomJudgeSystem[2];
    [SerializeField] private GameObject[] judgeSystemParents;
    private void Awake()
    {
        s_PM = this;
        for (int i = 0; i < 4; i++)
        {
            s_judgeSystems[i]
                = judgeSystemParents[0].transform.GetChild(i).GetComponent<JudgeSystem>();
        }
        for (int i = 0; i < 2; i++)
        {
            s_bottomJudgeSystems[i]
                = judgeSystemParents[1].transform.GetChild(i).GetComponent<BottomJudgeSystem>();
        }
    }
    public static void toPlayMode(bool _isPlayMode)
    {

    }

    private void NoteCalculate()
    {
        
    }
}
