using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteOption : MonoBehaviour
{
    [SerializeField] private GameObject[] NoteObject;
    [SerializeField] private SpriteRenderer[] LongRenderer;
    [SerializeField] private SpriteRenderer[] EndRenderer;
    [SerializeField] private SpriteRenderer[] ArrowRenderer;
    public void ToLongNote(int _Legnth, bool _isGame = false)
    {
        bool _isLong;
        if (_Legnth == 0) { _isLong = false; }
        else { _isLong = true; }

        LongRenderer[0].enabled = _isLong;
        LongRenderer[1].enabled = _isLong;
        EndRenderer[0].enabled = _isLong;
        EndRenderer[1].enabled = _isLong;

        Vector3 _pos;
        foreach(SpriteRenderer _renderer in EndRenderer)
        {
            _pos = _renderer.transform.localPosition;
            if (_isGame) { _pos.y = (_Legnth * 5) / 2.0f * AutoTest.testSpeed; }
            else { _pos.y = (_Legnth * 5) / 2.0f; }
            _renderer.transform.localPosition = _pos;
        }

        Vector3 _scale;
        foreach(SpriteRenderer _renderer in LongRenderer)
        {
            _scale = _renderer.transform.localScale;
            if (_isGame) { _scale.y = _Legnth / 4.0f * AutoTest.testSpeed; }
            else { _scale.y = _Legnth / 4.0f; }
            _renderer.transform.localScale = _scale;
        }

        if (CompareTag("Bottom")) { ArrowRenderer[1].enabled = _isLong; }
    }
    public void ToPoweredNote(bool _isPowered)
    {
        NoteObject[0].SetActive(!_isPowered);
        NoteObject[1].SetActive(_isPowered);
        if (CompareTag("Bottom"))
        {
            ArrowRenderer[0].enabled = _isPowered;
            ArrowRenderer[1].enabled = _isPowered;
            if (EndRenderer[0].transform.localPosition.y == 0)
            {
                ArrowRenderer[1].enabled = false;
            }
        }
    }
    public void ToGameMode(bool _isGameMode)
    {
        if (!CompareTag("Bottom")) { return; }

        if (_isGameMode)
        {
            foreach(SpriteRenderer renderer in ArrowRenderer)
            {
                renderer.transform.localPosition = new Vector3(0, 0, -1);
                renderer.transform.localRotation = Quaternion.Euler(90, 0, 0);
            }
        }
        else
        {
            foreach(SpriteRenderer renderer in ArrowRenderer)
            {
                renderer.transform.localPosition = new Vector3(0, 0, -1);
                renderer.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
