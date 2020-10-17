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

    public Sprite bossHealthBarBlock;
    public Sprite bossHealthBarEmpty;

    public float bossHudY;
    public float bossHudX;

    public float bossBarHudX;
    public float bossBarHudY;

    public float hudBarXAlt;
    public float hudBarYAlt;

    public bool gameOverStarted;
    public GameObject bossHUDObject;

    private void OnGUI()
    {
        if (isActive)
        {
            //Size of the GUI items
            float x = Camera.main.pixelWidth / 256.0f;
            float y = Camera.main.pixelHeight / 218.0f;
            Vector2 cmrBase = new Vector2(Camera.main.rect.x * Screen.width - bossBarHudX, Camera.main.rect.y * Screen.height + bossBarHudY);
            bossHUDObject.transform.position = new Vector3(Camera.main.transform.position.x + bossHudX, Camera.main.transform.position.y + bossHudY, 10f);

            Sprite healthBar = bossHealthBarBlock;

            Rect healthBarRect = new Rect(healthBar.rect.x / healthBar.texture.width, healthBar.rect.y / healthBar.texture.height,
                                    healthBar.rect.width / healthBar.texture.width, healthBar.rect.height / (healthBar.texture.height));
            //  Rect healthBarRect = new Rect(healthBar.rect.width / healthBar.texture.width, healthBar.rect.height / healthBar.texture.height, 
            //                        healthBar.rect.x / healthBar.texture.width, healthBar.rect.y / healthBar.texture.height);

            Sprite emptyBar = bossHealthBarEmpty;
            Rect emptyBarRect = new Rect(emptyBar.rect.x / emptyBar.texture.width, emptyBar.rect.y / emptyBar.texture.height,
                                    emptyBar.rect.width / emptyBar.texture.width, emptyBar.rect.height / emptyBar.texture.height);

            // Rect emptyBarRect = new Rect(emptyBar.rect.width / emptyBar.texture.width, emptyBar.rect.height / emptyBar.texture.height,
            //                        emptyBar.rect.x / emptyBar.texture.width, emptyBar.rect.y / emptyBar.texture.height);
          /*  for (int i = 0; i < healthMax / 5; i++)
            {
                if (healthCurrent / 5 > i)
                    GUI.DrawTextureWithTexCoords(new Rect(cmrBase.x + x * 24f, cmrBase.y + y * (72 - i * 2), x * 8, y * 2), healthBar.texture, healthBarRect);
                else
                    GUI.DrawTextureWithTexCoords(new Rect(cmrBase.x + x * 24f, cmrBase.y + y * (72 - i * 2), x * 8, y * 2), emptyBar.texture, emptyBarRect);
            }*/

            for (int i = 0; i < healthMax; i++)
            {
                if (healthCurrent > i)
                    GUI.DrawTextureWithTexCoords(new Rect(cmrBase.x + y * (72 - i * 1.25f), cmrBase.y + x * 24f, y * 2, x * 3), healthBar.texture, healthBarRect);
                else
                    GUI.DrawTextureWithTexCoords(new Rect(cmrBase.x + y * (72 - i * 1.25f), cmrBase.y + x * 24f, y * 2, x * 3), emptyBar.texture, emptyBarRect);
            }

    
        }


    }
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
        if(isActive)
        {
            bossHUDObject.SetActive(true);
        }
        else
        {
            bossHUDObject.SetActive(false);
        }
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
