using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMove : MonoBehaviour
{
    private static LineMove s_LM;
    public static int s_LineNum = 1;
    private static readonly string[] s_Triggers = { "", "" };
    [SerializeField] private GameObject[] LineGameObject = new GameObject[4];
    [SerializeField] private Transform GenerateField;
    [SerializeField] private Animator LineMoveAnimate;
    private GameObject[] CopyLineObject = new GameObject[4];

    private void Awake()
    {
        for (int i = 0; i < GenerateField.childCount; i++)
        {
            Destroy(GenerateField.GetChild(i).gameObject);
        }
    }
    public static void GenerateCopyLine()
    {

    }
    public void LineMovement(bool _isLeft = false, bool _isRight = false)
    {
        if (_isLeft == _isRight) { return; }

        int _LineIndex;

        if (_isLeft)
        {
            _LineIndex = s_LineNum;
            //** 2개가 보여야하는 라인을 활성화
            CopyLineObject[_LineIndex].SetActive(true);

            //** 5개의 라인의 로컬 좌표를 설정하고
            
            //** 현재 BPM에 따라, 애니메이션 속도 설정
            LineMoveAnimate.speed = 1.0f;

            //** 애니메이션을 플레이한 뒤
            LineMoveAnimate.SetTrigger(s_Triggers[0]);

        }
    }
}
