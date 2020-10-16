using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverAnimator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Animator>().Play("Drone_Attack1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
