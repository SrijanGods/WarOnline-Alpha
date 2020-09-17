using Photon.Pun;

namespace _Scripts.Photon.Room
{
    public class FactionID : MonoBehaviourPun
    {
        public int teamID = -1;
        public string myAccID;

        private void OnEnable()
        {
            myAccID = PhotonNetwork.AuthValues.UserId;
        }
    }
}