using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveLoad : MonoBehaviour
{
    public static SaveLoad saveLoad;

    NoteSavedData noteSaved = new NoteSavedData();

    List<GameObject> listObject;

    [SerializeField]
    GameObject[] PrefabObject;

    [SerializeField]
    TMP_InputField inputBpm;

    [SerializeField]
    TMP_InputField inputStartDelayMs;

    [SerializeField]
    TMP_InputField inputFileName;

    [SerializeField]
    GameObject NoteField;

    private void Awake()
    {
        saveLoad = this;
    }

    private void Start()
    {
        ResetSavedData();
    }

    [ContextMenu("Save")]
    void SaveDataToJson()
    {
        ResetSavedData();

        try
        {
            noteSaved.bpm = Convert.ToSingle(inputBpm.text);
        }
        catch { return; }

        try
        {
            noteSaved.startDelayMs = Convert.ToInt32(inputStartDelayMs.text);
        }
        catch { return; }

        if (noteSaved.bpm == 0) return;

        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            listObject.Add(NoteField.transform.GetChild(i).gameObject);
        }

        listObject.Sort(delegate (GameObject A, GameObject B)
        {
            if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
            else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
            return 0;
        });

        for (int i = 0; i < listObject.Count; i++)
        {
            GameObject gameObject;
            gameObject = listObject[i];

            if (gameObject.tag == "chip" || gameObject.tag == "btChip")
            {
                noteSaved.Note_legnth.Add(0);
            }
            else
            {
                int legnth;
                legnth = (int)(NoteField.transform.GetChild(i).localScale.y / 100);
                noteSaved.Note_legnth.Add(legnth);
            }

            int ms;
            ms = (int)(150 * gameObject.transform.localPosition.y / Convert.ToSingle(inputBpm.text));
            noteSaved.Note_ms.Add(ms);

            switch (gameObject.transform.localPosition.x)
            {
                case -300:
                    noteSaved.Note_line.Add(1);
                    break;

                case -100:
                    noteSaved.Note_line.Add(2);
                    break;

                case +100:
                    noteSaved.Note_line.Add(3);
                    break;

                case +300:
                    noteSaved.Note_line.Add(4);
                    break;

                case 0:
                    noteSaved.Note_line.Add(5);
                    break;
            }
        }

        try
        {
            string jsonData = JsonUtility.ToJson(noteSaved, true);
            string path = Path.Combine(Application.dataPath, inputFileName.text + ".json");
            File.WriteAllText(path, jsonData);
        }
        catch { return; }
    }

    [ContextMenu("Load")]
    IEnumerator LoadDataFromJson()
    {
        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            Destroy(NoteField.transform.GetChild(i).gameObject);
        }

        ResetSavedData();
        try
        {
            string path = Path.Combine(Application.dataPath, inputFileName.text + ".json");
            string jsonData = File.ReadAllText(path);
            noteSaved = JsonUtility.FromJson<NoteSavedData>(jsonData);
        }
        catch { yield break; }

        inputBpm.text = (noteSaved.bpm).ToString();
        AutoTest.autoTest.bpm = noteSaved.bpm;
        inputStartDelayMs.text = (noteSaved.startDelayMs).ToString();
        AutoTest.autoTest.delay = noteSaved.startDelayMs;

        for (int i = 0; i < noteSaved.Note_ms.Count; i++)
        {
            GameObject copiedObject;
            Vector3 copiedObjectPos;
            copiedObjectPos = new Vector3(0, 0, 0);

            // 노트파일 해석 시작
            // 노트 X 위치와 노트의 종류 해석
            if (noteSaved.Note_line[i] == 5)
            {
                copiedObjectPos.x = 0;

                if (noteSaved.Note_legnth[i] != 0)
                {
                    copiedObject = Instantiate(PrefabObject[3], NoteField.transform);
                    copiedObject.transform.localScale = new Vector3(1, noteSaved.Note_legnth[i] * 100, 1);
                }
                else
                {
                    copiedObject = Instantiate(PrefabObject[2], NoteField.transform);
                }
            }
            else
            {
                if (noteSaved.Note_legnth[i] != 0)
                {
                    copiedObject = Instantiate(PrefabObject[1], NoteField.transform);
                    copiedObject.transform.localScale = new Vector3(1, noteSaved.Note_legnth[i] * 100, 1);
                }
                else
                {
                    copiedObject = Instantiate(PrefabObject[0], NoteField.transform);
                }

                switch (noteSaved.Note_line[i])
                {
                    case 1:
                        copiedObjectPos.x = -300;
                        break;

                    case 2:
                        copiedObjectPos.x = -100;
                        break;

                    case 3:
                        copiedObjectPos.x = +100;
                        break;

                    case 4:
                        copiedObjectPos.x = +300;
                        break;
                }
            }

            // 노트의 Y좌표 해석
            // Ms = 150 * PosY / Bpm |=>| PosY = Bpm * Ms / 150
            copiedObjectPos.y 
                = noteSaved.bpm * noteSaved.Note_ms[i] / 150;

            copiedObject.transform.localPosition = copiedObjectPos;
        }

        yield return new WaitForSeconds(.1f);
        PageSystem.pageSystem.PageSet(PageSystem.pageSystem.firstPage);
    }

    private void ResetSavedData()
    {
        noteSaved.bpm = new float();
        noteSaved.startDelayMs = new int();

        noteSaved.Note_legnth = new List<int>();
        noteSaved.Note_ms = new List<int>();
        noteSaved.Note_line = new List<int>();

        listObject = new List<GameObject>();
    }

    public void ButtonSave()
    {
        SaveDataToJson();
    }

    public void ButtonLoad()
    {
        StartCoroutine(LoadDataFromJson());
    }
}

[System.Serializable]
public class NoteSavedData
{
    public float bpm;
    public int startDelayMs;

    public List<int> Note_legnth;
    public List<int> Note_ms;
    public List<int> Note_line;
}