using _Scripts.Tank.TankHealth;
using Photon.Pun;
using UnityEngine;

namespace _Scripts.Photon.Room
{
    public class FactionID : MonoBehaviourPun
    {
        public int teamIndex = -1, actorNumber;
        public string myAccID;
        public Color myColor;

        private void OnEnable()
        {
     
        }

        public void Start()
        {
            if (photonView.IsMine)
            {
                myAccID = PhotonNetwork.AuthValues.UserId;
                actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                photonView.RPC(nameof(SyncSelfID), RpcTarget.Others, actorNumber, myAccID,
                    PhotonNetwork.LocalPlayer.NickName);
            }
        }

        public void SetTeam(int t)
        {
            var c = GlobalValues.TeamColors[t];
            photonView.RPC(nameof(SyncTeamID), RpcTarget.All, t, c.r, c.g, c.b);
        }

        public void SetFFA(int t, Color c)
        {
            photonView.RPC(nameof(SyncTeamID), RpcTarget.All, t, c.r, c.g, c.b);
        }

        [PunRPC]
        public void SyncSelfID(int an, string acc, string nickname)
        {
            actorNumber = an;
            myAccID = acc;
            GetComponent<TankHealth>().otherShownName.text = nickname;
        }

        [PunRPC]
        public void SyncTeamID(int t, float r, float g, float b)
        {
            teamIndex = t;
            myColor = new Color(r, g, b);
            GetComponent<TankHealth>().otherShownName.color = myColor;
        }
    }
}