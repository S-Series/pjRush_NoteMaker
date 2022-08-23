using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveLoad : MonoBehaviour
{
    public static SaveLoad saveLoad;
    public static bool s_isWorking = false;

    static NoteSavedData noteSaved = new NoteSavedData();
    private const string editorVersion = "0.9.11.hotfix_D";
    private float bpm;
    private static bool isSaving = false;
    private static bool isLoading = false;

    #region Old Save Data ----------------------------------
    private List<GameObject> note;
    private List<float> noteMs;
    private List<float> notePos;
    private List<int> noteLine;
    private List<int> noteLegnth;
    private List<bool> notePowered;


    private List<GameObject> effect;
    private List<float> EffectMs;
    private List<float> EffectPos;
    private List<float> EffectForce;
    private List<int> EffectDuration;

    private List<GameObject> speed;
    private List<float> SpeedMs;
    private List<float> SpeedPos;
    private List<float> SpeedBpm;
    #endregion

    #region New Save Data ----------------------------------
    #endregion
    [SerializeField] GameObject[] PrefabObject;
    [SerializeField] TMP_InputField inputBpm;
    [SerializeField] TMP_InputField inputStartDelayMs;
    [SerializeField] TMP_InputField inputFileName;
    [SerializeField] GameObject NoteField;
    [SerializeField] TextMeshPro SaveCompleteMessage;
    [SerializeField] GameObject[] BlockObject;
    private void Awake()
    {
        saveLoad = this;
    }
    private void Start()
    {
        ResetSavedData();
        StartCoroutine(StartLoad(PlayerPrefs.GetString("NoteFileName")));
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                ButtonSave();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (Input.GetKey(KeyCode.S))
            {
                ButtonSave();
            }
        }
    }

    [ContextMenu("Save")]
    IEnumerator SaveDataToJson()
    {
        s_isWorking = true;
        BlockObject[0].SetActive(true);
        var wait = new WaitForSeconds(0.5f);

        ResetSavedData();

        NormalNote.Sorting();
        SpeedNote.Sorting();
        EffectNote.Sorting();
        yield return wait;
        NoteClasses.CalculateNoteMs();

        noteSaved.Version = editorVersion;
        noteSaved.bpm = ValueManager.bpm;
        noteSaved.startDelayMs = ValueManager.delay;

        for (int i = 0; i < NormalNote.normalNotes.Count; i++)
        {
            NormalNote savingNotes;
            savingNotes = NormalNote.normalNotes[i];
            noteSaved.NoteMs.Add(savingNotes.ms);
            noteSaved.NotePos.Add(savingNotes.pos);
            noteSaved.NoteLine.Add(savingNotes.line);
            noteSaved.NoteLegnth.Add(savingNotes.legnth);
            noteSaved.NotePowered.Add(savingNotes.isPowered);
            yield return null;
        }
        for (int i = 0; i < SpeedNote.speedNotes.Count; i++)
        {
            SpeedNote savingNotes;
            savingNotes = SpeedNote.speedNotes[i];
            noteSaved.SpeedMs.Add(savingNotes.ms);
            noteSaved.SpeedPos.Add(savingNotes.pos);
            noteSaved.SpeedBpm.Add(savingNotes.bpm);
            noteSaved.SpeedNum.Add(savingNotes.multiply);
            yield return null;
        }
        for (int i = 0; i < EffectNote.effectNotes.Count; i++)
        {
            EffectNote savingNotes;
            savingNotes = EffectNote.effectNotes[i];
            noteSaved.EffectMs.Add(savingNotes.ms);
            noteSaved.EffectPos.Add(savingNotes.pos);
            noteSaved.EffectForce.Add(savingNotes.value);
            noteSaved.EffectIsPause.Add(savingNotes.isPause);
            yield return null;
        }

        yield return wait;

        try
        {
            string jsonData = JsonUtility.ToJson(noteSaved, true);
            string path = Application.dataPath + "/_DataBox/" + inputFileName.text + ".json";
            print(path);
            File.WriteAllText(path, jsonData);
            PlayerPrefs.SetString("NoteFileName", inputFileName.text);
            StartCoroutine(DisplaySaveCompleteMessage(true));
        }
        catch
        {
            StartCoroutine(DisplaySaveCompleteMessage(false));
        }
        s_isWorking = false;
        BlockObject[0].SetActive(false);
    }

    [ContextMenu("Load")]
    IEnumerator LoadDataFromJson()
    {
        s_isWorking = true;
        BlockObject[1].SetActive(true);

        ResetSavedData();
        NoteClasses.ResetNotes();
        try
        {
            string path = Application.dataPath + "/_DataBox/" + inputFileName.text + ".json";
            string jsonData = File.ReadAllText(path);
            print(path);
            noteSaved = JsonUtility.FromJson<NoteSavedData>(jsonData);
        }
        catch
        {
            s_isWorking = false;
            BlockObject[1].SetActive(false);
            yield break;
        }

        if (noteSaved.Version != editorVersion)
        {
            BlockObject[1].SetActive(false);
            BlockObject[2].SetActive(true);
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    BlockObject[1].SetActive(true);
                    BlockObject[2].SetActive(false);
                    break;
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    s_isWorking = false;
                    BlockObject[2].SetActive(false);
                    yield break;
                }
                yield return null;
            }
        }

        try
        {
            ValueManager.bpm = noteSaved.bpm;
            ValueManager.delay = noteSaved.startDelayMs;
            ValueManager.DisplayValue();

            for (int i = 0; i < NoteField.transform.childCount; i++)
                { Destroy(NoteField.transform.GetChild(i).gameObject); }

            NoteClasses.ResetNotes();
            for (int i = 0; i < noteSaved.NoteMs.Count; i++)
            {
                NormalNote normalNote = new NormalNote();
                normalNote.noteObject = null;
                normalNote.ms = noteSaved.NoteMs[i];
                normalNote.line = noteSaved.NoteLine[i];
                normalNote.legnth = noteSaved.NoteLegnth[i];
                try { normalNote.pos = noteSaved.NotePos[i]; }
                catch { normalNote.pos = noteSaved.bpm * normalNote.ms / 150.0f; }
                try { normalNote.isPowered = noteSaved.NotePowered[i]; }
                catch { normalNote.isPowered = false; }
                NormalNote.normalNotes.Add(normalNote);
            }
            if (NormalNote.normalNotes.Count != 0)
            {
                print(NormalNote.normalNotes[0].pos);
                if (NormalNote.normalNotes[0].pos < 1600.0f)
                {
                    foreach(NormalNote _note in NormalNote.normalNotes) { _note.pos += 1600.0f; }
                }
            }
            for (int i = 0; i < noteSaved.SpeedMs.Count; i++)
            {
                SpeedNote speedNote = new SpeedNote();
                speedNote.noteObject = null;
                speedNote.ms = noteSaved.SpeedMs[i];
                speedNote.pos = noteSaved.SpeedPos[i];
                speedNote.bpm = noteSaved.SpeedBpm[i];
                speedNote.multiply = noteSaved.SpeedNum[i];
                SpeedNote.speedNotes.Add(speedNote);
            }
            for (int i = 0; i < noteSaved.EffectMs.Count; i++)
            {
                EffectNote effectNote = new EffectNote();
                effectNote.noteObject = null;
                effectNote.ms = noteSaved.EffectMs[i];
                effectNote.pos = noteSaved.EffectPos[i];
                effectNote.isPause = noteSaved.EffectIsPause[i];
                effectNote.value = noteSaved.EffectForce[i];
                EffectNote.effectNotes.Add(effectNote);
            }
            //*--------------------------------------
            NormalNote.Sorting();
            SpeedNote.Sorting();
            EffectNote.Sorting();
            //*--------------------------------------
            for (int i = 0; i < NormalNote.normalNotes.Count; i++)
            {
                NormalNote normalNote;
                GameObject copyObject;

                Vector3 autoPos = new Vector3(0, 0, 0);
                Vector3 autoScale = new Vector3(1, 1, 1);

                normalNote = NormalNote.normalNotes[i];
                if (normalNote.line >= 5)
                {
                    if (normalNote.legnth == 0)
                    {
                        copyObject = Instantiate(PrefabObject[2], NoteField.transform);
                    }
                    else
                    {
                        copyObject = Instantiate(PrefabObject[3], NoteField.transform);
                        Vector3 scale;
                        scale = copyObject.transform.localScale;
                        scale.y = 100.0f * normalNote.legnth;
                        copyObject.transform.localScale = scale;
                    }
                }
                else
                {
                    if (normalNote.legnth == 0)
                    {
                        copyObject = Instantiate(PrefabObject[0], NoteField.transform);
                        copyObject.transform.GetChild(0).gameObject.SetActive(normalNote.isPowered);
                        //copyObject.GetComponent<SpriteRenderer>().enabled = !normalNote.isPowered;
                    }
                    else
                    {
                        copyObject = Instantiate(PrefabObject[1], NoteField.transform);
                        Vector3 scale;
                        scale = copyObject.transform.localScale;
                        scale.y = 100.0f * normalNote.legnth;
                        copyObject.transform.localScale = scale;
                    }
                }

                switch (normalNote.line)
                {
                    case 1:
                        autoPos.x = -300;
                        autoPos.z = 0;
                        break;

                    case 2:
                        autoPos.x = -100;
                        autoPos.z = 0;
                        break;

                    case 3:
                        autoPos.x = +100;
                        autoPos.z = 0;
                        break;

                    case 4:
                        autoPos.x = +300;
                        autoPos.z = 0;
                        break;

                    case 5:
                        autoPos.x = -200;
                        autoPos.z = -.01f;
                        break;

                    case 6:
                        autoPos.x = +200;
                        autoPos.z = -.01f;
                        break;
                }
                autoPos.y = normalNote.pos;
                copyObject.transform.localPosition = autoPos;

                normalNote.noteObject = copyObject;
            }
            for (int i = 0; i < SpeedNote.speedNotes.Count; i++)
            {
                SpeedNote speedNote;
                GameObject copyObject;

                speedNote = SpeedNote.speedNotes[i];
                copyObject = Instantiate(PrefabObject[4], NoteField.transform);
                copyObject.GetComponentInChildren<TextMeshPro>().text
                    = string.Format("{0:F2}", speedNote.bpm) + "  x  " + string.Format("{0:F1}", speedNote.multiply);
                copyObject.transform.localPosition = new Vector3(0, speedNote.pos, 0);

                speedNote.noteObject = copyObject;
            }
            for (int i = 0; i < EffectNote.effectNotes.Count; i++)
            {
                EffectNote effectNote;
                GameObject copyObject;

                effectNote = EffectNote.effectNotes[i];
                copyObject = Instantiate(PrefabObject[5], NoteField.transform);
                copyObject.transform.localPosition = new Vector3(0, effectNote.pos, 0);
                copyObject.transform.GetChild(0).GetChild(0).localScale
                    = new Vector3(3.25f, effectNote.value, 1.0f);
                copyObject.GetComponentInChildren<TextMeshPro>().text = effectNote.value.ToString();
                if (effectNote.isPause) copyObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                else copyObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

                effectNote.noteObject = copyObject;
            }
            PlayerPrefs.SetString("NoteFileName", inputFileName.text);
            s_isWorking = false;
            BlockObject[1].SetActive(false);
        }
        catch
        {
            ResetSavedData();
            s_isWorking = false;
            BlockObject[1].SetActive(false);
            Debug.Log("파일 오류");
        }
    }
    IEnumerator CreateNewJsonData()
    {
        s_isWorking = true;
        BlockObject[3].SetActive(true);
        bool isFileExist = false;
        ResetSavedData();
        try
        {
            string path = Path.Combine(Application.dataPath, inputFileName.text + ".json");
            string jsonData = File.ReadAllText(path);
            noteSaved = JsonUtility.FromJson<NoteSavedData>(jsonData);
            isFileExist = true;
        }
        catch { isFileExist = false; }

        while (isFileExist)
        {
            BlockObject[3].SetActive(false);
            BlockObject[4].SetActive(true);
            if (Input.GetKeyDown(KeyCode.Return))
            {
                BlockObject[3].SetActive(true);
                BlockObject[4].SetActive(false);
                break;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                s_isWorking = false;
                BlockObject[3].SetActive(false);
                BlockObject[4].SetActive(false);
                yield break;
            }
            yield return null;
        }

        ResetSavedData();
        noteSaved.bpm = 120;
        noteSaved.Version = editorVersion;
        try
        {
            string jsonData = JsonUtility.ToJson(noteSaved, true);
            string path = Path.Combine(Application.dataPath, inputFileName.text + ".json");
            File.WriteAllText(path, jsonData);
            PlayerPrefs.SetString("NoteFileName", inputFileName.text);
            StartCoroutine(DisplaySaveCompleteMessage(true));
        }
        catch
        {
            StartCoroutine(DisplaySaveCompleteMessage(false));
        }
        BlockObject[3].SetActive(false);
        yield return SaveDataToJson();
    }
    private IEnumerator StartLoad(string fileName)
    {
        if (fileName == "") {yield break;}

        s_isWorking = true;
        inputFileName.text = fileName;
        BlockObject[5].SetActive(true);
        while(true)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                BlockObject[5].SetActive(false);
                yield return LoadDataFromJson();
                break;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                BlockObject[5].SetActive(false);
                inputFileName.text = "";
                break;
            }
            yield return null;
        }
    }
    private void ResetSavedData()
    {
        noteSaved = new NoteSavedData();
        NoteField = PageSystem.pageSystem.NoteField;

        note = new List<GameObject>();
        effect = new List<GameObject>();
        speed = new List<GameObject>();

        noteMs = new List<float>();
        notePos = new List<float>();
        noteLine = new List<int>();
        noteLegnth = new List<int>();
        notePowered = new List<bool>();

        effect = new List<GameObject>();
        EffectMs = new List<float>();
        EffectForce = new List<float>();
        EffectDuration = new List<int>();

        speed = new List<GameObject>();
        SpeedMs = new List<float>();
        SpeedPos = new List<float>();
        SpeedBpm = new List<float>();

        SpeedMs = new List<float>();
        SpeedBpm = new List<float>();
        SpeedPos = new List<float>();
    }
    public void ButtonSave()
    {
        StartCoroutine(SaveDataToJson());
    }
    public void ButtonLoad()
    {
        StartCoroutine(LoadDataFromJson());
    }
    public void ButtonCreate()
    {
        StartCoroutine(CreateNewJsonData());
    }
    private IEnumerator DisplaySaveCompleteMessage(bool success)
    {
        if (success)
        {
            SaveCompleteMessage.text = "Save Completed";
            SaveCompleteMessage.color = new Color32(0, 255, 0, 255);
            yield return new WaitForSeconds(3.0f);
            SaveCompleteMessage.text = "Music File Name";
            SaveCompleteMessage.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            SaveCompleteMessage.text = "Save Failed";
            SaveCompleteMessage.color = new Color32(255, 0, 0, 255);
            yield return new WaitForSeconds(3.0f);
            SaveCompleteMessage.text = "Music File Name";
            SaveCompleteMessage.color = new Color32(255, 255, 255, 255);
        }
    }
}

[System.Serializable]
public class NoteSavedData
{
    public string Version;
    public float bpm;
    public int startDelayMs;

    public List<int> NoteLegnth = new List<int>();
    public List<float> NoteMs = new List<float>();
    public List<float> NotePos = new List<float>();
    public List<int> NoteLine = new List<int>();
    public List<bool> NotePowered = new List<bool>();

    public List<float> EffectMs = new List<float>();
    public List<float> EffectPos = new List<float>();
    public List<float> EffectForce = new List<float>();
    public List<bool> EffectIsPause = new List<bool>();

    public List<float> SpeedMs = new List<float>();
    public List<float> SpeedPos = new List<float>();
    public List<float> SpeedBpm = new List<float>();
    public List<float> SpeedNum = new List<float>();
}
