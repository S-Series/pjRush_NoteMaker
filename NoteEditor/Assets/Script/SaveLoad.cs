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
    private const string editorVersion = "0.9.7";
    private float bpm;
    private bool isLoopInLoad;
    private bool isBreakInLoad;

    private List<GameObject> note;
    private List<float> noteMs;
    private List<float> notePos;
    private List<int> noteLine;
    private List<int> noteLegnth;
    private List<bool> noteDouble;


    private List<GameObject> effect;
    private List<float> EffectMs;
    private List<float> EffectPos;
    private List<float> EffectForce;
    private List<int> EffectDuration;

    private List<GameObject> speed;
    private List<float> SpeedMs;
    private List<float> SpeedPos;
    private List<float> SpeedBpm;

    [SerializeField] GameObject[] PrefabObject;
    [SerializeField] TMP_InputField inputBpm;
    [SerializeField] TMP_InputField inputStartDelayMs;
    [SerializeField] TMP_InputField inputFileName;
    [SerializeField] TextMeshProUGUI dropdownDifficulty;
    [SerializeField] GameObject NoteField;
    [SerializeField] TextMeshPro SaveCompleteMessage;
    [SerializeField] GameObject SaveBlock;
    [SerializeField] GameObject LoadBlock;
    [SerializeField] GameObject CreateBlock;
    private void Awake()
    {
        saveLoad = this;
        SaveBlock.SetActive(false);
        LoadBlock.SetActive(false);
        CreateBlock.SetActive(false);
    }
    private void Start() {
        ResetSavedData();
        
        try{
            inputFileName.text = PlayerPrefs.GetString("NoteFileName");
            StartCoroutine(LoadDataFromJson());
        }
        catch{
            inputFileName.text = "";
            PlayerPrefs.SetString("NoteFileName", "");
        }
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
    IEnumerator SaveDataToJson(){
        SaveBlock.SetActive(true);

        ResetSavedData();

        noteSaved.Version = editorVersion;
        bpm = AutoTest.autoTest.bpm;

        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            GameObject targetNote;
            targetNote = NoteField.transform.GetChild(i).gameObject;

            // Add to List -----------------
            if (targetNote.tag == "Effect") effect.Add(targetNote);
            else if (targetNote.tag == "Bpm") speed.Add(targetNote);
            else note.Add(targetNote);
        }

        note.Sort(delegate (GameObject A, GameObject B)
        {
            if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
            else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
            return 0;
        });

        effect.Sort(delegate (GameObject A, GameObject B)
        {
            if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
            else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
            return 0;
        });

        speed.Sort(delegate (GameObject A, GameObject B)
        {
            if (A.transform.localPosition.y > B.transform.localPosition.y) return 1;
            else if (A.transform.localPosition.y < B.transform.localPosition.y) return -1;
            return 0;
        });

        if (speed.Count >= 1)
        {
            /// <summary>
            /// 
            /// 인자값은 var로 선언은 되지만, 적용은 되지 않는다
            /// GameObject test = new GameObject();
            ///
            /// var GetPos = transform.localPosition;
            ///
            /// float testPosA;
            /// testPosA = test.GetPos.y;
            /// testPosA = test.transform.localPosition.y;
            /// 
            /// </summary>

            // Add to List -----------------
            SpeedPos.Add(0);
            SpeedBpm.Add(bpm);
            for (int i = 0; i < speed.Count; i++)
            {
                float speedNum;
                speedNum = speed[i].transform.GetChild(0).GetChild(0).localPosition.x
                    * speed[i].transform.GetChild(0).GetChild(0).localPosition.y;

                SpeedPos.Add(speed[i].transform.localPosition.y);
                noteSaved.SpeedPos.Add(speed[i].transform.localPosition.y);
                SpeedBpm.Add(speedNum);

                noteSaved.SpeedNum.Add(speed[i].transform.GetChild(0).GetChild(0).localPosition.x);
                noteSaved.SpeedBpm.Add(speed[i].transform.GetChild(0).GetChild(0).localPosition.y);
            }

            // Add to List -----------------
            int newMs;
            SpeedMs.Add(0);
            newMs = (int)(speed[0].transform.localPosition.y * 150 / bpm);
            SpeedMs.Add(newMs);
            for (int i = 1; i < speed.Count; i++)
            {
                newMs += (int)(Mathf.Abs(SpeedPos[i + 1] - SpeedPos[i]) * 150 / Mathf.Abs(SpeedBpm[i]));
                SpeedMs.Add(newMs);
            }

            for (int i = 0; i < speed.Count; i++)
            {
                float speedNum;
                speedNum = SpeedBpm[i];
                if (speedNum < 0)
                {
                    try
                    {
                        SpeedPos[i] = (SpeedPos[i] + (SpeedPos[i + 1] - SpeedPos[i]) * 2);
                    }
                    catch
                    {
                        Debug.LogError("속도값이 잘못된 SpeedNote가 존재합니다");
                    }
                }
                else if (speedNum == 0)
                {
                    Debug.LogError("속도값이 0인 SpeedNote가 존재합니다.");
                }
            }
        }

        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < note.Count; i++)
        {
            // Add to List -----------------
            notePos.Add(note[i].transform.localPosition.y);
        }

        int SpeedNoteinfoIndex;
        SpeedNoteinfoIndex = 0;

        for (int i = 0; i < note.Count; i++)
        {
            GameObject targetNote;
            targetNote = note[i];

            for (int j = SpeedNoteinfoIndex; j < SpeedMs.Count; j++)
            {
                if (SpeedPos[j] < notePos[i])
                {
                    SpeedNoteinfoIndex = j;
                }
                else
                {
                    break;
                }
            }

            float ms;
            try{
                float posDif;
                posDif = notePos[i] - SpeedPos[SpeedNoteinfoIndex];
                ms = (int)(posDif * 150 / SpeedBpm[SpeedNoteinfoIndex]) + SpeedMs[SpeedNoteinfoIndex];
            }
            catch{
                ms = (int)(notePos[i] * 150 / bpm);
            }

            // Add to List -----------------
            noteMs.Add(ms);
            noteLegnth.Add((int)(note[i].transform.localScale.y / 100));
            if (targetNote.tag == "chip"){
                noteDouble.Add(targetNote.transform.GetChild(0).GetChild(0)
                    .GetComponent<SpriteRenderer>().enabled);
            }
            else{
                noteDouble.Add(false);
            }

            // Add to List -----------------
            if (targetNote.tag == "chip" || targetNote.tag == "long"){
                switch (targetNote.transform.localPosition.x){
                    case -300:
                        noteLine.Add(1);
                        break;

                    case -100:
                        noteLine.Add(2);
                        break;

                    case +100:
                        noteLine.Add(3);
                        break;

                    case +300:
                        noteLine.Add(4);
                        break;
                }
            }
            else{
                switch (targetNote.transform.localPosition.x){
                    case -100:
                        noteLine.Add(5);
                        break;

                    case +100:
                        noteLine.Add(6);
                        break;
                }
            }
        }

        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < effect.Count; i++){
            // Add to List -----------------
            EffectPos.Add(effect[i].transform.localPosition.y);
        }

        SpeedNoteinfoIndex = 0;
        for (int i = 0; i < effect.Count; i++){
            GameObject targetObject;
            targetObject = effect[i];

            GameObject childObject;
            childObject = targetObject.transform.GetChild(0).GetChild(0).gameObject;

            float EffectPosY;
            EffectPosY = EffectPos[i];

            for (int j = SpeedNoteinfoIndex; j < SpeedMs.Count - 1; j++){
                if (SpeedPos[j] < EffectPosY){
                    SpeedNoteinfoIndex = j;
                }
                else { break; }
            }

            float posDif;
            posDif = EffectPosY - SpeedPos[SpeedNoteinfoIndex];

            float ms;
            ms = (int)(posDif * 150 / SpeedBpm[SpeedNoteinfoIndex]) + SpeedMs[SpeedNoteinfoIndex];

            // Add to List -----------------
            EffectMs.Add(ms);
            EffectForce.Add(childObject.transform.localPosition.x);
            EffectDuration.Add((int)(childObject.transform.localScale.y));
        }

        try
        {
            SpeedMs.RemoveAt(0);
        }
        catch { }

        noteSaved.bpm = bpm;
        noteSaved.startDelayMs = AutoTest.autoTest.delay;

        noteSaved.NoteMs = noteMs;
        noteSaved.NotePos = notePos;
        noteSaved.NoteLine = noteLine;
        noteSaved.NoteLegnth = noteLegnth;
        noteSaved.isDouble = noteDouble;

        noteSaved.SpeedMs = SpeedMs;

        noteSaved.EffectMs = EffectMs;

        SaveBlock.SetActive(false);

        yield return new WaitForSeconds(0.5f);

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
    }

    [ContextMenu("Load")]
    IEnumerator LoadDataFromJson(){
        isLoopInLoad = false;
        isBreakInLoad = false;
        yield return new WaitForSeconds(.1f);

        AutoTest.autoTest.bpm = noteSaved.bpm;
        AutoTest.autoTest.delay = noteSaved.startDelayMs;

        for (int i = 0; i < NoteField.transform.childCount; i++)
        {
            Destroy(NoteField.transform.GetChild(i).gameObject);
        }

        ResetSavedData();
        try
        {
            string path = Path.Combine(Application.dataPath, inputFileName.text + ".json");
            string jsonData = File.ReadAllText(path);
            print(path);
            noteSaved = JsonUtility.FromJson<NoteSavedData>(jsonData);
        }
        catch 
        { 
            yield break; 
        }

        if (noteSaved.Version == null) {
            LoadBlock.SetActive(true);
            isLoopInLoad = true;
            isBreakInLoad = false;
            var wait = new WaitForSeconds(1.0f);
            while (isLoopInLoad){
                yield return null;
                if (isBreakInLoad){
                    print("Break by null");
                    yield break;
                }
            }
        }
        else if (noteSaved.Version != editorVersion) {
            LoadBlock.SetActive(true);
            isLoopInLoad = true;
            isBreakInLoad = false;
            var wait = new WaitForSeconds(1.0f);
            while (isLoopInLoad){
                yield return null;
                if (isBreakInLoad){
                    print("Break by Ver");
                    yield break;
                }
            }
        }

        inputBpm.text = (noteSaved.bpm).ToString();
        AutoTest.autoTest.bpm = noteSaved.bpm;
        inputStartDelayMs.text = (noteSaved.startDelayMs).ToString();
        AutoTest.autoTest.delay = noteSaved.startDelayMs;

        for (int i = 0; i < noteSaved.SpeedMs.Count; i++) {
            GameObject copy;
            copy = Instantiate(PrefabObject[5], NoteField.transform);
            copy.transform.localPosition = new Vector3(0, noteSaved.SpeedPos[i], 0);
            copy.transform.GetComponentInChildren<TextMeshPro>().text = 
                noteSaved.SpeedBpm[i].ToString() + "\nx " + noteSaved.SpeedNum[i].ToString();
            copy.transform.GetChild(0).GetChild(0).localPosition 
                = new Vector3(noteSaved.SpeedNum[i], noteSaved.SpeedBpm[i], 0);
        }

        for (int i = 0; i < noteSaved.NoteMs.Count; i++){
            GameObject copiedObject;
            Vector3 copiedObjectPos;
            copiedObjectPos = new Vector3(0, 0, 0);

            // 노트파일 해석 시작
            // 노트 X 위치와 노트의 종류 해석
            try{
                if (noteSaved.NoteLine[i] == 5 || noteSaved.NoteLine[i] == 6)
                {
                    if (noteSaved.NoteLine[i] == 5) { copiedObjectPos.x = -100; }
                    else { copiedObjectPos.x = 100; }

                    if (noteSaved.NoteLegnth[i] != 0)
                    {
                        copiedObject = Instantiate(PrefabObject[3], NoteField.transform);
                        copiedObjectPos.z = 0.003f;
                        copiedObject.transform.localScale = new Vector3(0.75f, noteSaved.NoteLegnth[i] * 100, 1);
                    }
                    else
                    {
                        copiedObjectPos.z = 0.001f;
                        copiedObject = Instantiate(PrefabObject[2], NoteField.transform);
                        copiedObject.transform.localScale = new Vector3(0.75f, 1, 1);
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
                            copiedObjectPos.x = -100;
                            break;

                        case 6:
                            copiedObjectPos.x = +100;
                            break;
                    }
                }

                // 노트의 Y좌표 해석
                // Ms = 150 * PosY / Bpm |=>| PosY = Bpm * Ms / 150
                if (noteSaved.NotePos.Count == 0){
                    float pos;
                    pos = noteSaved.bpm * noteSaved.NoteMs[i] / 150;
                    if (pos % 1 >= .5f) pos = pos - pos % 1 + 1;
                    else pos = pos - pos % 1;
                    copiedObjectPos.y = pos;
                }
                else{
                    copiedObjectPos.y = noteSaved.NotePos[i];
                }
                
                if(noteSaved.NoteLegnth[i] == 0 && noteSaved.NoteLine[i] < 5){
                    if (noteSaved.isDouble.Count == 0){
                        copiedObject.transform.GetChild(0).GetChild(0)
                            .GetComponent<SpriteRenderer>().enabled = false;
                    }
                    else{
                        copiedObject.transform.GetChild(0).GetChild(0)
                            .GetComponent<SpriteRenderer>().enabled = noteSaved.isDouble[i];
                    }
                    
                }
                copiedObject.transform.localPosition = copiedObjectPos;
            }
            catch { 
                print("catch 처리됨");
                break; 
            }
        }
        yield return new WaitForSeconds(.1f);
        PageSystem.pageSystem.PageSet(PageSystem.pageSystem.firstPage);

        PlayerPrefs.SetString("NoteFileName", inputFileName.text);
        TestPlay.testPlay.FileName = inputFileName.text;
    }
    IEnumerator CreateNewJsonData(){
        bool isFileExist = false;
        isLoopInLoad = true;
        isBreakInLoad = false;
        ResetSavedData();
        try{
            string path = Path.Combine(Application.dataPath, inputFileName.text + ".json");
            string jsonData = File.ReadAllText(path);
            print(path);
            noteSaved = JsonUtility.FromJson<NoteSavedData>(jsonData);
            isFileExist = true;
        }
        catch{ isFileExist = false; }

        if (isFileExist == true){
            CreateBlock.SetActive(true);
            while (isLoopInLoad){
                yield return null;
                if (isBreakInLoad){
                    yield break;
                }
            }
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
    }
    private void ResetSavedData()
    {
        NoteField = PageSystem.pageSystem.NoteField;

        note = new List<GameObject>();
        effect = new List<GameObject>();
        speed = new List<GameObject>();

        noteSaved.bpm = new float();
        noteSaved.startDelayMs = new int();

        noteSaved.NoteLegnth = new List<int>();
        noteSaved.NoteMs = new List<float>();
        noteSaved.NoteLine = new List<int>();
        noteSaved.isDouble = new List<bool>();

        noteSaved.EffectMs = new List<float>();
        noteSaved.EffectForce = new List<float>();
        noteSaved.EffectDuration = new List<int>();

        noteSaved.SpeedMs = new List<float>();
        noteSaved.SpeedPos = new List<float>();
        noteSaved.SpeedBpm = new List<float>();
        noteSaved.SpeedNum = new List<float>();

        noteMs = new List<float>();
        notePos = new List<float>();
        noteLine = new List<int>();
        noteLegnth = new List<int>();
        noteDouble = new List<bool>();

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
    public void ButtonCreate(){
        if (inputFileName.text == "" || inputFileName.text == null){
            SaveCompleteMessage.text = "Missing File name";
            SaveCompleteMessage.color = new Color32(255, 0, 0, 255);
            return;
        }
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
            SaveBlock.SetActive(false);
            SaveCompleteMessage.text = "Save Failed";
            SaveCompleteMessage.color = new Color32(255, 0, 0, 255);
            yield return new WaitForSeconds(3.0f);
            SaveCompleteMessage.text = "Music File Name";
            SaveCompleteMessage.color = new Color32(255, 255, 255, 255);
        }
    }
    public void ButtonLoadErrorSubmit(){
        LoadBlock.SetActive(false);
        CreateBlock.SetActive(false);
        isLoopInLoad = false;
    }
    public void ButtonLoadErrorCancle(){
        LoadBlock.SetActive(false);
        CreateBlock.SetActive(false);
        isBreakInLoad = true;
    }
}

[System.Serializable]
public class NoteSavedData{
    public string Version;
    public float bpm;
    public float startDelayMs;

    public List<int> NoteLegnth;
    public List<float> NoteMs;
    public List<float> NotePos;
    public List<int> NoteLine;
    public List<bool> isDouble;

    public List<float> EffectMs;
    public List<float> EffectForce;
    public List<int> EffectDuration;

    public List<float> SpeedMs;
    public List<float> SpeedPos;
    public List<float> SpeedBpm;
    public List<float> SpeedNum;
}