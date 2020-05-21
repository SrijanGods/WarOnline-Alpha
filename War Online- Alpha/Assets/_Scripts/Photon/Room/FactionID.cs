using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FactionID : MonoBehaviour
{
    public float _teamID = 1;
    public float myAccID;

    private void Start()
    {
        myAccID = gameObject.GetComponent<PhotonView>().ViewID;
    }

}
