using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public SoundAudioClip[] audioClipArray;

    private Dictionary<Sound, float> soundTimerDictionary;
    SoundManager soundManager;

    public enum Sound
    {
        test,
        playerShoot,
        playerPickup,
        playerJump,
        playerTakeDamage,
        playerLand,
        enemyDieChicken,
        enemyDieWalker,
        enemyDieDrone,
        enemyDiePipe,
        enemyTakeDamage,
        pipeShoot,
        bossShoot,
        bossSound1,
        zombieShoot,
        menuSelect,
        menuMove,
        walkerMove,
        chickenJump,
        bossLights,
        bossExplosion,
        bossRoar,




       // delay,
    }

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
       // soundTimerDictionary[Sound.delay] = 0f;
    }

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }

    AudioClip GetAudioClip(Sound sound)
    {
        foreach (SoundAudioClip soundAudioClip in audioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + "not found!");
        return null;
    }

    private bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            default:
                //True for most sounds
                //If sound is not played only once, then add a case
                return true;
         /*   case Sound.delay:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = .15f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
                break;*/
        }
    }

    public void PlaySound(Sound sound, float volume, bool is3d, Vector3 pos)
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.volume = volume;
            if (is3d)
            {
                audioSource.spatialBlend = 0.75f;
                audioSource.gameObject.transform.position = pos;
            }
            audioSource.PlayOneShot(GetAudioClip(sound));
           
            

            Object.Destroy(soundGameObject, GetAudioClip(sound).length);
        }
    }
}
