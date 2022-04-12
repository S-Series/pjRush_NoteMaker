using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeSystem : MonoBehaviour
{
    public static bool isAutoPlayMode;
    #region properties
    public List<GameObject> TestPlayObject;
    public List<float> TestPlayMs;
    public List<int> TestPlayLegnth;
    public List<bool> TestPlayDouble;
    public KeyCode MainKey;
    public KeyCode SubKey;
    public bool isBottom;
    private GameObject TargetObject;
    private GameObject longTargetObject;
    private bool isLongJudge;
    private int ms;
    private float judgeMs;
    private float longJudgeMs;
    private int longNoteJudgeMs;
    private int index;
    private int longIndex;
    private int type;
    private bool isDouble;
    float wait;
    [SerializeField] Animator HitEffect;
    [SerializeField] AudioSource[] HitSound;
    AutoTest auto;
    [SerializeField] private GameObject LongBlind;
    private readonly static Color32[] spriteColor = {
        new Color32(255, 255, 255, 255),
        new Color32(240, 240, 240, 255),
        new Color32(225, 225, 225, 255),
        new Color32(150, 150, 150, 255)
    };
    private void Start()
    {
        auto = AutoTest.autoTest;
        TestPlayObject = new List<GameObject>();
        TestPlayMs = new List<float>();
        index = 0;
        ms = 0;
        isLongJudge = false;
    }
    private void OnEnable()
    {
        auto = AutoTest.autoTest;
        index = 0;
        ms = 0;
        isLongJudge = false;
    }
    private void Awake() {
        TestPlayMs = new List<float>();
        TestPlayLegnth = new List<int>();
        TestPlayDouble = new List<bool>();
        TestPlayObject = new List<GameObject>();
    }
    #endregion
    void Update(){
        try{
            judgeMs = TestPlayMs[index] - ms;
            TargetObject = TestPlayObject[index];
        }
        catch { return; }
        try{
            longJudgeMs = TestPlayMs[longIndex] - ms;
        }
        catch { longJudgeMs = 1; }
        ms = TestPlay.playMs;
        // ------------------------------------------------------------------------
        if (isAutoPlayMode){
            if (judgeMs <= 0f){
                if (isBottom){
                    if (TestPlayObject[index].transform.localPosition.x < 0){
                        type = -1;
                    }
                    else{
                        type = 1;
                    }
                }
                JudgeResult(judgeMs);
                CheckLong(index, isDouble);
            }
            if (longJudgeMs <= 0){
                isLongJudge = true;
                if (TestPlayLegnth[longIndex] != 0){
                    StartCoroutine(LongStart(TestPlayLegnth[longIndex], TestPlayObject[longIndex]));
                }
                longIndex++;
            }
            return;
        }
        // ------------------------------------------------------------------------
        if (longJudgeMs <= 0){
            if (TestPlayLegnth[longIndex] != 0){
                StartCoroutine(LongStart(TestPlayLegnth[longIndex], TestPlayObject[longIndex]));
            }
            longIndex++;
        }
        if (Input.GetKeyDown(MainKey)){
            type = -1;
            NoteInput();
        }
        if (Input.GetKeyDown(SubKey))
        {
            type = +1;
            NoteInput();
        }
        if (Input.GetKeyUp(MainKey) || Input.GetKeyUp(SubKey))
        {
            StartCoroutine(longKeep());
        }
        if (judgeMs < -85){
            isLongJudge = false;
            JudgeResult(-100f);
            CheckLong(index, isDouble);
        }

        if (!isLongJudge)
        {
            LongBlind.transform.localPosition -= new Vector3(0, TestPlay.testPlay.BlindMovingPos, 0);
        }
    }
    private void NoteInput(){
        isLongJudge = true;
        if (judgeMs >= -85f && judgeMs <= 100f){
            JudgeResult(judgeMs);
            CheckLong(index, isDouble);
        }
        else{
            //HitEffect.SetTrigger("None");
        }
    }
    private IEnumerator longKeep()
    {
        wait = 15 / AutoTest.autoTest.bpm;
        yield return new WaitForSeconds(2 * wait);
        if (!Input.GetKey(MainKey) && !Input.GetKey(SubKey)) isLongJudge = false;
    }
    private void CheckLong(int getIndex, bool _isDouble){
        if (TestPlayLegnth[getIndex] == 0){
            if (_isDouble){
                TestPlayObject[index].transform.GetChild(0).GetChild(0).
                    GetComponent<SpriteRenderer>().enabled = false;
                isDouble = false;
            }
            else{
                TestPlayObject[getIndex].SetActive(false);
                index++;
                try{
                    isDouble = TestPlayDouble[index];
                }
                catch{}
            }
        }
        else{
            index++;
            try{
                isDouble = TestPlayDouble[index];
            }
            catch{}
        }
    }
    private void JudgeResult(float judgeMs){
        if (judgeMs == 999f){
            HitSound[1].Play();
            TestPlay.testPlay.Rush[1]++;
            TestPlay.testPlay.JudgeDisplay();
            HitEffect.SetTrigger("Record");
            TestPlay.scoreManager.scoreJudgeType(true);
            return;
        }
        else if (judgeMs == -999f){
            TestPlay.testPlay.Lost[1]++;
            TestPlay.testPlay.JudgeDisplay();
            TestPlay.scoreManager.ResetCombo();
            return;
        }
        // ------------------------------------------------
        else if (judgeMs >= -30f && judgeMs <= 30f){
            TestPlay.testPlay.Rush[1]++;
            TestPlay.scoreManager.scoreJudgeType(true);
            HitEffect.SetTrigger("Record");
            HitSound[0].Play();
        }
        else if (judgeMs >= -55f && judgeMs <= 55f){
            if (judgeMs > 0)
            {
                TestPlay.testPlay.Rush[0]++;
            }
            else
            {
                TestPlay.testPlay.Rush[2]++;
            }
            HitEffect.SetTrigger("Record");
            HitSound[0].Play();
            TestPlay.scoreManager.scoreJudgeType(true);
        }
        else if (judgeMs >= -85f && judgeMs <= 85f){
            if (judgeMs > 0)
            {
                TestPlay.testPlay.Step[0]++;
            }
            else
            {
                TestPlay.testPlay.Step[1]++;
            }
            HitEffect.SetTrigger("Trace");
            HitSound[0].Play();
            TestPlay.scoreManager.scoreJudgeType(false);
        }
        else if (judgeMs > 85f && judgeMs <= 100f){
            isLongJudge = false;
            TestPlay.testPlay.Lost[0]++;
            TestPlay.scoreManager.ResetCombo();
        }
        else if (judgeMs <= -85f){
            TestPlay.testPlay.Lost[1]++;
            TestPlay.scoreManager.ResetCombo();
        }
        else {
            return; 
        }
        TestPlay.testPlay.JudgeDisplay();
    }
    private IEnumerator LongStart(int Legnth, GameObject longObject)
    {
        SpriteRenderer sprite;
        sprite = longObject.GetComponentInChildren<SpriteRenderer>();

        wait = 15 / TestPlay.testBpm;
        var delay = new WaitForSeconds(wait);
        yield return delay;
        for (int i = 1; i < Legnth; i++)
        {
            if (isLongJudge)
            {
                JudgeResult(999f);
                sprite.color = spriteColor[0];
                StartCoroutine(color(sprite, wait / 4, spriteColor[1], spriteColor[2]));
            }
            else
            {
                isLongJudge = false;
                sprite.color = spriteColor[3];
                JudgeResult(-999f);
                //ComboManager.comboManager.resetCombo();
            }

            yield return delay;

            if (!TestPlay.isPlay) break;
        }
        sprite.color = spriteColor[0];
        longObject.SetActive(false);
    }
    private IEnumerator color(SpriteRenderer sprite, float duration, Color32 color1, Color32 color2)
    {
        yield return new WaitForSeconds(duration);
        sprite.color = color1;
        yield return new WaitForSeconds(duration);
        sprite.color = color2;
        yield return new WaitForSeconds(duration);
        sprite.color = color1;
    }
    public void resetJudgeSystem()
    {
        foreach(GameObject gameObject in TestPlayObject){
            Destroy(gameObject);
        }
        index = 0;
        longIndex = 0;
        ms = 0;
        isDouble = false;
        TestPlayMs = new List<float>();
        TestPlayLegnth = new List<int>();
        TestPlayDouble = new List<bool>();
        TestPlayObject = new List<GameObject>();
    }
    public void reStart(){
        if (TestPlayMs.Count == 0) return;
        ms = 0;
        index = 0;
        longIndex = 0;
        isDouble = TestPlayDouble[0];
    }
    public void NoteDataAddTo(GameObject _noteObject, float _ms, int _legnth, bool _isDouble)
    {
        TestPlayObject.Add(_noteObject);
        TestPlayMs.Add(_ms);
        TestPlayLegnth.Add(_legnth);
        TestPlayDouble.Add(_isDouble);
    }
}
