using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteOption : MonoBehaviour
{
    [SerializeField] private GameObject[] NoteObject;
    [SerializeField] private SpriteRenderer[] StartRenderer;
    [SerializeField] private SpriteRenderer[] LongRenderer;
    [SerializeField] private SpriteRenderer[] EndRenderer;
    [SerializeField] private SpriteRenderer[] ArrowRenderer;
    public void ToSelected(bool _isSelected)
    {
        Color32 _color;
        if (_isSelected) { _color = new Color32(0, 255, 0, 255); }
        else { _color = new Color32(255, 255, 255, 255); }

        foreach (SpriteRenderer _renderer in StartRenderer) { _renderer.color = _color; }
        foreach (SpriteRenderer _renderer in LongRenderer) { _renderer.color = _color; }
        foreach (SpriteRenderer _renderer in EndRenderer) { _renderer.color = _color; }
        foreach (SpriteRenderer _renderer in ArrowRenderer) { _renderer.color = _color; }
    }
    public void VisibleSetting(int _type, bool _isVisible)
    {
        if (_type == 0)
        {
            StartRenderer[0].enabled = _isVisible;
            StartRenderer[1].enabled = _isVisible;
        }
        else if (_type == 1)
        {
            LongRenderer[0].enabled = _isVisible;
            LongRenderer[1].enabled = _isVisible;
        }
        else if (_type == 2)
        {
            EndRenderer[0].enabled = _isVisible;
            EndRenderer[1].enabled = _isVisible;
        }
    }
    public void ToLongNote(int _legnth, int _gameSpeed = 100)
    {
        bool _isLong;
        if (_legnth == 0) { _isLong = false; }
        else { _isLong = true; }

        LongRenderer[0].enabled = _isLong;
        LongRenderer[1].enabled = _isLong;
        EndRenderer[0].enabled = _isLong;
        EndRenderer[1].enabled = _isLong;

        Vector3 _pos;
        foreach(SpriteRenderer _renderer in EndRenderer)
        {
            _pos = _renderer.transform.localPosition;
            _pos.y = (_legnth * 5) / 2.0f * (_gameSpeed / 100.0f);
            _renderer.transform.localPosition = _pos;
        }

        Vector3 _scale;
        foreach(SpriteRenderer _renderer in LongRenderer)
        {
            _scale = _renderer.transform.localScale;
            _scale.y = _legnth / 4.0f * 0.9975f * (_gameSpeed / 100.0f);
            _renderer.transform.localScale = _scale;
        }

        /*
        if (_isGame)
        {
            NoteObject[0].transform.GetChild(0).localScale
                = new Vector3(1, 1.0f / AutoTest.testSpeed, 1);
            NoteObject[1].transform.GetChild(0).localScale
                = new Vector3(1, 1.0f / AutoTest.testSpeed, 1);
        }
        */
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
    public void EnableCollider(bool _isEnable)
    {
        foreach (SpriteRenderer renderer in StartRenderer)
            { renderer.transform.GetComponent<Collider2D>().enabled = _isEnable; }
        foreach (SpriteRenderer renderer in LongRenderer)
            { renderer.transform.GetComponent<Collider2D>().enabled = _isEnable; }
        foreach (SpriteRenderer renderer in EndRenderer)
            { renderer.transform.GetComponent<Collider2D>().enabled = _isEnable; }
    }
}
