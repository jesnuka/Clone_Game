using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCutsceneManager : MonoBehaviour
{
    public GameObject menuUI;
    public GameObject introUI;
    public Camera menuCamera;

    public GameObject introText1;
    public GameObject introText2;
    public GameObject introText3;
    public GameObject introPlayerLight;

    public float cameraMoveSpeed;
    public float cameraMoveTime;

    Transform[] viewList;
    Transform currentView;

    public Vector3 cameraPosIntro;
    public Vector3 cameraPosMenu;
    public bool cameraCanMove;

    bool introSkipped;

    private void Awake()
    {
        if(menuCamera == null)
        {
            menuCamera = Camera.main;
        }
    }
    private void Update()
    {
        if ((Input.GetKey("space")) || (Input.GetKey(KeyCode.Return)) || (Input.GetKey(KeyCode.X)))
        {
            introSkipped = true;
        }

        if(introSkipped)
        {
            cameraCanMove = false;
            introPlayerLight.SetActive(true);
            menuUI.SetActive(true);
            introUI.SetActive(false);
            menuCamera.transform.position = cameraPosMenu;
        }
    }
    private void LateUpdate()
    {
        //TODO: Add cutscene skip button.
        if(cameraCanMove)
        {
            cameraMoveTime += Time.deltaTime * cameraMoveSpeed;
            menuCamera.transform.position = Vector3.Lerp(cameraPosIntro, cameraPosMenu, cameraMoveTime);
        }

        if(cameraMoveTime >= 1)
        {
            cameraCanMove = false;
            menuCamera.transform.position = cameraPosMenu;
        }
    }
    private void Start()
    {
        Debug.Log("Start");
        StartCoroutine(IntroCutscene1(0));
    }

    IEnumerator IntroCutscene1(int time)
    {
        yield return new WaitForSeconds(time);
        if(!introSkipped)
        {
            introUI.SetActive(true);
            introText1.SetActive(true);
            Debug.Log("1");
            StartCoroutine(IntroCutscene2(9));
        }
        
    }
    IEnumerator IntroCutscene2(int time)
    {
        yield return new WaitForSeconds(time);
        if(!introSkipped)
        {
            introText1.SetActive(false);
            introText2.SetActive(true);
            Debug.Log("2");
            StartCoroutine(IntroCutscene3(9));
        }
        
    }IEnumerator IntroCutscene3(int time)
    {
        yield return new WaitForSeconds(time);
        if (!introSkipped)
        {
            introText2.SetActive(false);
            introText3.SetActive(true);
            Debug.Log("3");
            StartCoroutine(IntroCutscene4(9));
        }
        
    }
    IEnumerator IntroCutscene4(int time)
    {
        yield return new WaitForSeconds(time);
        if (!introSkipped)
        {
            introText3.SetActive(false);
            introUI.SetActive(false);
            cameraCanMove = true;
            Debug.Log("4");
            StartCoroutine(IntroCutscene5(14));
        }
       
    }
    IEnumerator IntroCutscene5(int time)
    {
        yield return new WaitForSeconds(time);
        if (!introSkipped)
        {
            menuUI.SetActive(true);
            Debug.Log("5");
            introPlayerLight.SetActive(true);
        }
        
    }
}
