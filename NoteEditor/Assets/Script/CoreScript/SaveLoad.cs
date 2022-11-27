using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveLoad : MonoBehaviour
{
    public static SaveLoad saveLoad;
    public static bool s_isWorking = false;

    static NoteSavedData noteSaved = new NoteSavedData();
    private const double editorVersion = 1.02;
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
    [SerializeField] TMP_InputField inputFileName;
    [SerializeField] GameObject NoteField;
    [SerializeField] TextMeshPro SaveCompleteMessage;
    [SerializeField] GameObject[] BlockObject;
    private void Awake()
    {
        saveLoad = this;

        if (!Directory.Exists(Application.dataPath + "/_DataBox/"))
            { Directory.CreateDirectory(Application.dataPath + "/_DataBox/"); }
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
        LineNote.Sorting();

        yield return wait;

        NoteClasses.CalculateNoteMs();

        noteSaved.Version = editorVersion;
        noteSaved.bpm = ValueManager.bpm;
        noteSaved.startDelayMs = ValueManager.delay;

        for (int i = 0; i < NormalNote.normalNotes.Count; i++)
        {
            NormalNote savingNote;
            savingNote = NormalNote.normalNotes[i];
            noteSaved.NoteMs.Add(savingNote.ms);
            noteSaved.NotePos.Add(savingNote.pos);
            noteSaved.NoteLine.Add(savingNote.line);
            noteSaved.NoteLegnth.Add(savingNote.legnth);
            noteSaved.NotePowered.Add(savingNote.isPowered);
        }
        for (int i = 0; i < SpeedNote.speedNotes.Count; i++)
        {
            SpeedNote savingNote;
            savingNote = SpeedNote.speedNotes[i];
            noteSaved.SpeedMs.Add(savingNote.ms);
            noteSaved.SpeedPos.Add(savingNote.pos);
            noteSaved.SpeedBpm.Add(savingNote.bpm);
            noteSaved.SpeedMultiple.Add(savingNote.multiply);
        }
        for (int i = 0; i < EffectNote.effectNotes.Count; i++)
        {
            EffectNote savingNote;
            savingNote = EffectNote.effectNotes[i];
            noteSaved.EffectMs.Add(savingNote.ms);
            noteSaved.EffectPos.Add(savingNote.pos);
            noteSaved.EffectForce.Add(savingNote.value);
            noteSaved.EffectIsPause.Add(savingNote.isPause);
        }
        for (int i = 0; i < LineNote.lineNotes.Count; i++)
        {
            LineNote savingNote;
            savingNote = LineNote.lineNotes[i];
            noteSaved.LineMs.Add(savingNote.ms);
            noteSaved.LinePos.Add(savingNote.pos);
            noteSaved.LinePower.Add(savingNote.startPower);
            noteSaved.LineEndPower.Add(savingNote.endPower);
            noteSaved.isLineSingle.Add(savingNote.isSingle);
        }
        for (int i = 0; i < LineTriggerNote.triggerNotes.Count; i++)
        {
            LineTriggerNote _savingNote;
            _savingNote = LineTriggerNote.triggerNotes[i];
            noteSaved.LineTriggerMs.Add(_savingNote.startMs);
            noteSaved.LineTriggerEndMs.Add(_savingNote.endMs);
            noteSaved.LineTriggerPos.Add(_savingNote.pos);
            noteSaved.LineTriggerLegnth.Add(_savingNote.legnth);
        }

        yield return wait;

        if (!Directory.Exists(Application.dataPath + "/_DataBox/"))
            { Directory.CreateDirectory(Application.dataPath + "/_DataBox/"); }
        
        if (!Directory.Exists(Application.dataPath + "/_DataBox/_MusicFile/"))
            { Directory.CreateDirectory(Application.dataPath + "/_DataBox/_MusicFile"); }

        try
        {
            string jsonData = JsonUtility.ToJson(noteSaved, true);
            string path = Application.dataPath + "/_DataBox/" + inputFileName.text + ".json";
            /*for (int i = 0; true; i++)
            {
                if (!File.Exists(path)) { break; }

                path = Application.dataPath + "/_DataBox/"
                    + inputFileName.text + "(" + i.ToString() + ").json";
            }*/
            File.WriteAllText(path, jsonData);
            PlayerPrefs.SetString("NoteFileName", inputFileName.text);
            StartCoroutine(MusicFileMessage("Save Complete", new Color32(0, 255, 0, 255)));
        }
        catch
        {
            StartCoroutine(MusicFileMessage("Save Failed", new Color32(255, 0, 0, 255)));
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
        LineEdit.DeselectNote();
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

        if (noteSaved.Version > editorVersion) { MusicFileMessage("상위버전 파일"); yield break; }
        if (noteSaved.Version < editorVersion)
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

        //try {
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
                catch { normalNote.pos = Mathf.RoundToInt(noteSaved.bpm * normalNote.ms / 150.0f); }
                try { normalNote.isPowered = noteSaved.NotePowered[i]; }
                catch { normalNote.isPowered = false; }
                NormalNote.normalNotes.Add(normalNote);
            }
            if (NormalNote.normalNotes.Count != 0)
            {
                print(NormalNote.normalNotes[0].pos);
                if (NormalNote.normalNotes[0].pos < 1600.0f)
                {
                    foreach(NormalNote _note in NormalNote.normalNotes) { _note.pos += 1600; }
                }
            }
            for (int i = 0; i < noteSaved.SpeedMs.Count; i++)
            {
                SpeedNote speedNote = new SpeedNote();
                speedNote.noteObject = null;
                speedNote.ms = noteSaved.SpeedMs[i];
                speedNote.pos = noteSaved.SpeedPos[i];
                speedNote.bpm = noteSaved.SpeedBpm[i];
                if (noteSaved.SpeedMultiple.Count != 0) 
                    { speedNote.multiply = noteSaved.SpeedMultiple[i]; }
                else { speedNote.multiply = noteSaved.SpeedNum[i]; }
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
            for (int i = 0; i < noteSaved.LineMs.Count; i++)
            {
                LineNote lineNote = new LineNote();
                lineNote.ms = noteSaved.LineMs[i];
                lineNote.pos = noteSaved.LinePos[i];
                lineNote.startPower = noteSaved.LinePower[i];
                lineNote.endPower = noteSaved.LineEndPower[i];
                lineNote.isSingle = noteSaved.isLineSingle[i];
                LineNote.lineNotes.Add(lineNote);
            }
            for (int i = 0; i < noteSaved.LineTriggerMs.Count; i++)
            {

            }
            //*--------------------------------------
            NormalNote.Sorting();
            SpeedNote.Sorting();
            EffectNote.Sorting();
            LineNote.Sorting();
            LineTriggerNote.Sorting();
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
                    copyObject = Instantiate(PrefabObject[1], NoteField.transform);
                }
                else
                {
                    if (normalNote.isPowered) 
                        { copyObject = Instantiate(PrefabObject[2], NoteField.transform); }
                    else { copyObject = Instantiate(PrefabObject[0], NoteField.transform); }
                }

                NoteOption _noteOption;
                _noteOption = copyObject.GetComponent<NoteOption>();
                _noteOption.ToLongNote(normalNote.legnth);

                //** autoPos.x
                switch (normalNote.line)
                {
                    case 1:
                        autoPos.x = -300;
                        break;

                    case 2:
                        autoPos.x = -100;
                        break;

                    case 3:
                        autoPos.x = +100;
                        break;

                    case 4:
                        autoPos.x = +300;
                        break;

                    case 5:
                        autoPos.x = -200;
                        break;

                    case 6:
                        autoPos.x = +200;
                        break;
                }
                autoPos.y = normalNote.pos;
                autoPos.z = 0;
                copyObject.transform.localPosition = autoPos;

                normalNote.noteObject = copyObject;
            }
            for (int i = 0; i < SpeedNote.speedNotes.Count; i++)
            {
                SpeedNote speedNote;
                GameObject copyObject;

                speedNote = SpeedNote.speedNotes[i];
                copyObject = Instantiate(PrefabObject[3], NoteField.transform);
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
                copyObject = Instantiate(PrefabObject[4], NoteField.transform);
                copyObject.transform.localPosition = new Vector3(0, effectNote.pos, 0);
                copyObject.transform.GetChild(0).GetChild(0).localScale
                    = new Vector3(3.25f, effectNote.value, 1.0f);
                copyObject.GetComponentInChildren<TextMeshPro>().text = effectNote.value.ToString();
                if (effectNote.isPause) copyObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                else copyObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

                effectNote.noteObject = copyObject;
            }
            for (int i = 0; i < LineNote.lineNotes.Count; i++)
            {
                LineNote targetNote;
                LineOption targetOption;
                GameObject copyObject;

                targetNote = LineNote.lineNotes[i];
                copyObject = Instantiate(PrefabObject[5], NoteField.transform);
                copyObject.transform.localPosition = new Vector3(0, targetNote.pos, 0);
                
                for (int j = 0; j < copyObject.transform.childCount; j++) 
                    { copyObject.transform.GetChild(j).gameObject.SetActive(true); }

                targetOption = copyObject.GetComponent<LineOption>();
                targetOption.Selected(false);
                targetOption.UpdateSliderInfo(targetNote);

                targetNote.noteObject = copyObject;
            }
            LineMove.ReDrewLine();
            PlayerPrefs.SetString("NoteFileName", inputFileName.text);
            s_isWorking = false;
            BlockObject[1].SetActive(false);
        /*}
        catch
        {
            ResetSavedData();
            s_isWorking = false;
            BlockObject[1].SetActive(false);
            Debug.Log("파일 오류");
        }*/
    }
    IEnumerator CreateNewJsonData()
    {
        s_isWorking = true;
        BlockObject[3].SetActive(true);
        bool isFileExist = false;
        ResetSavedData();
        try
        {
            string path;
            path = Application.dataPath + "/_DataBox/" + inputFileName.text + ".json";
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
            StartCoroutine(MusicFileMessage("Save Complete", new Color32(0, 255, 0, 255)));
        }
        catch
        {
            StartCoroutine(MusicFileMessage("Save Failed", new Color32(255, 0, 0, 255)));
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

        #region OldData --------------------------
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
        #endregion
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
    private IEnumerator MusicFileMessage(string _message, Color32? _color = null)
    {
        Color32 _textColor;
        if (!_color.HasValue) { _textColor = new Color32(255, 255, 255, 255); }
        else { _textColor = _color.Value; }

        SaveCompleteMessage.text = _message;
        SaveCompleteMessage.color = _textColor;
        yield return new WaitForSeconds(3.0f);
        SaveCompleteMessage.text = "Music File Name";
        SaveCompleteMessage.color = new Color32(255, 255, 255, 255);
    }
}

[System.Serializable]
public class NoteSavedData
{
    public double Version;
    public float bpm;
    public int startDelayMs;

    //** Normal Note -------------------------
    public List<int> NoteLegnth = new List<int>();
    public List<int> NoteMs = new List<int>();
    public List<int> NotePos = new List<int>();
    public List<int> NoteLine = new List<int>();
    public List<bool> NotePowered = new List<bool>();

    //** Speed Note -------------------------
    public List<int> SpeedMs = new List<int>();
    public List<int> SpeedPos = new List<int>();
    public List<float> SpeedBpm = new List<float>();
    public List<float> SpeedNum = new List<float>();
    public List<float> SpeedMultiple = new List<float>();

    //** Effect Note -------------------------
    public List<int> EffectMs = new List<int>();
    public List<int> EffectPos = new List<int>();
    public List<int> EffectForce = new List<int>();
    public List<bool> EffectIsPause = new List<bool>();

    //** Line Note -------------------------
    public List<int> LineMs = new List<int>();
    public List<int> LinePos = new List<int>();
    public List<int> LinePower = new List<int>();
    public List<int> LineEndPower = new List<int>();
    public List<bool> isLineSingle = new List<bool>();

    //** Line Trigger Note -------------------------
    public List<int> LineTriggerMs = new List<int>();
    public List<int> LineTriggerEndMs = new List<int>();
    public List<int> LineTriggerPos = new List<int>();
    public List<int> LineTriggerLegnth = new List<int>();
}
