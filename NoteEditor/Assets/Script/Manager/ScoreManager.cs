using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshPro[] ScoreTmp;
    [SerializeField] TextMeshPro[] ComboTmp;
    // noteScoreCount = total_noteCount * 12;
    private int testPlayScore = 0;
    public static int noteScoreCount = 0;
    private int notePlayScoreCount = 0;
    [SerializeField] private int combo = 0;
    private int comboIndex = 0;
    public void ResetCombo(){
        combo = 0;
        comboIndex = 0;
        for (int i = 0; i < 4; i++){
            ComboTmp[i].text = "0";
            ComboTmp[i].color = new Color32(255,255,255,100);
        }
    }
    public void ResetScore(){
        ResetCombo();
        StopAllCoroutines();
        testPlayScore = 0;
        notePlayScoreCount = 0;
    }
    public void scoreJudgeType(bool isPerfect){
        if (isPerfect){
            StartCoroutine(IAddScore(12));
        }
        else{
            StartCoroutine(IAddScore(6));
        }
        combo++;
        if (combo >= 9999){
            ComboTmp[0].text = "M";
            ComboTmp[1].text = "a";
            ComboTmp[2].text = "x";
            ComboTmp[3].text = "+";
            return;
        }
        if (combo >= Mathf.Pow(10, comboIndex)){
            ComboTmp[3 - comboIndex].color = new Color32(255,255,255,255);
            comboIndex++;
        }
        
        char[] comboString = (string.Format("{0:D4}", combo)).ToCharArray();
        for (int i = 0; i < 4; i++){
            ComboTmp[i].text = comboString[i].ToString();
        }
    }
    private IEnumerator IAddScore(int count){
        var wait = new WaitForSeconds(0.125f);
        for (int i = 0; i < count; i++){
            notePlayScoreCount++;
            DisplayScore();
            yield return wait;
        }
    }
    private void DisplayScore(){
        int score;
        score = (int)(100000000 * (Convert.ToDouble(notePlayScoreCount) / noteScoreCount));
        char[] scoreText = (string.Format("{0:D9}", score)).ToCharArray();
        for (int i = 0; i < 9; i++){
            ScoreTmp[i].text = scoreText[i].ToString();
        }
    }
}
