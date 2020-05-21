using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncer : MonoBehaviour {

    public GameObject rocket;
    public Transform CreationPoint;
    public float RocketSpeed = 1f;
    public float constantForce = 10f;
    public Vector3 rocketSize;

    private void Update()
    {
        if (transform.rotation.z < 90f)
        {
            transform.Rotate(0, 0, 6 * Time.deltaTime);
        }
        if (transform.rotation.z > 90f)
        {
            transform.Rotate(0, 0, -6 * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            RocketSpeed += 1;
        }

        if (Input.GetButtonUp("Fire"))
        {
            Debug.Log(RocketSpeed);
            RocketThrow();
            RocketSpeed = 0;
        }

    }

    public void RocketThrow()
    {
        GameObject RL = Instantiate(rocket as GameObject);
        RL.transform.localScale = rocketSize;
        RL.transform.position = CreationPoint.position;
        // RL.GetComponent<Rigidbody>().AddForce(constantForce * RocketSpeed * Time.deltaTime, 0, 0);
       // RL.GetComponent<Rigidbody>().velocity = transform.forward * Time.deltaT;
    }
}
