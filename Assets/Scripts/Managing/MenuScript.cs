using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public SoundManager soundManager;

    public GameObject menuElements;
    public GameObject creditsElements;
    public GameObject controlsElements;
    AudioSource menuAudioSource;

    private void Awake()
    {
        if(menuAudioSource == null)
        {
            menuAudioSource = this.GetComponent<AudioSource>();
        }
    }

    public void PlayMoveSound()
    {
        soundManager.PlaySound(SoundManager.Sound.menuMove, 1f, false, Vector3.zero);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene_Level1");
        soundManager.PlaySound(SoundManager.Sound.menuSelect, 1f, false, Vector3.zero);
    }

    public void Credits()
    {
        soundManager.PlaySound(SoundManager.Sound.menuSelect, 1f, false, Vector3.zero);
        menuElements.SetActive(false);
        creditsElements.SetActive(true);
    }

    public void Controls()
    {
        soundManager.PlaySound(SoundManager.Sound.menuSelect, 1f, false, Vector3.zero);
        menuElements.SetActive(false);
        controlsElements.SetActive(true);
    }


    public void returnToMenu()
    {
        soundManager.PlaySound(SoundManager.Sound.menuSelect, 1f, false, Vector3.zero);
        menuElements.SetActive(true);
        controlsElements.SetActive(false);
        creditsElements.SetActive(false);
    }

    public void ReturnToMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
        soundManager.PlaySound(SoundManager.Sound.menuSelect, 1f, false, Vector3.zero);

    }
    public void Quit()
    {
        soundManager.PlaySound(SoundManager.Sound.menuSelect, 1f, false, Vector3.zero);
        Application.Quit();
    }

    
}
