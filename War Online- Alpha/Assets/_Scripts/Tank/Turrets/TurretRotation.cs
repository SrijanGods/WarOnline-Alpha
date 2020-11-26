using System;
using _Scripts.Controls;
using _Scripts.Tank.Turrets.Sniper;
using UnityEngine;
using Photon.Pun;

public class TurretRotation : MonoBehaviourPun, IPunObservable
{
    [SerializeField, Tooltip("Rotation speed")]
    public float rotateSpeed = 8.0f;

    private float yaw = 0f;

    [SerializeField, Tooltip("Left angle")]
    private float minAngle = 0f;

    [SerializeField, Tooltip("Right angle")]
    private float maxAngle = 60f;

    //exclusively for sniper
    private float sniperrotateSpeed = 6f;
    private Transform dummyScope;
    private float xaw;

    private float maxXAngle = 1.179f;
    private float minXAngle = -3f;

    // private TouchProcessor tP;

    public float recenterSpeed;
    Vector3 InitialRotation;
    void Start()
    {
        // tP = GetComponentInParent<TouchProcessor>();
        minAngle += 360;
        minXAngle += 360;

        if (transform.name == "Sniper")
        {
            dummyScope = gameObject.GetComponent<Sniper>().scope;
            //dummyScope.parent = dummyScope.parent.parent;
        }

        InitialRotation = transform.forward;
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        var lookX = SimulatedInput.GetAxis(InputCodes.MouseLookX);
        var lookY = SimulatedInput.GetAxis(InputCodes.MouseLookY);

        if (Math.Abs(lookX) > .05f)
        {
            yaw += rotateSpeed * lookX;
        }

        /*if (Math.Abs(lookY) > .05f)
        {
            yaw += rotateSpeed * lookY;
        }*/

        yaw = Mathf.Clamp(yaw, -rotateSpeed, rotateSpeed);

        transform.Rotate(xaw, yaw, 0f, Space.World);
        //DBG.Log("Turret Rotation : " + yaw);
        //if (transform.localEulerAngles.y > maxAngle || transform.localEulerAngles.y < minAngle)
        //{
        //    if (transform.localEulerAngles.y <    minAngle)
        //    {
        //        Vector3 tempVec = new Vector3(0f, minAngle, 0f);
        //        transform.localEulerAngles = tempVec;
        //    }
        //    else
        //    {
        //        Vector3 tempVec = new Vector3(0f, maxAngle, 0f);
        //        transform.localEulerAngles = tempVec;
        //    }
        //}

        yaw = 0f;

        if (!SimulatedInput.GetButton(InputCodes.Recenter)) return;
 
        if (transform.localEulerAngles.magnitude < 10) 
            transform.localRotation = Quaternion.identity;
        else 
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, recenterSpeed);

 
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) stream.SendNext(transform.rotation);
        else transform.rotation = (Quaternion) stream.ReceiveNext();
    }

    /// <summary>
    /// This method magically transforms the x rotation into a continuous numberline, thater than: 359, 360, 0, 1, 2, 3, 4, 5
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static float ConvertToAngle180(float input)
    {
        while (input > 360)
        {
            input = input - 360;
        }
        while (input < -360)
        {
            input = input + 360;
        }
        if (input > 180)
        {
            input = input - 360;
        }
        if (input < -180)
            input = 360 + input;
        return input;
    }
    public void SniperCamVerticalRotation()
    {
        //DBG.BeginMethod("SniperCamVerticalRotation");

        var lookY = SimulatedInput.GetAxis(InputCodes.MouseLookY);
        if (Math.Abs(lookY) > .05f)
            xaw += sniperrotateSpeed * lookY;

        xaw = Mathf.Clamp(xaw, -sniperrotateSpeed, sniperrotateSpeed);


        //Debug.Log("ConvertToAngle180(transform.eulerAngles.x):  " + ConvertToAngle180(transform.eulerAngles.x));

        //xaw < 0 means turret rotates up
        if (xaw < 0 && ConvertToAngle180(transform.eulerAngles.x) > -20)//Prevents rotating up to -20 degrees in X axis
            transform.Rotate(xaw, 0f, 0f, Space.Self);

        //xaw > 0 means turret rotates up
        if (xaw > 0 && ConvertToAngle180(transform.eulerAngles.x) < 10)//Prevents rotating down below 10 degrees in X axis
            transform.Rotate(xaw, 0f, 0f, Space.Self);

    
        xaw = 0f;
        //DBG.EndMethod("SniperCamVerticalRotation");

    }
}