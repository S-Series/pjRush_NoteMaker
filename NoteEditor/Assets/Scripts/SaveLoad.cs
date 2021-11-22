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
    List<GameObject> effectObject;
    List<GameObject> bpmObject;

    [SerializeField]
    GameObject[] PrefabObject;

    [SerializeField]
    TMP_InputField inputBpm;

    [SerializeField]
    TMP_InputField inputStartDelayMs;

    [SerializeField]
    TMP_InputField inputFileName;

    [SerializeField]
    TextMeshProUGUI dropdownDifficulty;

    [SerializeField]
    GameObject NoteField;

    private void Awake()
    {
        saveLoad = this;
    }

    private void Start()
    {
        ResetSavedData();

        try
        {
            inputFileName.text = PlayerPrefs.GetString("NoteFileName");
            StartCoroutine(LoadDataFromJson());
        }
        catch
        {
            inputFileName.text = "";
            PlayerPrefs.SetString("NoteFileName", "");
        }
    }

    [ContextMenu("Save")]
    void SaveDataToJson()
    {
        float gameBpm;
        gameBpm = AutoTest.autoTest.bpm;

        noteSaved.difficulty = Convert.ToInt32(dropdownDifficulty.text);

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
            GameObject ChildObject;
            ChildObject = NoteField.transform.GetChild(i).gameObject;

            if (ChildObject.tag == "Effect") effectObject.Add(ChildObject);
            else if (ChildObject.tag == "Bpm") bpmObject.Add(ChildObject);
            else listObject.Add(ChildObject);
        }

        if (listObject.Count >= 2)
        {
            listObject.Sort(delegate (GameObject A, GameObject B)
            {
                if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
                else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
                return 0;
            });
        }

        if (bpmObject.Count >= 2)
        {
            bpmObject.Sort(delegate (GameObject A, GameObject B)
            {
                if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
                else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
                return 0;
            });
        }

        if (effectObject.Count >= 2)
        {
            effectObject.Sort(delegate (GameObject A, GameObject B)
            {
                if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
                else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
                return 0;
            });
        }

        for (int i = 0; i < listObject.Count; i++)
        {
            GameObject gameObject;
            gameObject = listObject[i];

            if (gameObject.tag == "chip" || gameObject.tag == "btChip")
            {
                noteSaved.NoteLegnth.Add(0);
            }
            else
            {
                int legnth;
                legnth = (int)(listObject[i].transform.localScale.y / 100);
                Debug.Log(legnth);
                noteSaved.NoteLegnth.Add(legnth);
            }

            int ms;
            ms = (int)(150 * gameObject.transform.localPosition.y / gameBpm);
            noteSaved.NoteMs.Add(ms);

            switch (gameObject.transform.localPosition.x)
            {
                case -300:
                    noteSaved.NoteLine.Add(1);
                    break;

                case -100:
                    noteSaved.NoteLine.Add(2);
                    break;

                case +100:
                    noteSaved.NoteLine.Add(3);
                    break;

                case +300:
                    noteSaved.NoteLine.Add(4);
                    break;

                case 0:
                    noteSaved.NoteLine.Add(5);
                    break;
            }
        }

        for (int i = 0; i < effectObject.Count; i++)
        {
            GameObject gameObject;
            gameObject = effectObject[i];

            int ms;
            ms = (int)(150 * gameObject.transform.localPosition.y / gameBpm);
            noteSaved.EffectMs.Add(ms);

            float force;
            force = Convert.ToSingle(gameObject.name);
            noteSaved.EffectForce.Add(force);
        }

        for (int i = 0; i < bpmObject.Count; i++)
        {
            GameObject gameObject;
            gameObject = bpmObject[i];

            int ms;
            ms = (int)(150 * gameObject.transform.localPosition.y / gameBpm);
            noteSaved.SpeedMs.Add(ms);

            float bpm;
            bpm = Convert.ToSingle(gameObject.name);
            noteSaved.EffectForce.Add(bpm);
        }

        try
        {
            string jsonData = JsonUtility.ToJson(noteSaved, true);
            string path = Path.Combine(Application.dataPath, inputFileName.text + ".json");
            File.WriteAllText(path, jsonData);
        }
        catch { return; }

        PlayerPrefs.SetString("NoteFileName", inputFileName.text);
    }

    [ContextMenu("Load")]
    IEnumerator LoadDataFromJson()
    {
        yield return new WaitForSeconds(.1f);

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

        for (int i = 0; i < noteSaved.NoteMs.Count; i++)
        {
            GameObject copiedObject;
            Vector3 copiedObjectPos;
            copiedObjectPos = new Vector3(0, 0, 0);

            // 노트파일 해석 시작
            // 노트 X 위치와 노트의 종류 해석
            if (noteSaved.NoteLine[i] == 5)
            {
                copiedObjectPos.x = 0;

                if (noteSaved.NoteLegnth[i] != 0)
                {
                    copiedObject = Instantiate(PrefabObject[3], NoteField.transform);
                    copiedObjectPos.z = 0.003f;
                    copiedObject.transform.localScale = new Vector3(1, noteSaved.NoteLegnth[i] * 100, 1);
                }
                else
                {
                    copiedObjectPos.z = 0.001f;
                    copiedObject = Instantiate(PrefabObject[2], NoteField.transform);
                }
            }
            else
            {
                if (noteSaved.NoteLegnth[i] != 0)
                {
                    copiedObject = Instantiate(PrefabObject[1], NoteField.transform);
                    copiedObjectPos.z = 0.002f;
                    copiedObject.transform.localScale = new Vector3(1, noteSaved.NoteLegnth[i] * 100, 1);
                }
                else
                {
                    copiedObject = Instantiate(PrefabObject[0], NoteField.transform);
                }

                switch (noteSaved.NoteLine[i])
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

                    case 5:
                        copiedObjectPos.x = 0;
                        break;
                }
            }

            // 노트의 Y좌표 해석
            // Ms = 150 * PosY / Bpm |=>| PosY = Bpm * Ms / 150
            copiedObjectPos.y 
                = noteSaved.bpm * noteSaved.NoteMs[i] / 150;

            copiedObject.transform.localPosition = copiedObjectPos;
        }

        yield return new WaitForSeconds(.1f);
        PageSystem.pageSystem.PageSet(PageSystem.pageSystem.firstPage);

        PlayerPrefs.SetString("NoteFileName", inputFileName.text);
    }

    private void ResetSavedData()
    {
        NoteField = PageSystem.pageSystem.NoteField;

        noteSaved.bpm = new float();
        noteSaved.startDelayMs = new int();

        noteSaved.NoteLegnth = new List<int>();
        noteSaved.NoteMs = new List<int>();
        noteSaved.NoteLine = new List<int>();

        noteSaved.EffectMs = new List<int>();
        noteSaved.EffectForce = new List<float>();
        noteSaved.EffectDuration = new List<int>();

        noteSaved.SpeedMs = new List<int>();
        noteSaved.SpeedBpm = new List<float>();

        listObject = new List<GameObject>();
        effectObject = new List<GameObject>();
        bpmObject = new List<GameObject>();
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
    public int difficulty;
    public int startDelayMs;

    public List<int> NoteLegnth;
    public List<int> NoteMs;
    public List<int> NoteLine;

    public List<int> EffectMs;
    public List<float> EffectForce;
    public List<int> EffectDuration;

    public List<int> SpeedMs;
    public List<float> SpeedBpm;
}