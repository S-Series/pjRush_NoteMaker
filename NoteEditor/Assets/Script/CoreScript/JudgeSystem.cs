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
    public KeyCode[] inputKey = new KeyCode[2]{KeyCode.None, KeyCode.None};
    
    //** private ---------------------------
    private int playLine;
    private int playJudgeMs = 0;
    private int noteIndex, targetNoteMs = 0;
    private int longIndex, targetLongMs = 0;
    private bool isLongJudge = false;
    private bool isJudgeAlive = false, isLongJudgeAlive = false;
    private IEnumerator longKeepCoroutine;
    private NormalNote targetNote = null, targetLongNote = null;
    [SerializeField] private Animator judgeEffect;
    [SerializeField] private SpriteRenderer lineEffect;

    //** Unity Actions ---------------------------
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
            CheckJudge();
            isLongJudge = true;
        }
        if (Input.GetKeyUp(inputKey[0]) || Input.GetKeyUp(inputKey[1]))
        {
            if (!Input.GetKey(inputKey[0]) && !Input.GetKey(inputKey[1]))
            {
                isLongJudge = false;
                lineEffect.enabled = false;
            }
        }

        if (playJudgeMs <= -judgeRange[2])
        {
            noteIndex++;
            ApplyJudge(targetNote, -100, false);

            if (noteIndex == gameNotes.Count) { isJudgeAlive = false; return; }
            targetNote = gameNotes[noteIndex];
            targetNoteMs = targetNote.ms;
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
        StopAllCoroutines();
        lineEffect.enabled = false;
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
    private void CheckJudge()
    {
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
    private void ApplyJudge(NormalNote _note, int _judgeMs, bool _isFast, bool _isFromLong = false)
    {
        //** S-Perfect
        if (_judgeMs < judgeRange[0] && _judgeMs > -judgeRange[0])
        {
            JudgePass(_isFromLong, _note.isPowered);
            lineEffect.sprite = lineEffectSprite[1];
            ScoreManager.ApplyJudge(0, _isFast);
            ScoreManager.ApplyCombo(true);
            judgeEffect.SetTrigger("Play");
        }
        //** Perfect
        else if (_judgeMs < judgeRange[1] && _judgeMs > -judgeRange[1])
        {
            JudgePass(_isFromLong, _note.isPowered);
            lineEffect.sprite = lineEffectSprite[1];
            ScoreManager.ApplyJudge(1, _isFast);
            ScoreManager.ApplyCombo(true);
            judgeEffect.SetTrigger("Play");
        }
        //** Near
        else if (_judgeMs < judgeRange[2] && _judgeMs > -judgeRange[2])
        {
            JudgePass(_isFromLong, _note.isPowered);
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
    private void JudgePass(bool _isLong, bool _isPowered)
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
        Animator _animate;
        int _targetMs, _maxCount = 0;
        float _timer = 0.0f, _maxTimer = 0.0f;
        bool _isAnimating = false, _isPassed = true;

        _animate = _note.noteObject.GetComponent<Animator>();
        _animate.enabled = true;
        _targetMs = NoteManager.CalculateNoteMs(_note.pos + 100);
        print(_targetMs);

        if (isLongJudge) { _animate.SetTrigger("Catch"); }

        _maxCount = _note.legnth - 1;
        _maxTimer = 0.15f * 125 / s_playBpm;
        for (int i = 0; i < _maxCount;)
        {
            if (isLongJudge) { _timer = 0.0f; _isPassed = true; }
            else
            {
                _timer += Time.deltaTime;
                if (_timer >= _maxTimer) { _isPassed = false; }
            }

            if (!_isPassed)
            {
                _isAnimating = false;
                _animate.SetTrigger("Lost");
            }
            else if (!_isAnimating)
            {
                _isAnimating = true;
                _animate.SetTrigger("Catch");
            }


            if (_targetMs <= s_playMs)
            {
                if (_isPassed)
                {
                    ApplyJudge(_note, 0, true, true);
                }
                else
                {
                    ApplyJudge(_note, -100, true, true);
                }
                _targetMs = NoteManager.CalculateNoteMs(_note.pos + (100 * (i + 2)));
                i++;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        _animate.SetTrigger("Exit");
        _animate.enabled = false;
        _note.noteObject.SetActive(false);
    }
}