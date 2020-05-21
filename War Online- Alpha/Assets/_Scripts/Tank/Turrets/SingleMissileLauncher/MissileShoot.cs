using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileShoot : MonoBehaviour {

    public Rigidbody rigidbody;

    public Transform Transform;

    

    public void MissileShootFunction(float Force)
    {

        Debug.Log("MissileShootFunction" + Force);

        rigidbody = GetComponent<Rigidbody>();

        rigidbody.AddForce(Transform.forward * Force);

    }
}
