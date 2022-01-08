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

    [SerializeField]
    GameObject StaticGuide;
    GameObject StaticGuideParent;

    readonly float[] posX = new float[5]{0.0f, 925.926f, 1851.852f, 2777.778f, 3703.704f };

    private void Awake()
    {
        StaticGuideParent = StaticGuide.transform.parent.gameObject;
        for (int i = 1; i < 9; i++)
        {
            GameObject copy;
            copy = Instantiate(StaticGuide, StaticGuideParent.transform);
            copy.transform.localPosition = new Vector3(0, 1600 * i, 0);
            copy.transform.GetChild(0).GetComponent<TextMeshPro>().text = "00" + (i + 1).ToString();
        }
        for (int i = 9; i < 99; i++)
        {
            GameObject copy;
            copy = Instantiate(StaticGuide, StaticGuideParent.transform);
            copy.transform.localPosition = new Vector3(0, 1600 * i, 0);
            copy.transform.GetChild(0).GetComponent<TextMeshPro>().text = "0" + (i + 1).ToString();
        }
        for (int i = 99; i < 999; i++)
        {
            GameObject copy;
            copy = Instantiate(StaticGuide, StaticGuideParent.transform);
            copy.transform.localPosition = new Vector3(0, 1600 * i, 0);
            copy.transform.GetChild(0).GetComponent<TextMeshPro>().text = (i + 1).ToString();
        }
        StaticGuideParent.SetActive(false);
    }

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

            for (int j = 0; j < 4; j++)
            {
                float posY;
                posY = (1600 * i) / count + 1600 * j;
                GameObject copy;
                copy = Instantiate(GuideLinePrefab, GuideLineBox.transform);
                copy.transform.localPosition = new Vector3(0.0f, posY, 0.0f);
                if ((int)(posY % 400) == 0)
                {
                    copy.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 150);
                }
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
            NoteEdit.noteEdit.guidePosSector = count;
        }
        else
        {
            GuideLineGenerate(1);
            inputField.text = "1";
            return;
        }
    }
}
