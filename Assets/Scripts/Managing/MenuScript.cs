using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
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
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene_Level1");
    }

    public void Credits()
    {
        menuElements.SetActive(false);
        creditsElements.SetActive(true);
    }

    public void Controls()
    {
        menuElements.SetActive(false);
        controlsElements.SetActive(true);
    }


    public void returnToMenu()
    {
        menuElements.SetActive(true);
        controlsElements.SetActive(false);
        creditsElements.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    
}
