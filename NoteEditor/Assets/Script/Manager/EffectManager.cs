using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class EffectManager : MonoBehaviour
{
    public static EffectManager effectManager;
    public static bool isEffectSelected;
    private bool previewEffect;
    const KeyCode inputKeyCode = KeyCode.LeftControl;
    private readonly Rect originRect = new Rect (0, 0, 1, 1);
    private readonly Rect previewRect = new Rect (0.01f, 0.1375f, 0.725f, 0.725f);
    [SerializeField] GameObject EffectPreviewCamera;
    [SerializeField] GameObject EffectPreviewGameObject;
    [SerializeField] Volume volume;
    private Vignette vignette;
    private ChromaticAberration chromatic;
    private Bloom bloom;

    #region Teleport Effect Setting     || Type Num = 1
    [SerializeField] GameObject TeleportSector;
    [SerializeField] TMP_InputField[] TeleportInputField;
    [SerializeField] Button[] TeleportButton;
    #endregion

    #region Pause Effect Setting      || Type Num = 2
    [SerializeField] GameObject PauseSector;
    #endregion
    
    private void Awake()
    {
        effectManager = this;
        previewEffect = false;
        EffectPreviewCamera.GetComponent<Camera>().rect = originRect;
        EffectPreviewCamera.SetActive(false);
        EffectPreviewGameObject.SetActive(false);
    }
    private void Update()
    {
        if (isEffectSelected)
        {
            if (Input.GetKeyDown(inputKeyCode))
            {
                EffectPreview();
            }
        }
        else if (previewEffect)
        {
            previewEffect = false;
            EffectPreview();
        }
    }
    private void EffectPreview()
    {
        print("run1");
        if (!AutoTest.isTest && 
            !TestPlay.isPlay && 
            !TestPlay.isPlayReady)
        {
            previewEffect = !previewEffect;
            EffectPreviewGameObject.SetActive(previewEffect);
            print("run2");
        }

        if (previewEffect)
        {
            EffectPreviewCamera.GetComponent<Camera>().rect = previewRect;
            EffectPreviewCamera.SetActive(true);
            EffectPreviewGameObject.SetActive(true);
            //AutoTest.autoTest.TestPlayObject[1].SetActive(true);
        }
        else
        {
            EffectPreviewCamera.GetComponent<Camera>().rect = originRect;
            EffectPreviewCamera.SetActive(false);
            EffectPreviewGameObject.SetActive(false);
            //AutoTest.autoTest.TestPlayObject[1].SetActive(false);
        }
    }
    public void SetEffectInfo()
    {
        GameObject SelectedObject;
        Vector3 SelectedObjectPos;
        SelectedObject = NoteEdit.Selected;
        SelectedObjectPos = 
            NoteEdit.Selected.transform
            .GetChild(0).GetChild(0).GetChild(0)
            .GetChild(0).transform.localPosition;

        switch (GetEffectType(SelectedObjectPos))
        {
            // Teleport Effect Setting
            case 1:
                break;
            // FadeOut Effect Setting
            case 2:
                break;
            // Chromatic Effect Setting
            case 3:
                break;
            // Effect Error
            default:
                Debug.LogError("Effect Type Undefined");
                return;
        }
    }
    public int GetEffectType(Vector3 checkingObjectPos)
    {
        return (int)checkingObjectPos.x;
    }
    public float GetEffectVariable(Vector3 checkingObjectPos)
    {
        return checkingObjectPos.y;
    }
}
