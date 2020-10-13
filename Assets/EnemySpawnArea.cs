using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnArea : MonoBehaviour
{
    public GameObject[] spawnableEnemyList;
    public bool spawnPoint; //If this transition point overall allows for spawning (Most do)
    public bool hasSpawned; //If has spawned, dont spam spawn

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Whee");
        if (collision.tag == "Player" && spawnPoint && !hasSpawned)
        {
            foreach(GameObject enemy in spawnableEnemyList)
            {
                enemy.GetComponent<EnemyController>().spawnAreaActive = true;
            }
            hasSpawned = true;
            //  player.isOnLadder = true;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && spawnPoint) //&& hasSpawned)
        {
            foreach (GameObject enemy in spawnableEnemyList)
            {
                enemy.GetComponent<EnemyController>().spawnAreaActive = false;
            }
            hasSpawned = false;
            // player.isOnLadder = false;
        }

    }
}
