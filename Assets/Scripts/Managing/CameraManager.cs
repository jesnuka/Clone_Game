using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    PlayerController2D playerController;
    GameObject playerObject;

    [SerializeField]
    float cameraMoveTime;
    [SerializeField]
    float cameraMoveSpeed;

    [SerializeField]
    Transform currentTransform;


    //Transform transitionBegin;
    Transform transitionTarget;

    //Use this when calling TransitionCamera function
    //It will select the specific begin and end transitions from the list
    //They need to be manually placed there currently and kept track of though
    public int transitionIndex;


    public Mode currentMode;

    public enum Mode
    {
        Follow,
        Stationary,
        Transition
    }

    private void Awake()
    {
        currentMode = Mode.Follow;

        playerObject = GameObject.FindWithTag("Player");
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController2D>();

    }

    private void Update()
    {
        switch (currentMode)
        {
            case Mode.Follow:
                FollowPlayer();
                break;
        }
    }

    void FollowPlayer()
    {
        this.transform.position = new Vector3(playerObject.transform.position.x, this.transform.position.y, this.transform.position.z);
    }

    private void LateUpdate()
    {
        if(currentMode == Mode.Transition)
        {
            TransitionCamera(transitionIndex);
        }
    }

    
    public void TransitionLeft()
    {

    }
    public void TransitionRight()
    {

    }
    public void TransitionUp()
    {

    }
    public void TransitionDown()
    {

    }

    public void TransitionCamera(int i) //The value i is the direction. 0 = left, 1 = right, 2 = up, 3 = down
    {
        if (currentMode == Mode.Transition)
        {
            cameraMoveTime += Time.deltaTime * cameraMoveSpeed;
            transform.position = Vector3.Lerp(this.transform.position, transitionTarget.position, cameraMoveTime);
        }

        if (cameraMoveTime >= 1)
        {
            currentMode = Mode.Stationary;
            transform.position = transitionTarget.position;
        }
    }

    void SwitchMode(Mode mode)
    {
        switch (mode)
        {
            case Mode.Follow:
                currentMode = Mode.Follow;
                break;

            case Mode.Stationary:
                currentMode = Mode.Stationary;
                break;

            case Mode.Transition:
                currentMode = Mode.Transition;
                break;
        }
    }
}
