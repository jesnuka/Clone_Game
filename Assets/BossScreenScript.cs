using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossScreenScript : MonoBehaviour
{
    public GameObject aiScreenReal;
    public GameObject aiScreenFalling;
    public SoundManager soundManager;

    public int healthCurrent;
    public int healthMax;

    public bool isActive;

    public GameObject explosionEffect;
    public GameObject explosionEffectParticle;

    public GameObject bossCutsceneManager;
    public GameObject screenParticle;

    public GameObject uiWhiteOut;
    public Color uiWhiteOutCurrColor;
    public Color uiWhiteOutCurrColorStart;
    public Color uiWhiteOutCurrColorMax;

    public bool whiteOutActive;

    public bool gameOverStarted;

    private void Awake()
    {
       // uiWhiteOut.GetComponent<Image>().color = uiWhiteOutCurrColorStart;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isActive)
        {
            if (other.attachedRigidbody != null && other.attachedRigidbody.GetComponent<BulletScript>())
            {
                BulletScript bullet = other.attachedRigidbody.GetComponent<BulletScript>();
                soundManager.PlaySound(SoundManager.Sound.enemyDieChicken, 1f, true, this.transform.position);
                screenParticle.GetComponent<ParticleSystem>().Play();
                healthCurrent -= 1;
                //Also play effect on screen???
                Destroy(other.transform.gameObject);
            }
        }
        
    }

    private void Update()
    {
        if(healthCurrent < 25 && healthCurrent > 10)
        {
            //half hp, display effect
        }
        if(healthCurrent < 10)
        {
            //Display last effect
        }

        if(healthCurrent <= 0)
        {
            //WIN!!!
            //Display explosion
            if(!gameOverStarted)
            {
                gameOverStarted = true;
                bossCutsceneManager.GetComponent<BossCutscene>().TriggerShake(6f);
                soundManager.PlaySound(SoundManager.Sound.bossExplosion, 1f, true, this.transform.position);
                soundManager.PlaySound(SoundManager.Sound.bossExplosion, 1f, true, this.transform.position);
                soundManager.PlaySound(SoundManager.Sound.bossExplosion, 1f, true, this.transform.position);
                explosionEffectParticle.GetComponent<ParticleSystem>().Play();
                bossCutsceneManager.GetComponent<BossCutscene>().DefeatBoss();

                Color colnew = uiWhiteOut.GetComponent<Image>().color;
                colnew.a = 0;
                //uiWhiteOut.GetComponent<Image>().color = colnew;
                //uiWhiteOut.GetComponent<Image>().color = colnew;
                //uiWhiteOut.GetComponent<Image>().CrossFadeAlpha(1, 5f, true);//= Color.Lerp(uiWhiteOutCurrColorStart, uiWhiteOutCurrColorMax, Mathf.PingPong(Time.time, 3));
                bossCutsceneManager.GetComponent<BossCutscene>().gameOver = true;
                bossCutsceneManager.GetComponent<BossCutscene>().SetExplosionColors();
            }
            
        }
    }

}
