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
    Vector3 transitionTarget;

    //Use this when calling TransitionCamera function
    //It will select the specific begin and end transitions from the list
    //They need to be manually placed there currently and kept track of though
    public int transitionIndex;
    Vector3 stageDimensions;

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
        stageDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
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
        transitionIndex = 0;
        currentMode = Mode.Transition;
        transitionTarget = new Vector3(transform.position.x - stageDimensions.x, transform.position.y, transform.position.z);
    }
    public void TransitionRight()
    {
        transitionIndex = 1;
        currentMode = Mode.Transition;
        //transitionTarget = new Vector3(transform.position.x + Camera.main.pixelWidth, transform.position.y, 0f);
        transitionTarget = new Vector3(transform.position.x + stageDimensions.x, transform.position.y, transform.position.z);
    }
    public void TransitionUp()
    {
        transitionIndex = 2;
        currentMode = Mode.Transition;
        transitionTarget = new Vector3(transform.position.x, transform.position.y + stageDimensions.y, transform.position.z);
    }
    public void TransitionDown()
    {
        transitionIndex = 3;
        currentMode = Mode.Transition;
        transitionTarget = new Vector3(transform.position.x, transform.position.y - stageDimensions.y, transform.position.z);
    }

    public void TransitionCamera(int i) //The value i is the direction. 0 = left, 1 = right, 2 = up, 3 = down
    {
        if (currentMode == Mode.Transition)
        {
            cameraMoveTime += Time.deltaTime * cameraMoveSpeed;
            transform.position = Vector3.Lerp(this.transform.position, transitionTarget, cameraMoveTime);
        }

        if (cameraMoveTime >= 1)
        {
            currentMode = Mode.Stationary;
            transform.position = transitionTarget;
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
