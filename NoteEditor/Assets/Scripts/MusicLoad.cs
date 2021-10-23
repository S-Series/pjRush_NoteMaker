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

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    IEnumerator GetAudioClip()
    {
        string path;
        path = Application.dataPath + "/" + songName.text + ".mp3";
        Debug.Log(path);
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                yield break;
            }
            else
            {
                MusicClip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = MusicClip;
                audioSource.Play();
            }
        }
    }

    public void BUttonGetAudio()
    {
        StartCoroutine(GetAudioClip());
    }
}
