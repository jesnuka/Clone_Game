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


    public enum Direction
    {
        left,
        right,
        up,
        down
    }

    private void Awake()
    {
        cameraManager = Camera.main.gameObject.GetComponent<CameraManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.GetComponent<PlayerController2D>())
        {
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
                    cameraManager.TransitionUp(followsAfter, amount);
                    transitionDirection = Direction.down;
                    break;
                case Direction.down:
                    cameraManager.TransitionDown(followsAfter, amount);
                    transitionDirection = Direction.up;
                    break;
            }
            
            if(isOneShot)
            {
                Destroy(this.gameObject);
            }
            

        }
    }
}
