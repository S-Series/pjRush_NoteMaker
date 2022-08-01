using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static bool isSplit = false;
    public static Animator AnimatorLineSplit;
    private static readonly string[] TriggerLineSplit = {"Split", "UnSplit"};
    [SerializeField] GameObject LineAnimatorObject;
    private void Awake()
    {
        AnimatorLineSplit = LineAnimatorObject.GetComponent<Animator>();
    }
    public static void SplitChange()
    {
        if (isSplit) { AnimatorLineSplit.SetTrigger(TriggerLineSplit[1]); }
        else { AnimatorLineSplit.SetTrigger(TriggerLineSplit[0]); }
        isSplit = !isSplit;
    }
    public static void ResetSplit()
    {
        isSplit = false;
        AnimatorLineSplit.SetTrigger(TriggerLineSplit[1]);
    }
}
