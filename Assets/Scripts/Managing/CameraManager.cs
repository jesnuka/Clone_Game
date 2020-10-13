using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    PlayerController2D playerController;
    GameObject playerObject;
    public GameObject background;
    public GameObject background2;

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


    public Vector3 resetPos1;
    public Vector3 resetPos2;
    public Vector3 resetPos3;

    //Transform transitionBegin;
    Vector3 transitionTarget;

    public bool followsAfter;

    //Use this when calling TransitionCamera function
    //It will select the specific begin and end transitions from the list
    //They need to be manually placed there currently and kept track of though
    public int transitionIndex;
    public bool followNext;

    public float backgroundTimer;
    public float backgroundTimerMax;
    public bool backgroundToggleOn;

    public Vector3 bossfightTransform;

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
        if(backgroundToggleOn)
        {
            backgroundTimer -= Time.deltaTime;
            ToggleBackground();
        }
        
        switch (currentMode)
        {
            case Mode.Follow:
                FollowPlayer();
                break;
        }

    }

    public void ResetCamera(int checkId)
    {
        if(checkId == 0)
        {
            transform.position = resetPos1;
            background.SetActive(true);
            followAreaValue = 1;
            currentMode = Mode.Follow;
        }
        else if (checkId == 1)
        {
            currentMode = Mode.Stationary;
            transform.position = resetPos2;
            background2.SetActive(false);

        }
        else if (checkId == 2)
        {
            transform.position = resetPos3;
            followAreaValue = 3;
            currentMode = Mode.Follow;
            this.GetComponent<Camera>().orthographicSize = 9;
        }
    }

    void ToggleBackground()
    {
        if(backgroundTimer < 0)
        {
            if (background.activeSelf || background2.activeSelf)
            {
                background.SetActive(false);
                background2.SetActive(false);
                backgroundToggleOn = false;
            }
            
            else
            {
                background2.SetActive(true);
                backgroundToggleOn = false;
            }

        }
    }
    void FollowPlayer()
    {
        
        switch (followAreaValue)
        {
            case 0: //Testing only, follows everywhere
                this.transform.position = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, transform.position.z);
                break;
            case 1:
                this.transform.position = new Vector3(Mathf.Clamp(playerObject.transform.position.x, followLimitX1L, followLimitX1R), this.transform.position.y, this.transform.position.z);
                break;
            case 2:
                this.transform.position = new Vector3(Mathf.Clamp(playerObject.transform.position.x, followLimitX2L, followLimitX2R), this.transform.position.y, this.transform.position.z);
                break;
            case 3:
                this.transform.position = new Vector3(Mathf.Clamp(playerObject.transform.position.x, followLimitX3L, followLimitX3R), this.transform.position.y, this.transform.position.z);
                break;
            case 4:
                //Don't follow this time
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

    
    public void TransitionLeft(bool followValue, float amount)
    {
        cameraMoveTime = 0;
        transitionIndex = 0;
        currentMode = Mode.Transition;
        transitionTarget = new Vector3(transform.position.x - stageDimensions.x * amount, transform.position.y , transform.position.z);
        playerController.PushPlayer(0);
        if(followValue)
        {
            followsAfter = true;
        }
    }
    public void TransitionRight(bool followValue, float amount)
    {
        cameraMoveTime = 0;
        transitionIndex = 1;
        currentMode = Mode.Transition;
        //transitionTarget = new Vector3(transform.position.x + Camera.main.pixelWidth, transform.position.y, 0f);
        transitionTarget = new Vector3(transform.position.x + stageDimensions.x * amount, transform.position.y, transform.position.z);
        playerController.PushPlayer(1);
        if (followValue)
        {
            followsAfter = true;
        }

    }
    public void TransitionUp(bool followValue, float amount, bool toggleBackground)
    {
        cameraMoveTime = 0;
        transitionIndex = 2;
        currentMode = Mode.Transition;
        transitionTarget = new Vector3(transform.position.x, transform.position.y + stageDimensions.y * amount, transform.position.z);
        playerController.PushPlayer(2);
        if (followValue)
        {
            Debug.Log("FollowAfter is true now in camera");
            followsAfter = true;
        }
        if (toggleBackground)
        {

            backgroundToggleOn = true;
            backgroundTimer = backgroundTimerMax;
        }
    }
    public void TransitionDown(bool followValue, float amount, bool toggleBackground)
    {
        cameraMoveTime = 0;
        transitionIndex = 3;
        currentMode = Mode.Transition;
        transitionTarget = new Vector3(transform.position.x, transform.position.y - stageDimensions.y * amount, transform.position.z);
        playerController.PushPlayer(3);
        if (followValue)
        {
            followsAfter = true;
        }
        if (toggleBackground)
        {
            backgroundToggleOn = true;
            backgroundTimer = backgroundTimerMax;
        }
    }
    public void TransitionBoss(float amount)
    {
        cameraMoveTime = 0;
        transitionIndex = 1;
        currentMode = Mode.Transition;
        //transitionTarget = new Vector3(transform.position.x + Camera.main.pixelWidth, transform.position.y, 0f);
       // transitionTarget = new Vector3(transform.position.x + stageDimensions.x * amount, transform.position.y, transform.position.z);
        transitionTarget = new Vector3(bossfightTransform.x, bossfightTransform.y, transform.position.z);
        playerController.PushPlayer(1);
        this.GetComponent<Camera>().orthographicSize = 15;

    }
    public void TransitionCamera(int i) //The value i is the direction. 0 = left, 1 = right, 2 = up, 3 = down
    {
        if (currentMode == Mode.Transition)
        {
            cameraMoveTime += Time.deltaTime * cameraMoveSpeed;
            transform.position = Vector3.Lerp(this.transform.position, transitionTarget, cameraMoveTime);
        }

        if (cameraMoveTime >= cameraMoveTimeMax && !followsAfter)
        {
            currentMode = Mode.Stationary;
            transform.position = transitionTarget;
        }
        else if(cameraMoveTime >= cameraMoveTimeMax && followsAfter)
        {
            Debug.Log("Follow after");
            transform.position = transitionTarget;
            currentMode = Mode.Follow;
            followsAfter = false;
        }
    }

    public void SwitchMode(Mode mode)
    {
        switch (mode)
        {
            case Mode.Follow:
                currentMode = Mode.Follow;
                break;

            case Mode.Stationary:
                currentMode = Mode.Stationary;
                followAreaValue = 4;
                break;

            case Mode.Transition:
                currentMode = Mode.Transition;
                followAreaValue = 4;
                break;
        }
    }
}
