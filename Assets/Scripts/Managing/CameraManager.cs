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


    //Place sets of transitions here, when moving to Stationary screens 
    [SerializeField]
    Transform[] transitionBegin;
    //These "End" transitions also work as the screens that the camera will stay Stationary in, unless stated otherwise
    [SerializeField]
    Transform[] transitionEnd;

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

    public void TransitionCamera(int i)
    {
        if (currentMode == Mode.Transition)
        {
            cameraMoveTime += Time.deltaTime * cameraMoveSpeed;
            transform.position = Vector3.Lerp(transitionBegin[i].position, transitionEnd[i].position, cameraMoveTime);
        }

        if (cameraMoveTime >= 1)
        {
            currentMode = Mode.Stationary;
            transform.position = transitionEnd[i].position;
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
