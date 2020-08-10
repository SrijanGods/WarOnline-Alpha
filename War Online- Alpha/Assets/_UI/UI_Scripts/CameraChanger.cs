using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    [SerializeField] Camera secondaryCamera;
    [SerializeField] Camera mainCam;

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = mainCam;
        secondaryCamera.gameObject.SetActive(false);
    }

    public void ChangeCameraToSec()
    {
        mainCam.gameObject.SetActive(false);
        GetComponent<Canvas>().worldCamera = secondaryCamera;
        secondaryCamera.gameObject.SetActive(true);
    }
     public void ChangeCameraToMain()
    {
        mainCam.gameObject.SetActive(true);
        GetComponent<Canvas>().worldCamera = Camera.main;
        secondaryCamera.gameObject.SetActive(false);
    }



}
