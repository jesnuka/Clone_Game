using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

public class BossCutscene : MonoBehaviour
{
    public PlayerController2D player;
    public SoundManager soundManager;
    public float lightCounter;
    public float lightCounter2;
    public float lightCounter3;

    public float lightCounterMax;

    bool light1On;
    bool light2On;
    bool light3On;

    bool allLightsOn;

    public GameObject lightObj1;
    public GameObject lightObj2;
    public GameObject lightObj3;

    public int LightAmount;

    public bool cutsceneStarted;
    public bool cutscenePlaying;

    public GameObject globalLight;
    public float globalLightIntensityOG;
    
    public Color globalLightColorOG;

    public Camera mainCam;
    public Color globalLightColorBoss;
    
    public float globalLightIntensityBoss;
    public Color globalLightColorBossDEAD;

    public EnemyController bossController;

    public AudioSource bossMusic;
    public bool musicStarted;

    public GameObject bossScreen;
    public GameObject bossTentacleR;
    public GameObject bossTentacleL;

    public AudioSource bossAmbience;

    public float camShakeDur;
    public float camShakeMagn;
    public float camShakeDamp;

    public Vector3 camShakeInitialPos;

    public bool camIsShaking;

    public bool gameOver;
    public GameObject uiWhiteOut;

    private void Awake()
    {
        ResetStats();
        mainCam = Camera.main;
    }
    public void DefeatBoss()
    {
        //Win
        bossController.RemoveHealth(2000); //Insta kill
    }
    public void ShakeScreen()
    {
        if(gameOver)
        {
            if(camShakeDur < 2)
            {
                uiWhiteOut.SetActive(true);
            }
        }
        if(camShakeDur > 0)
        {
            mainCam.transform.localPosition = camShakeInitialPos + Random.insideUnitSphere * camShakeMagn;
            camShakeDur -= Time.deltaTime * camShakeDamp;

        }
        else
        {
            camIsShaking = false;
            camShakeDur = 0f;
            mainCam.transform.localPosition = camShakeInitialPos;
            if(gameOver)
            {
                SceneManager.LoadScene("EndCreditsScene");
            }
        }
    }
    public void ResetStats()
    {
        player.inBossFight = false;
        bossController.roosterCutsceneOver = false;
        allLightsOn = false;
        lightObj1.SetActive(false);
        lightObj2.SetActive(false);
        lightObj3.SetActive(false);
        light1On = false;
        light2On = false;
        light3On = false;
        musicStarted = false;
        globalLight.GetComponent<Light2D>().intensity = globalLightIntensityOG;
        globalLight.GetComponent<Light2D>().color = globalLightColorOG;
        cutsceneStarted = false;
        cutscenePlaying = false;
        lightCounter = lightCounterMax;
        bossScreen.GetComponent<BossScreenScript>().isActive = false;
        bossAmbience.Stop();
        bossScreen.GetComponent<BossScreenScript>().isActive = false;
        bossScreen.GetComponent<BossScreenScript>().healthCurrent = bossScreen.GetComponent<BossScreenScript>().healthMax;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.GetComponent<PlayerController2D>())
        {
            if(!cutsceneStarted)
            {
                PlayerController2D player = other.attachedRigidbody.GetComponent<PlayerController2D>();
                player.inBossFight = true;
                globalLight.GetComponent<Light2D>().intensity = 0;
                cutsceneStarted = true;
                cutscenePlaying = true;
                lightCounter = lightCounterMax;
            }
            
        }
    }

    public void SetExplosionColors()
    {
        globalLight.GetComponent<Light2D>().color = globalLightColorBossDEAD;
    }

    public void TriggerShake(float dur)
    {
        camShakeDur = dur;
        camIsShaking = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(camIsShaking)
        {
            ShakeScreen();
        }
        if(cutscenePlaying)
        {
            lightCounter -= Time.deltaTime;
            if(lightCounter < 0 && !light1On && !light2On && !light3On)
            {
               // Debug.Log("zip");
                soundManager.PlaySound(SoundManager.Sound.bossLights, 0.5f, false, Vector3.zero);
                lightCounter = lightCounterMax;
                lightObj1.SetActive(true);
                light1On = true;
            }

            if (lightCounter < 0 && light1On && !light2On && !light3On)
            {
               // Debug.Log("zoop");
                soundManager.PlaySound(SoundManager.Sound.bossLights, 0.5f, false, Vector3.zero);
                lightCounter = lightCounterMax;
                lightObj2.SetActive(true);
                light2On = true;
            }

            if (lightCounter < 0 && light1On && light2On && !light3On)
            {
                light3On = true;
              //  Debug.Log("zappe");
                soundManager.PlaySound(SoundManager.Sound.bossLights, 0.5f, false, Vector3.zero);
                lightCounter = lightCounterMax;
                lightObj3.SetActive(true);
                allLightsOn = true;
                
            }


            if(allLightsOn)
            {
                
                bossTentacleL.GetComponent<Animator>().Play("Boss_LeftTentacle");
                bossTentacleR.GetComponent<Animator>().Play("Boss_RightTentacle");
                
                bossController.roosterCutsceneOver = true;
                if(!musicStarted)
                {
                    globalLight.GetComponent<Light2D>().intensity = globalLightIntensityBoss;
                    globalLight.GetComponent<Light2D>().color = globalLightColorBoss;
                    bossScreen.GetComponent<BossScreenScript>().isActive = true;
                    bossAmbience.Play();
                    musicStarted = true;
                    bossMusic.Play();
                    bossScreen.GetComponent<BossScreenScript>().isActive = true;
                    TriggerShake(4.0f);
                }
                
                //Do boss roar here etc, start fight!
            }
        }
    }
}
