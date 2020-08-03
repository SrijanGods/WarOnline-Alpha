using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    [SerializeField] Camera secondaryCamera;
    [SerializeField] GameObject backButton;

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        backButton.SetActive(false);
    }

    public void ChangeCameraAndBackButton()
    {
        GetComponent<Canvas>().worldCamera = secondaryCamera;
        backButton.SetActive(true);

    }


}
