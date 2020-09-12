using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    PlayerController2D player;

    private void Awake()
    {
        if(player == null)
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
