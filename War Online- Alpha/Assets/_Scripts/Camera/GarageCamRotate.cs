using UnityEngine;
using System.Collections;
using Photon.Pun;

public class GarageCamRotate : MonoBehaviourPun
{
    [SerializeField]
    private Transform target;

    public float rotateSpeed;

    [SerializeField]
    private float minDist;

    [SerializeField]
    private float distToChange;

    [SerializeField]
    private Vector3 offsetPosition;

    [SerializeField]
    private Space offsetPositionSpace = Space.Self;

    [SerializeField]
    private bool lookAt = true;

    

    private void Start()
    {

    }
    private void FixedUpdate()
    {
        target.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, Mathf.Infinity))
        {

        }

        if (hit.distance < minDist)
        {
            offsetPosition.z -= distToChange;
        }
        else if (hit.distance > minDist)
        {
            offsetPosition.z += distToChange;
        }
        else if (hit.distance == minDist)
        {
            return;
        }
    }
    private void LateUpdate()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (target == null)
        {
            Debug.LogWarning("Missing target ref !", this);

            return;
        }

        // compute position
        if (offsetPositionSpace == Space.Self)
        {
            transform.position = target.TransformPoint(offsetPosition);
        }
        else
        {
            transform.position = target.position + offsetPosition;
        }

        // compute rotation
        if (lookAt)
        {
            transform.LookAt(target);
        }
        else
        {
            transform.rotation = target.rotation;
        }
    }
}