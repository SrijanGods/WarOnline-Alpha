using Photon.Pun;
using UnityEngine;

namespace _Scripts.Photon.Room
{
    public class FactionID : MonoBehaviour
    {
        public int teamID = 1;
        public int myAccID;

        private void Start()
        {
            myAccID = gameObject.GetComponent<PhotonView>().ViewID;
        }
    }
}