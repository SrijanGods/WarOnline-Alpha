using Photon.Pun;
using UnityEngine;

namespace _Scripts.Photon.Room
{
    public class FactionID : MonoBehaviourPun
    {
        public int teamIndex = -1;
        public string myAccID;
        public Color myColor;

        private void OnEnable()
        {
            if (photonView.IsMine)
            {
                myAccID = PhotonNetwork.AuthValues.UserId;
                photonView.RPC(nameof(SyncID), RpcTarget.Others, teamIndex, myAccID, 0, 0, 0);
            }
        }

        public void SetTeam(int t)
        {
            var c = GlobalValues.TeamColors[t];
            photonView.RPC(nameof(SyncID), RpcTarget.All, t, myAccID, c.r, c.g, c.b);
        }

        public void SetFFA(int t, Color c)
        {
            photonView.RPC(nameof(SyncID), RpcTarget.All, t, myAccID, c.r, c.g, c.b);
        }

        [PunRPC]
        public void SyncID(int t, string a, float r, float g, float b)
        {
            teamIndex = t;
            myAccID = a;
            myColor = new Color(r, g, b);
        }
    }
}