using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomJudgeSystem : MonoBehaviour
{
    public static int s_playMs;
    public static int s_SpeedMs;
    public static int s_EffectMs;
    public static bool s_isNowOnPlay;
    public KeyCode[] inputKey = new KeyCode[2]{KeyCode.None, KeyCode.None};
    public List<NormalNote> gameNotes = new List<NormalNote>();
}
