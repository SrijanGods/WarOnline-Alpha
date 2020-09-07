using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FactionID : MonoBehaviour
{
        public int _teamID = 1;
        public int teamID = 1;
        public int myAccID;

    private void Start()
    {
        myAccID = gameObject.GetComponent<PhotonView>().ViewID;
    }
}

