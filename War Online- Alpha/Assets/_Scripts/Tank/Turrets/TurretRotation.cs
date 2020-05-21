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

    void Start()
    {
        minAngle += 360;
        minXAngle += 360;

        if (transform.name == "Sniper")
        {
            dummyScope = gameObject.GetComponent<snipershooting>().scope;
        }
    }

    void Update()
    {
        if(Input.GetAxis("Mouse X") != 0f)
        {
            yaw += rotateSpeed * Input.GetAxis("Mouse X");
        }

        if (Input.GetAxis("Rotate") != 0f)
        {
            yaw += rotateSpeed * Input.GetAxis("Rotate");
        }

        yaw = Mathf.Clamp(yaw, -rotateSpeed, rotateSpeed);

        transform.Rotate(Vector3.up, yaw);

        if (transform.localEulerAngles.y > maxAngle && transform.localEulerAngles.y < minAngle)
        {
            if(yaw < 0f)
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
        /*
        if (Input.GetButtonDown("escape"))
        {
            DoUnlockMouse();
        }
        if (Input.GetButtonUp("escape"))
        {
            DoLockMouse();
        }*/
    }

    void DoLockMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) stream.SendNext(yaw);
        else this.yaw = (float)stream.ReceiveNext();
    }

    void DoUnlockMouse ()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SniperCamRotation(bool running)
    {
        if (running)
        {

            if (Input.GetAxis("Mouse Y") != 0f)
            {
               // xaw += sniperrotateSpeed * Input.GetAxis("Mouse Y");
            }

            if (Input.GetAxis("Vertical") != 0f)
            {
                xaw += sniperrotateSpeed * -Input.GetAxis("Vertical");
            }

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


