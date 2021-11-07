using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class MusicLoad : MonoBehaviour
{
    AudioSource audioSource;
    AudioClip MusicClip;

    [SerializeField]
    TMP_InputField songName;

    [SerializeField]
    GameObject loadSuccessCheck;

    string SavedSongName;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        loadSuccessCheck.SetActive(false);

        try
        {
            SavedSongName = PlayerPrefs.GetString("SongName");
            songName.text = SavedSongName;
            StartCoroutine(GetAudioClip());
        }
        catch 
        {
            ResetSave();
        }
    }

    IEnumerator GetAudioClip()
    {
        loadSuccessCheck.SetActive(false);
        string path;
        path = Application.dataPath + "/" + songName.text + ".mp3";
        // Debug.Log(path);
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                ResetSave();
                yield break;
            }
            else
            {
                try
                {
                    PlayerPrefs.SetString("SongName", songName.text);
                    loadSuccessCheck.SetActive(true);
                    MusicClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = MusicClip;
                }
                catch { ResetSave(); }
            }
        }
    }

    private void ResetSave()
    {
        PlayerPrefs.SetString("SongName", "");
        songName.text = "";
    }

    public void BUttonGetAudio()
    {
        StartCoroutine(GetAudioClip());
    }
}
