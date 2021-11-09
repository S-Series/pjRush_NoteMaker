using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GuideGenerate : MonoBehaviour
{
    [SerializeField]
    GameObject GuideLineBox;

    [SerializeField]
    GameObject ColliderBox;

    [SerializeField]
    GameObject GuideLinePrefab;

    [SerializeField]
    GameObject ColliderPrefab;

    [SerializeField]
    TMP_InputField inputField;

    readonly float[] posX = new float[5]{0.0f, 925.926f, 1851.852f, 2777.778f, 3703.704f };

    private void Start()
    {
        GuideLineGenerate(1);
        inputField.text = "1";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GuideLineGenerate(int count)
    {
        float pos;
        try
        {
            pos = 1600 / count;
        }
        catch { pos = 0; }

        for (int i = 0; i < GuideLineBox.transform.childCount; i++)
        {
            Destroy(GuideLineBox.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < ColliderBox.transform.childCount; i++)
        {
            Destroy(ColliderBox.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                for (int l = 0; l < 4; l++)
                {
                    float pos_y;
                    GameObject copy;
                    pos_y = 1600 * l + pos * i;
                    copy = Instantiate(ColliderPrefab, ColliderBox.transform);
                    copy.transform.localPosition = new Vector3(posX[j], pos_y, 0);
                    copy.transform.localScale = new Vector3(1.0f, (1600 / count), 1.0f);
                }
            }

            float div;
            div = 1600 / count;

            for (int j = 0; j < 4; j++)
            {
                GameObject copy;
                copy = Instantiate(GuideLinePrefab, GuideLineBox.transform);
                copy.transform.localPosition = new Vector3(0.0f, div * i + 1600 * j, 0.0f);
            }
        }
    }

    // this function is triggered by Button
    public void ButtonGuideGenerate()
    {
        int count;
        try
        {
            count = Convert.ToInt32(inputField.text);
        }
        catch
        {
            count = 1;
            inputField.text = "1";
        }

        if (count >= 1 && count <= 64)
        {
            GuideLineGenerate(count);
            NoteEdit.noteEdit.guidePosSector = 1600 / count;
        }
        else
        {
            inputField.text = "1";
            return;
        }
    }
}
