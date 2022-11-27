using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private List<string> testList;

    public double GetAverage()
    {
        //* return에 필요한 변수 선언
        int _count = 0;
        double _value = 0.0f;

        for (int i = 0; i < testList.Count; i++)
        {
            try
            {
                //* List값을 double로 변환 시도
                //* 변환에 성공했다면 value값에 더한 후 카운트에 +1
                _value += Convert.ToDouble(testList[i]);
                _count++;
            }
            //* 예외처리
            catch { ; }
        }

        //* count가 0일 경우 예외처리
        //* 0이 아니라면 평균값을 return
        if (_count == 0) { return 0.0; }
        else { return _value / _count; }
    }
}
