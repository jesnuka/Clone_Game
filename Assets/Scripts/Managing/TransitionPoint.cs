using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public CameraManager cameraManager;
    public Direction transitionDirection;

    public bool isOneShot;


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
                    cameraManager.TransitionLeft();
                    transitionDirection = Direction.right;
                    break;
                case Direction.right:
                    cameraManager.TransitionRight();
                    transitionDirection = Direction.left;
                    break;
                case Direction.up:
                    cameraManager.TransitionUp();
                    transitionDirection = Direction.down;
                    break;
                case Direction.down:
                    cameraManager.TransitionDown();
                    transitionDirection = Direction.up;
                    break;
            }
            

        }
    }
}
