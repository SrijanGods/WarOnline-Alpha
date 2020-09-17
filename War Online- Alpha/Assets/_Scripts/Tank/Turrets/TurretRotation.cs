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
    private float sniperrotateSpeed = 0.3f;
    private Transform dummyScope;
    private float xaw;

    private float maxXAngle = 1.179f;
    private float minXAngle = -3f;

    // private TouchProcessor tP;

    public float recenterSpeed;

    void Start()
    {
        // tP = GetComponentInParent<TouchProcessor>();
        minAngle += 360;
        minXAngle += 360;

        if (transform.name == "Sniper")
        {
            dummyScope = gameObject.GetComponent<Sniper>().scope;
        }
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

        transform.Rotate(Vector3.up, yaw);

        if (transform.localEulerAngles.y > maxAngle || transform.localEulerAngles.y < minAngle)
        {
            if (transform.localEulerAngles.y < minAngle)
            {
                Vector3 tempVec = new Vector3(0f, minAngle, 0f);
                transform.localEulerAngles = tempVec;
            }
            else
            {
                Vector3 tempVec = new Vector3(0f, maxAngle, 0f);
                transform.localEulerAngles = tempVec;
            }
        }

        yaw = 0f;

        if (!SimulatedInput.GetButton(InputCodes.Recenter)) return;

        if (transform.localEulerAngles.magnitude < 10) transform.localRotation = Quaternion.identity;
        else transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, recenterSpeed);
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) stream.SendNext(transform.rotation);
        else transform.rotation = (Quaternion) stream.ReceiveNext();
    }

    public void SniperCamRotation(bool running)
    {
        if (running)
        {
            var y = SimulatedInput.GetAxis(InputCodes.MouseLookY);
            if (Math.Abs(y) > .05f)
            {
                xaw += sniperrotateSpeed * y;
            }

            /*if (Input.GetAxis("Vertical") != 0f)
            {
                xaw += sniperrotateSpeed * -Input.GetAxis("Vertical");
            }*/

            xaw = Mathf.Clamp(xaw, -sniperrotateSpeed, sniperrotateSpeed);

            dummyScope.Rotate(Vector3.right, xaw);

            if (dummyScope.localEulerAngles.x > maxXAngle && dummyScope.localEulerAngles.x < minXAngle)
            {
                if (xaw < 0f)
                {
                    Vector3 tempVec2 = new Vector3(minXAngle, 0f, 0f);
                    dummyScope.localEulerAngles = tempVec2;
                }
                else
                {
                    Vector3 tempVec2 = new Vector3(maxXAngle, 0f, 0f);
                    dummyScope.localEulerAngles = tempVec2;
                }
            }

            xaw = 0f;
        }
    }
}