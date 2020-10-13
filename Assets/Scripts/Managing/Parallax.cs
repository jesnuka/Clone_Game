using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startPos;
    public GameObject mainCam;
    public float parallaxEffect;

    public bool background2;
    public float fastY;

    private void Awake()
    {
        if(mainCam == null)
        {
            mainCam = GameObject.FindWithTag("MainCamera");
        }
    }
    private void Start()
    {

        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        if(!background2)
        {
            float temp = (mainCam.transform.position.x * (1 - parallaxEffect));
            float distance = (mainCam.transform.position.x * parallaxEffect);

            transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
        }
        else if(parallaxEffect == 0)
        {
            transform.position = new Vector3(mainCam.transform.position.x, fastY, 0);
        }
        else
        {
            float temp = (mainCam.transform.position.x * (1 - parallaxEffect));
            float distance = (mainCam.transform.position.x * parallaxEffect);
            float distance2 = (mainCam.transform.position.x * parallaxEffect);

            transform.position = new Vector3(167.8303f + distance, transform.position.y, transform.position.z);
        }
        

      /*  if(temp > startPos + length)
        {
            startPos += length;
        }
        else if(temp < startPos - length)
        {
            startPos -= length;
        }*/
    }
}
