using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public CameraManager cameraManager;
    public Direction transitionDirection;
    public float amount;

    public bool isOneShot;
    public bool followsAfter;
    public bool wasFollowAfter;

    public int followValueAfter;

    public bool blocksEntrance;
    public GameObject blockObject;

    public bool togglesBackground;

   



    public enum Direction
    {
        left,
        right,
        up,
        down,
        none,
        boss
    }

    private void Awake()
    {
        cameraManager = Camera.main.gameObject.GetComponent<CameraManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.GetComponent<PlayerController2D>())
        {
            if (wasFollowAfter)
            {
                Debug.Log("ok");
               // followsAfter = true;
              //  wasFollowAfter = false;
            }
            if (followsAfter)
            {
                    cameraManager.followAreaValue = followValueAfter;
            }

            PlayerController2D player = other.attachedRigidbody.GetComponent<PlayerController2D>();
            switch (transitionDirection)
            {
                case Direction.left:
                    cameraManager.TransitionLeft(followsAfter, amount);
                    transitionDirection = Direction.right;
                    break;
                case Direction.right:
                    cameraManager.TransitionRight(followsAfter, amount);
                    transitionDirection = Direction.left;
                    break;
                case Direction.up:
                    if(togglesBackground)
                    {
                        cameraManager.TransitionUp(followsAfter, amount, true);
                    }
                    else
                    {
                        cameraManager.TransitionUp(followsAfter, amount, false);
                    }
                    transitionDirection = Direction.down;
                    break;
                case Direction.down:
                    if(togglesBackground)
                    {
                        cameraManager.TransitionDown(followsAfter, amount, true);
                    }
                    else
                    {
                        cameraManager.TransitionDown(followsAfter, amount, false);
                    }
                    
                    transitionDirection = Direction.up;
                    break;
                case Direction.none:
                    break;
                case Direction.boss:
                    //Boss fight camera
                    cameraManager.TransitionBoss(amount);
                    //transitionDirection = Direction.left;
                    break;
            }
            if (wasFollowAfter)
            {
                followsAfter = true;
                wasFollowAfter = false;
            }

            else if (followsAfter)
            {
                //cameraManager.followAreaValue = followValueAfter;
                // cameraManager.SwitchMode(CameraManager.Mode.Follow);
                wasFollowAfter = true;
                followsAfter = false;
            }
            
            if(blocksEntrance)
            {
                blockObject.SetActive(true);
            }

            if(isOneShot)
            {
               this.gameObject.SetActive(false);
            }
            

        }
    }
}
