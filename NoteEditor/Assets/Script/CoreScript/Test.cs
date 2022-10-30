using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private IEnumerator LongCoroutine;
    private void Start()
    {
        LongCoroutine = ITest();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            StopCoroutine(LongCoroutine);
        }
        if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            LongCoroutine = ITest();
            StartCoroutine(LongCoroutine);
        }
    }
    IEnumerator ITest()
    {
        yield return new WaitForSeconds(1.0f);
        print("2");
    }
}
