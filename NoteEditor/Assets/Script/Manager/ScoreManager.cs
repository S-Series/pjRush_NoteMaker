using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager scoreManager;
    public static int s_playScore;
    private static int s_fullCombo;
    private static int s_maxCombo;
    private static int s_nowCombo;

    //** [0] = fast || [1] = late
    private static int s_SPerfect;
    private static int[] s_Perfect = {0, 0};
    private static int[] s_Near = {0, 0};
    private static int[] s_Miss = {0, 0};
    
    [SerializeField] TextMeshPro[] ScoreTmp;
    [SerializeField] TextMeshPro[] ComboTmp;
    [SerializeField] TextMeshPro[] JudgeCountTmp;

    private void Awake()
    {
        scoreManager = this;
    }
    
    /*//! Code For Debugging
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            print(String.Format("S-Perfect : {0} || Perfect : {1} || Near : {2} || Missed : {3}",
            s_SPerfect, s_Perfect[0] + s_Perfect[1], s_Near[0] + s_Near[1], s_Miss[0] + s_Miss[1]));
        }
    }
    //! ~Debugging
    */
    
    public static void ApplyJudge(int _judgeIndex, bool _isFast = false)
    {

        int _FastLateIndex;
        if (_isFast) { _FastLateIndex = 0; }
        else { _FastLateIndex = 1; }

        switch (_judgeIndex)
        {
            //* 세부퍼펙
            case 0:
                s_SPerfect++;
                scoreManager.JudgeCountTmp[0].text = String.Format("{0:D4}", s_SPerfect);
                break;
            
            //* 일반퍼펙
            case 1:
                s_Perfect[_FastLateIndex]++;
                scoreManager.JudgeCountTmp[1].text
                    = String.Format("{0:D4}", s_Perfect[0] + s_Perfect[1]);
                break;
            
            //* 간접판정
            case 2:
                s_Near[_FastLateIndex]++;
                scoreManager.JudgeCountTmp[2].text
                    = String.Format("{0:D4}", s_Near[0] + s_Near[1]);
                break;
            
            //* 미스
            case 3:
                s_Miss[_FastLateIndex]++;
                scoreManager.JudgeCountTmp[3].text
                    = String.Format("{0:D4}", s_Miss[0] + s_Miss[1]);
                break;

            default: return;
        }

        s_playScore = s_SPerfect + Mathf.CeilToInt((((s_SPerfect + s_Perfect[0] + s_Perfect[1]) 
            + ((s_Near[0] + s_Near[1]) * 0.5f)) / s_fullCombo) * 100000000);

        char[] _charArr;
        _charArr = String.Format("{0:D9}", s_playScore).ToCharArray();

        for (int i = 0; i < 9; i++)
        {
            scoreManager.ScoreTmp[i].text = _charArr[i].ToString();
        }
    }
    public static void ApplyCombo(bool _isPass)
    {
        if (!_isPass)
        {
            s_nowCombo = 0;
            foreach (TextMeshPro tmp in scoreManager.ComboTmp)
            {
                tmp.text = "0";
            }
        }
        else
        {
            s_nowCombo++;
            char[] _charArr;
            _charArr = String.Format("{0:D4}", s_nowCombo).ToCharArray();
            for (int i = 0; i < 4; i++)
            {
                scoreManager.ComboTmp[i].text = _charArr[i].ToString();
            }
        }
    }
    public static void ResetGamePlay()
    {
        s_playScore = 0;

        s_maxCombo = 0;
        s_nowCombo = 0;

        s_SPerfect = 0;
        s_Perfect = new int[2] { 0, 0 };
        s_Near = new int[2] { 0, 0 };
        s_Miss = new int[2] { 0, 0 };

        foreach (TextMeshPro _tmp in scoreManager.ScoreTmp) { _tmp.text = "0"; }
        foreach (TextMeshPro _tmp in scoreManager.ComboTmp) { _tmp.text = "0"; }
        foreach (TextMeshPro _tmp in scoreManager.JudgeCountTmp) { _tmp.text = "0"; }
    }
    public static void SetGameInfo(int _fullCombo)
    {
        s_fullCombo = _fullCombo;
    }
}
