using UnityEngine;
using System.Collections;
using Photon.Pun;

public class GarageCamRotate : MonoBehaviourPun
{
    public int sped1;
    public int sped2;
    
    private void FixedUpdate()
    {
        transform.Rotate(sped2 * Time.deltaTime, sped1 * Time.deltaTime, 0, Space.World);
    }
}