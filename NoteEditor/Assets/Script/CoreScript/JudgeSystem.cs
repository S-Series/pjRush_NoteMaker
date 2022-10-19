using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeSystem : MonoBehaviour
{
    //** static ---------------------------
    public static int s_playMs;
    public static int s_SpeedMs;
    public static int s_EffectMs;
    public static bool s_isNowOnPlay = false;
    public static float s_playBpm;
    public static Sprite[] judgeSprite;
    public static Sprite[] lineEffectSprite;
    private static readonly double[] judgeRange = new double[4]{ 22.5, 45.5, 70.5, 80.5};
    
    //** public ---------------------------
    public List<NormalNote> gameNotes = new List<NormalNote>();
    public bool isBpmChanged = false;
    public KeyCode[] inputKey = new KeyCode[2]{KeyCode.None, KeyCode.None};
    
    //** private ---------------------------
    private int playLine;
    private int playJudgeMs = 0;
    private int noteIndex, targetNoteMs = 0;
    private int longIndex, targetLongMs = 0;
    private bool isLongJudge = false;
    private bool isJudgeAlive = false, isLongJudgeAlive = false;
    private IEnumerator longCoroutine, longKeepCoroutine;
    private NormalNote targetNote = null, targetLongNote = null;
    [SerializeField] private Animator judgeEffect;
    [SerializeField] private SpriteRenderer lineEffect;

    //** Unity Actions ---------------------------
    private void Awake()
    {
        longCoroutine = ILongCoroutine(null);
    }
    private void Update()
    {
        if (!s_isNowOnPlay) { return; }
        if (!isJudgeAlive) { return; }

        if (isLongJudgeAlive)
        {
            if (targetLongMs <= s_playMs)
            {
                if (targetLongNote.legnth != 0)
                {
                    StartCoroutine(ILongCoroutine(targetLongNote));
                }
                longIndex++;
                if (longIndex == gameNotes.Count) { isLongJudgeAlive = false; }
                else
                {
                    targetLongNote = gameNotes[longIndex];
                    targetLongMs = gameNotes[longIndex].ms;
                }
            }
        }

        playJudgeMs = targetNoteMs - s_playMs;

        if (Input.GetKeyDown(inputKey[0]) || Input.GetKeyDown(inputKey[1]))
        {
            StopCoroutine(longCoroutine);
            isLongJudge = true;
            
            if (playJudgeMs < judgeRange[3] && playJudgeMs > -judgeRange[2])
            {
                noteIndex++;
                if (playJudgeMs >= 0) { ApplyJudge(targetNote, playJudgeMs, true); }
                else { ApplyJudge(targetNote, playJudgeMs, false); }

                if (noteIndex == gameNotes.Count) { isJudgeAlive = false; return; }

                targetNote = gameNotes[noteIndex];
                targetNoteMs = targetNote.ms;
            }
            else
            {
                lineEffect.sprite = lineEffectSprite[0];
                lineEffect.enabled = true;
            }
        }
        if (Input.GetKeyUp(inputKey[0]) || Input.GetKeyUp(inputKey[1]))
        {
            if (!(Input.GetKey(inputKey[0]) || Input.GetKey(inputKey[1])))
            {
                longCoroutine = IKeepLong();
                StartCoroutine(longCoroutine);
                lineEffect.enabled = false;
            }
        }

        if (playJudgeMs <= -judgeRange[2])
        {
            noteIndex++;
            if (noteIndex != gameNotes.Count)
            {
                ApplyJudge(targetNote, -100, false);
                targetNote = gameNotes[noteIndex];
                targetNoteMs = targetNote.ms;
            }
        }
    }

    //** public void ---------------------------
    public void ResetSystem(bool _isResetList = false)
    {
        if (_isResetList) { gameNotes = new List<NormalNote>(); }

        noteIndex = 0;
        targetNoteMs = 0;
        longIndex = 0;
        targetLongMs = 0;
        playJudgeMs = 0;
    }
    public void SystemSetting(KeyCode[] _kc, int _line)
    {
        inputKey = _kc;
        playLine = _line;
    }
    public void StartGamePlay()
    {
        if (gameNotes.Count == 0) { isJudgeAlive = false; return; }
        
        isJudgeAlive = true;
        isLongJudgeAlive = true;
        targetNote = gameNotes[0];
        targetNoteMs = gameNotes[0].ms;
        targetLongNote = gameNotes[0];
        targetLongMs = gameNotes[0].ms;
    }

    //** private void ---------------------------
    private void ApplyJudge(NormalNote _note, int _judgeMs, bool _isFast, bool _isFromLong = false)
    {
        //** S-Perfect
        if (_judgeMs < judgeRange[0] && _judgeMs > -judgeRange[0])
        {
            JudegPass(_isFromLong, _note.isPowered);
            lineEffect.sprite = lineEffectSprite[1];
            ScoreManager.ApplyJudge(0, _isFast);
            ScoreManager.ApplyCombo(true);
            judgeEffect.SetTrigger("Play");
        }
        //** Perfect
        else if (_judgeMs < judgeRange[1] && _judgeMs > -judgeRange[1])
        {
            JudegPass(_isFromLong, _note.isPowered);
            lineEffect.sprite = lineEffectSprite[1];
            ScoreManager.ApplyJudge(1, _isFast);
            ScoreManager.ApplyCombo(true);
            judgeEffect.SetTrigger("Play");
        }
        //** Near
        else if (_judgeMs < judgeRange[2] && _judgeMs > -judgeRange[2])
        {
            JudegPass(_isFromLong, _note.isPowered);
            lineEffect.sprite = lineEffectSprite[2];
            ScoreManager.ApplyJudge(2, _isFast);
            ScoreManager.ApplyCombo(true);
            judgeEffect.SetTrigger("Trace");
        }
        //** Miss
        else
        {
            if (_isFast) { lineEffect.sprite = lineEffectSprite[3]; }
            else {lineEffect.sprite = null; }
            ScoreManager.ApplyJudge(3, _isFast);
            ScoreManager.ApplyCombo(false);
        }

        if (_note.legnth == 0) { _note.noteObject.SetActive(false); }
        else { _note.noteObject.GetComponent<NoteOption>().VisibleSetting(0, false); }
        if (_isFromLong) { return; }

        lineEffect.enabled = true;
    }
    private void JudegPass(bool _isLong, bool _isPowered)
    {
        if (_isLong)
        {
            PlayMode.playMode.judgeSound[1].Play();
        }
        else if (_isPowered)
        {
            PlayMode.playMode.judgeSound[2].Play();
        }
        else
        {
            PlayMode.playMode.judgeSound[0].Play();
        }

    }

    //** private others ---------------------------
    private IEnumerator ILongCoroutine(NormalNote _note)
    {
        var _wait = new WaitForSeconds(15 / s_playBpm);

        bool _isAnimating = false;
        Animator _animate;
        _animate = _note.noteObject.GetComponent<Animator>();
        _animate.enabled = true;
        _animate.SetTrigger("Catch");

        for (int i = 0; i < _note.legnth - 1; i++)
        {
            if (isBpmChanged)
            {
                _wait = new WaitForSeconds(15 / s_playBpm);
                isBpmChanged = false;
            }
            yield return _wait;

            if (isLongJudge)
            {
                ApplyJudge(_note, 0, true);
                if (!_isAnimating)
                {
                    _isAnimating = true;
                    _animate.SetTrigger("Catch");
                }
            }
            else
            {
                ApplyJudge(_note, -100, true);
                if (_isAnimating)
                {
                    _isAnimating = false;
                    _animate.SetTrigger("Lost");
                }
            }
        }

        yield return _wait;
        yield return _wait;

        _animate.SetTrigger("Exit");
        _animate.enabled = false;
        _note.noteObject.SetActive(false);
    }
    private IEnumerator IKeepLong()
    {
        yield return new WaitForSeconds(30 / s_playBpm / (PlayMode.s_playModeSpeed / 100.0f));
        isLongJudge = false;
    }
}