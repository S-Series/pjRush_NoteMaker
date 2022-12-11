using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioManager audioManager;

    public static class GameAudio
    {
        public static AudioSource GameMusic;
        public static AudioSource HitEffects;
        
        public static void Overide(GameObject gameObject)
        {
            
        }
    }
    public static class AudioEffect
    {
        public static void Overide(GameObject gameObject)
        {
            
        }
    }

    private void Awake()
    {
        audioManager = this;
        //GameAudio.Overide(audioManager.AudioObjects[0]);
    }
    [SerializeField] GameObject[] AudioObjects;
}
