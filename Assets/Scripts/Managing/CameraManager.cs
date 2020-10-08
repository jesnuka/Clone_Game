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
    float cameraMoveTimeMax;
    [SerializeField]
    float cameraMoveSpeed;

    [SerializeField]
    Transform currentTransform;


    //This are used to lock movement in X axis of going too far to the sides. 
    //Seperate value for each area that has follow on, 3 total
    public int followAreaValue;
    public float followLimitX1L;
    public float followLimitX1R;

    public float followLimitX2L;
    public float followLimitX2R;

    public float followLimitX3L;
    public float followLimitX3R;

    //Transform transitionBegin;
    Vector3 transitionTarget;

    bool followsAfter;

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
        
        switch (followAreaValue)
        {
            case 1:
                this.transform.position = new Vector3(Mathf.Clamp(playerObject.transform.position.x, followLimitX1L, followLimitX1R), this.transform.position.y, this.transform.position.z);
                break;
            case 2:
                this.transform.position = new Vector3(Mathf.Clamp(playerObject.transform.position.x, followLimitX2L, followLimitX2R), this.transform.position.y, this.transform.position.z);
                break;
            case 3:
                this.transform.position = new Vector3(Mathf.Clamp(playerObject.transform.position.x, followLimitX3L, followLimitX3R), this.transform.position.y, this.transform.position.z);
                break;

        }
       
    }

    private void LateUpdate()
    {
        if(currentMode == Mode.Transition)
        {
            TransitionCamera(transitionIndex);
        }
    }

    
    public void TransitionLeft(bool followValue)
    {
        cameraMoveTime = 0;
        transitionIndex = 0;
        currentMode = Mode.Transition;
        transitionTarget = new Vector3(transform.position.x - stageDimensions.x, transform.position.y, transform.position.z);
        if(followValue)
        {
            Debug.Log("follow? "+ followValue);
            followsAfter = true;
        }
    }
    public void TransitionRight(bool followValue)
    {
        cameraMoveTime = 0;
        transitionIndex = 1;
        currentMode = Mode.Transition;
        //transitionTarget = new Vector3(transform.position.x + Camera.main.pixelWidth, transform.position.y, 0f);
        transitionTarget = new Vector3(transform.position.x + stageDimensions.x, transform.position.y, transform.position.z);
        if (followValue)
        {
            Debug.Log("follow? " + followValue);
            followsAfter = true;
        }
    }
    public void TransitionUp(bool followValue)
    {
        cameraMoveTime = 0;
        transitionIndex = 2;
        currentMode = Mode.Transition;
        transitionTarget = new Vector3(transform.position.x, transform.position.y + stageDimensions.y, transform.position.z);
        if (followValue)
        {
            Debug.Log("follow? " + followsAfter);
            followsAfter = true;
        }
    }
    public void TransitionDown(bool followValue)
    {
        cameraMoveTime = 0;
        transitionIndex = 3;
        currentMode = Mode.Transition;
        transitionTarget = new Vector3(transform.position.x, transform.position.y - stageDimensions.y, transform.position.z);
        if (followValue)
        {
            Debug.Log("follow? " + followValue);
            followsAfter = true;
        }
    }

    public void TransitionCamera(int i) //The value i is the direction. 0 = left, 1 = right, 2 = up, 3 = down
    {
        if (currentMode == Mode.Transition)
        {
            cameraMoveTime += Time.deltaTime * cameraMoveSpeed;
            transform.position = Vector3.Lerp(this.transform.position, transitionTarget, cameraMoveTime);
        }

        Debug.Log("follow? " + followsAfter);

        if (cameraMoveTime >= cameraMoveTimeMax && !followsAfter)
        {
            Debug.Log("follaaaaow? "+followsAfter);
            currentMode = Mode.Stationary;
            transform.position = transitionTarget;
        }
        else if(cameraMoveTime >= cameraMoveTimeMax && followsAfter)
        {
            Debug.Log("follobbbbbw? " + followsAfter);
            transform.position = transitionTarget;
            currentMode = Mode.Follow;
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
