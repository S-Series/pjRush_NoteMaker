using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMode : MonoBehaviour
{
    //** static ---------------------------
    public static PlayMode playMode;
    public static bool s_isPlay;
    public static int s_noteCount;
    public static int s_maxComboCount;
    public static int s_playModeSpeed = 400;
    public static KeyCode[][] s_keyCodes = new KeyCode[6][];
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
    
    //** Unity Actions ---------------------------
    private void Awake()
    {
        playMode = this;
        playAudio = GetComponent<AudioSource>();

        for (int i = 0; i < 6; i++)
        {
            s_keyCodes[i] = new KeyCode[2];
            s_keyCodes[i][0] = KeyCode.None;
            s_keyCodes[i][1] = KeyCode.None;
        }
        //Todo : KeyBinding 시스템 업데이트 전 임시 코드
        // /*
        s_keyCodes[0][0] = KeyCode.S;
        s_keyCodes[0][1] = KeyCode.J;
        s_keyCodes[1][0] = KeyCode.D;
        s_keyCodes[1][1] = KeyCode.K;
        s_keyCodes[2][0] = KeyCode.F;
        s_keyCodes[2][1] = KeyCode.L;
        s_keyCodes[3][0] = KeyCode.G;
        s_keyCodes[3][1] = KeyCode.Semicolon;

        s_keyCodes[4][0] = KeyCode.A;
        s_keyCodes[4][1] = KeyCode.None;
        s_keyCodes[5][0] = KeyCode.Quote;
        s_keyCodes[5][1] = KeyCode.None;
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
            float pos;
            pos = playSpeedPos + playEffectPos + (((JudgeSystem.s_playMs - playSpeedMs - playEffectMs) * JudgeSystem.s_playBpm) / 150);
            noteMovingField.localPosition = new Vector3(0, -pos * (3 * s_playModeSpeed / 100.0f), 0);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                JudgeSystem.s_isNowOnPlay = false;
                playAudio.Stop();
                noteMovingField.localPosition = new Vector3(0, 0, 0);
                GenerateNote();
                ResetPlayData();
            }
        }
    }
    private void FixedUpdate()
    {
        if (!JudgeSystem.s_isNowOnPlay) { return; }
        JudgeSystem.s_playMs++;
    }
    
    //** Static void ---------------------------
    public static void toPlayMode(bool _isPlayMode)
    {
        playMode.playCamera.gameObject.SetActive(_isPlayMode);
        playMode.modeObject[0].SetActive(!_isPlayMode);
        playMode.modeObject[1].SetActive(_isPlayMode);
        s_isPlay = _isPlayMode;
        playMode.GenerateNote();
        playMode.ResetPlayData();
    }
    
    //** public void ---------------------------
    
    //** private void ---------------------------
    private void GenerateNote()
    {
        isLoaded = false;
        for (int i = 0; i < 6; i++)
        {
            judgeSystems[i].ResetSystem(_isResetList: true);
            judgeSystems[i].SystemSetting(_kc: s_keyCodes[i], _line: i);
            for (int j = 0; j < noteGenerateField[i].transform.childCount; j++)
            {
                Destroy(noteGenerateField[i].transform.GetChild(0).gameObject);
            }
        }
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
        foreach (JudgeSystem _judge in judgeSystems)
        {
            _judge.ResetSystem();
        }
        playSpeedMs = 0;
        playSpeedPos = 0;
        playSpeedIndex = 0;

        playEffectMs = 0;
        playEffectPos = 0;
        playEffectIndex = 0;

        ScoreManager.ResetGamePlay();
    }
    //** private Value ---------------------------
    private bool isKeyCodeAvailable(KeyCode _kc)
    {
        if (_kc == KeyCode.Escape)
        for (int i = 0; i < 5; i++)
        {
            if (s_keyCodes[i][0] == _kc || s_keyCodes[i][0] == _kc) { return false; }
        }
        return true;
    }
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
            print(judgeSystem.gameObject.name);
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
        toPlayMode(_isPlay);
    }
    public void btn_bindKey(int _firstIndex, int _secondIndex)
    {

    }
    public void btn_doSomething()
    {

    }
}
