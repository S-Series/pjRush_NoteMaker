using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayMode : MonoBehaviour
{
    //** static ---------------------------
    public static PlayMode playMode;
    public static bool s_isPlay;
    public static int s_noteCount;
    public static int s_maxComboCount;
    public static int s_playModeSpeed = 100;
    public static float nowPos;
    public static KeyCode[] s_keyCodes = new KeyCode[11];
    private static List<SpeedNote> playSpeedNotes = new List<SpeedNote>();
    private static List<EffectNote> playEffectNotes = new List<EffectNote>();

    //** public ---------------------------
    public Sprite[] judgeSprites;
    public AudioSource[] judgeSound;
    public JudgeSystem[] judgeSystems = new JudgeSystem[4];
    public BottomJudgeSystem[] bottomJudgeSystems = new BottomJudgeSystem[2];

    //** private ---------------------------
    private int playSpeedIndex, playEffectIndex;
    private int playSpeedMs, playEffectMs;
    private float playSpeedPos, playEffectPos;
    private bool isLoaded = false;
    private bool[] isPlayTest = new bool[2] { false, false };
    private AudioSource playAudio;
    private SpeedNote targetSpeed;
    private EffectNote targetEffect;

    //** Serialize ---------------------------
    [SerializeField] private Camera playCamera;
    [SerializeField] private GameObject[] prefabObject;
    [SerializeField] private GameObject[] judgeSystemParents;
    [SerializeField] private GameObject[] modeObject;
    [SerializeField] private Transform noteMovingField;
    [SerializeField] private Transform[] noteGenerateField;
    [SerializeField] private Sprite[] lineEffectSprite;
    [SerializeField] private TextMeshPro speedText;
    
    //** Unity Actions ---------------------------
    private void Awake()
    {
        playMode = this;
        playAudio = GetComponent<AudioSource>();
        toPlayMode(false);
        btn_changeSpeed(PlayerPrefs.GetInt("playSpeed") - s_playModeSpeed);

        //Todo : KeyBinding 시스템 업데이트 전 임시 코드
        // /*
        s_keyCodes[0] = KeyCode.S;
        s_keyCodes[1] = KeyCode.D;
        s_keyCodes[2] = KeyCode.F;
        s_keyCodes[3] = KeyCode.G;

        s_keyCodes[4] = KeyCode.J;
        s_keyCodes[5] = KeyCode.K;
        s_keyCodes[6] = KeyCode.L;
        s_keyCodes[7] = KeyCode.Semicolon;

        s_keyCodes[8] = KeyCode.A;
        s_keyCodes[9] = KeyCode.Quote;
        // */
    }
    private void Start()
    {
        JudgeSystem.judgeSprite = judgeSprites;
        JudgeSystem.lineEffectSprite = lineEffectSprite;
    }
    private void Update()
    {
        if (!s_isPlay) { return; }

        if (isLoaded)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                print("Play");
                StartCoroutine(IStartPlay());
                isLoaded = false;
            }
        }
        else if (JudgeSystem.s_isNowOnPlay)
        {
            if (isPlayTest[0])
            {
                if (targetSpeed.ms <= JudgeSystem.s_playMs)
                {
                    playSpeedIndex++;
                    JudgeSystem.s_playBpm = targetSpeed.bpm * targetSpeed.multiply;

                    if (playSpeedIndex == playSpeedNotes.Count) { isPlayTest[0] = false; }
                    else
                    {
                        targetSpeed = playSpeedNotes[playSpeedIndex];
                        playSpeedMs = targetSpeed.ms;
                        playSpeedPos = targetSpeed.pos;
                    }
                }
            }
            if (isPlayTest[1])
            {
                if (playEffectNotes[playEffectIndex].ms <= JudgeSystem.s_playMs)
                {
                    playEffectIndex++;
                    if (playEffectIndex == playEffectNotes.Count) { isPlayTest[1] = false; }
                }
            }
            nowPos = playSpeedPos + playEffectPos + (((JudgeSystem.s_playMs - playSpeedMs - playEffectMs) * JudgeSystem.s_playBpm) / 150);
            noteMovingField.localPosition = new Vector3(0, -nowPos * (3 * s_playModeSpeed / 100.0f), 0);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                JudgeSystem.s_isNowOnPlay = false;
                playAudio.Stop();
                noteMovingField.localPosition = new Vector3(0, 0, 0);
                GenerateNote(false);
                ResetPlayData();
            }
        }
    }
    private void FixedUpdate()
    {
        if (!JudgeSystem.s_isNowOnPlay) { return; }
        JudgeSystem.s_playMs++;
        LineMove.s_nowMs = JudgeSystem.s_playMs;
    }
    
    //** Static void ---------------------------
    public static void toPlayMode(bool _isPlayMode)
    {
        playMode.playAudio.Stop();
        AutoTest.autoTest.CelarNoteField();
        JudgeSystem.s_isNowOnPlay = false;
        s_isPlay = _isPlayMode;
        playMode.playCamera.gameObject.SetActive(_isPlayMode);
        playMode.modeObject[0].SetActive(!_isPlayMode);
        playMode.modeObject[1].SetActive(_isPlayMode);

        playMode.ResetPlayData();
        playMode.GenerateNote(!_isPlayMode);
    }
    
    //** public void ---------------------------
    
    //** private void ---------------------------
    private void GenerateNote(bool _isGameEnd)
    {
        isLoaded = false;
        for (int i = 0; i < 6; i++)
        {
            KeyCode[] _keycode = new KeyCode[2];
            if (i < 4) //$ index 0 to 7
            {
                _keycode[0] = s_keyCodes[i];
                _keycode[1] = s_keyCodes[i + 4];
            }
            else //$ i == 4 || i == 5
            {
                _keycode[0] = s_keyCodes[i + 4];
                _keycode[1] = KeyCode.None;
            }
            judgeSystems[i].ResetSystem(_isResetList: true);
            judgeSystems[i].SystemSetting(_kc: _keycode, _line: i);
            for (int j = 0; j < noteGenerateField[i].transform.childCount; j++)
            {
                Destroy(noteGenerateField[i].transform.GetChild(0).gameObject);
            }
        }
        foreach (Transform field in noteGenerateField)
        {
            for (int i = 0; i < field.childCount; i++)
            {
                Destroy(field.GetChild(i).gameObject);
            }
        }

        if (_isGameEnd) { return; }
        
        NoteClasses.SortingNotes();
        NoteClasses.CalculateNoteMs();

        playSpeedNotes = SpeedNote.speedNotes;
        playEffectNotes = EffectNote.effectNotes;
        s_noteCount = NormalNote.normalNotes.Count;

        int _fullCombo = 0;
        for (int i = 0; i < NormalNote.normalNotes.Count; i++)
        {
            NormalNote _noteData;
            _noteData = NormalNote.copyData(NormalNote.normalNotes[i], false);

            GameObject _copyObject;
            if (_noteData.line < 5) 
            { 
                _copyObject = Instantiate(prefabObject[0], noteGenerateField[_noteData.line - 1]);
            }
            else
            {
                _copyObject = Instantiate(prefabObject[1], noteGenerateField[_noteData.line - 1]);
            }
            _noteData.noteObject = _copyObject;
            _copyObject.transform.localPosition 
                = new Vector3(0, _noteData.pos * (s_playModeSpeed / 100.0f), 0);

            NoteOption _noteOption;
            _noteOption = _copyObject.GetComponent<NoteOption>();
            _noteOption.ToLongNote(_noteData.legnth, s_playModeSpeed);
            _noteOption.ToPoweredNote(_noteData.isPowered);
            _noteOption.EnableCollider(false);

            if (_noteData.legnth == 0) { _fullCombo++; }
            else { _fullCombo += _noteData.legnth; }

            judgeSystems[_noteData.line - 1].gameNotes.Add(_noteData);
        }
        ScoreManager.SetGameInfo(_fullCombo);
        
        if (playSpeedNotes.Count == 0) { isPlayTest[0] = true; }
        else { targetSpeed = playSpeedNotes[0]; isPlayTest[0] = false; }

        if (playEffectNotes.Count == 0) { isPlayTest[1] = true; }
        else { targetEffect = playEffectNotes[0]; isPlayTest[1] = false; }

        JudgeSystem.s_playBpm = ValueManager.bpm;
        isLoaded = true;
    }
    private void ResetPlayData()
    {
        nowPos = 0;
        foreach (JudgeSystem _judge in judgeSystems)
        {
            _judge.ResetSystem();
            _judge.StopAllCoroutines();
        }
        playSpeedMs = 0;
        playSpeedPos = 0;
        playSpeedIndex = 0;

        playEffectMs = 0;
        playEffectPos = 0;
        playEffectIndex = 0;

        ScoreManager.ResetGamePlay();
    }
    private void ApplySpeed()
    {
        foreach (JudgeSystem judge in judgeSystems)
        {
            foreach (NormalNote normal in judge.gameNotes)
            {
                normal.noteObject.transform.localPosition
                    = new Vector3(0, normal.pos * (s_playModeSpeed / 100.0f), 0);
                normal.noteObject.GetComponent<NoteOption>().ToLongNote(normal.legnth, s_playModeSpeed);
            }
        }
    }
    
    //** private Value ---------------------------
    private IEnumerator IStartPlay()
    {
        if (playSpeedNotes.Count == 0) { isPlayTest[0] = false; }
        else { isPlayTest[0] = true; }
        if (playEffectNotes.Count == 0) { isPlayTest[1] = false; }
        else { isPlayTest[1] = true; }
        ResetPlayData();
        JudgeSystem.s_playMs = Mathf.RoundToInt(150 / JudgeSystem.s_playBpm);
        foreach(JudgeSystem judgeSystem in judgeSystems) 
        {
            judgeSystem.StartGamePlay();
        }
        yield return new WaitForSeconds(1.0f);
        JudgeSystem.s_isNowOnPlay = true;
        playAudio.clip = MusicLoad.MusicClip;
        yield return new WaitForSeconds(ValueManager.delay / 1000.0f);
        playAudio.Play();
    }

    //** UI Interactions ---------------------------
    public void btn_toPlayMode(bool _isPlay)
    {
        if (AutoTest.s_isTest) { return; }
        toPlayMode(_isPlay);
    }
    public void btn_bindKey(int _firstIndex, int _secondIndex)
    {

    }
    public void btn_changeSpeed(int _value)
    {
        s_playModeSpeed += _value;
        if (s_playModeSpeed < 50) { s_playModeSpeed = 50; }
        if (s_playModeSpeed > 1000) { s_playModeSpeed = 1000; }
        PlayerPrefs.SetInt("playSpeed", s_playModeSpeed);
        speedText.text = String.Format("{0:F2}", s_playModeSpeed / 100.0f);
        ApplySpeed();
    }
}
