using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    private Camera[] cams;
    private Transform target;

    public void Start()
    {
        UpdateCamList();
    }

    public void UpdateCamList()
    {
        cams = FindObjectsOfType<Camera>();

        foreach(Camera cam in cams)
        {
            if(cam.gameObject == true)
            {
                target = cam.transform;
            }
        }
    }

    public void LateUpdate()
    {
        transform.LookAt(transform.position + target.forward);
    }
}
