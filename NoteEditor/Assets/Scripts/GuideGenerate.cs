using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    readonly float[] pos_x = new float[5]{0.0f, 925.926f, 1851.852f, 2777.778f, 3703.704f };

    void Start()
    {
        GuideLineGenerate(2);
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
                    copy.transform.localPosition = new Vector3(pos_x[j], pos_y, 0);
                    copy.transform.localScale = new Vector3(1.0f, (1600 / count), 1.0f);
                }
            }
        }
    }
}
