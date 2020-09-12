using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startPos;
    public GameObject mainCam;
    public float parallaxEffect;

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
        float temp = (mainCam.transform.position.x * (1 - parallaxEffect));
        float distance = (mainCam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if(temp > startPos + length)
        {
            startPos += length;
        }
        else if(temp < startPos - length)
        {
            startPos -= length;
        }
    }
}
