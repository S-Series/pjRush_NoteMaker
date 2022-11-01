using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEdit : MonoBehaviour
{
    private static LineEdit s_lineEdit;
    public static bool isLineEdit = false;
    public static LineNote selectLineNote;
    private LineOption selectLineOption;
    [SerializeField] private GameObject prefabObject;

    void Awake()
    {
        s_lineEdit = this;
    }
    void Update()
    {
        if (!isLineEdit) { return; }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectNote(); return;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

        }
    }

    public static void DeselectNote()
    {
        isLineEdit = false;
        selectLineNote = null;
        s_lineEdit.selectLineOption = null;
    }
    public static void SelectLineNote(GameObject _object)
    {
        selectLineNote = LineNote.GetLineNote(_object);
        s_lineEdit.selectLineOption = _object.GetComponent<LineOption>();
    }
    private static void MoveNote(bool _isUp)
    {
        Vector3 pos;
        pos = new Vector3(0, selectLineNote.pos, 0);

        int count;
        float inputPos;

        if (_isUp)
        {
            count = Mathf.FloorToInt(pos.y / (1600.0f / GuideGenerate.GuideCount)) + 1;
            inputPos = Mathf.Floor(1600.0f / GuideGenerate.GuideCount * 100) * count / 100;
            if (inputPos >= 1600.0f * 999) inputPos = 1600.0f * 999;
            pos.y = inputPos;

            while (LineNote.IsNoteOverlap(inputPos))
            {
                count = Mathf.FloorToInt(pos.y / (1600.0f / GuideGenerate.GuideCount)) + 1;
                inputPos = Mathf.Floor(1600.0f / GuideGenerate.GuideCount * 100) * count / 100;
                if (inputPos >= 1600.0f * 999) inputPos = 1600.0f * 999;
                pos.y = inputPos;
            }
        }
        else
        {
            count = Mathf.CeilToInt(pos.y / (1600.0f / GuideGenerate.GuideCount)) - 1;
            inputPos = Mathf.Ceil(1600.0f / GuideGenerate.GuideCount * 100) * count / 100;
            if (inputPos < 0.0f) inputPos = 0.0f;
            pos.y = inputPos;

            while (LineNote.IsNoteOverlap(inputPos))
            {
                count = Mathf.CeilToInt(pos.y / (1600.0f / GuideGenerate.GuideCount)) - 1;
                inputPos = Mathf.Ceil(1600.0f / GuideGenerate.GuideCount * 100) * count / 100;
                if (inputPos < 0.0f) inputPos = 0.0f;
                pos.y = inputPos;
            }
        }
    }
}
