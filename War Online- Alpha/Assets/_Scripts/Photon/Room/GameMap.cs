using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace _Scripts.Photon.Room
{
    [RequireComponent(typeof(PhotonView))]
    public class GameMap : MonoBehaviourPunCallbacks
    {
        public Transform[] spawnPoints;

        private string PlayerPrefab
        {
            get
            {
                var x = GlobalValues.hull;

                switch (GlobalValues.turret)
                {
                    case "Acidton":
                        x += "AT";
                        break;
                    case "Blaster":
                        x += "BL";
                        break;
                    case "Duos":
                        x += "DS";
                        break;
                    case "FlameThrower":
                        x += "FT";
                        break;
                    case "MachineGun":
                        x += "MG";
                        break;
                    case "MissileLauncher":
                        x += "ML";
                        break;
                    case "Sniper":
                        x += "SP";
                        break;
                    case "WindChill":
                        x += "WC";
                        break;
                    default:
                        x += "DS";
                        break;
                }

                return x;
            }
        }

        private List<global::Photon.Realtime.Player> _otherPlayers;

        private int _nextSpawnTurn, _nextSpawnPoint;

        private bool _allSpawned;

        private void Start()
        {
            _otherPlayers = new List<global::Photon.Realtime.Player>(PhotonNetwork.PlayerListOthers);

            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) return;

            _nextSpawnTurn = PhotonNetwork.LocalPlayer.ActorNumber;

            var p = spawnPoints[_nextSpawnPoint];

            PhotonNetwork.Instantiate(PlayerPrefab, p.position, Quaternion.identity);

            _nextSpawnTurn = _otherPlayers[0].ActorNumber;
            _nextSpawnPoint = (_nextSpawnPoint + 1) % spawnPoints.Length;

            photonView.RPC(nameof(SpawnNonMasterPlayer), RpcTarget.Others, _nextSpawnTurn, _nextSpawnPoint);
        }

        public override void OnPlayerEnteredRoom(global::Photon.Realtime.Player newPlayer)
        {
            _otherPlayers.Add(newPlayer);

            if (!_allSpawned) return;

            if (!PhotonNetwork.IsMasterClient) return;

            _nextSpawnTurn = newPlayer.ActorNumber;
            _nextSpawnPoint = (_nextSpawnPoint + 1) % spawnPoints.Length;

            photonView.RPC(nameof(SpawnNonMasterPlayer), RpcTarget.Others, _nextSpawnTurn, _nextSpawnPoint);
        }

        public override void OnPlayerLeftRoom(global::Photon.Realtime.Player otherPlayer)
        {
            _otherPlayers.Remove(otherPlayer);
        }

        [PunRPC]
        public void SpawnNonMasterPlayer(int actorNumber, int pointIndex)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != actorNumber) return;

            var p = spawnPoints[pointIndex];

            PhotonNetwork.Instantiate(PlayerPrefab, p.position, Quaternion.identity);

            photonView.RPC(nameof(ClientPlayerSpawned), RpcTarget.MasterClient);
        }

        [PunRPC]
        public void ClientPlayerSpawned()
        {
            var i = _otherPlayers.FindIndex((player => player.ActorNumber == _nextSpawnTurn));
            if (i == _otherPlayers.Count - 1)
            {
                _allSpawned = true;
                return;
            }

            _nextSpawnTurn = _otherPlayers[i + 1].ActorNumber;
            _nextSpawnPoint = (_nextSpawnPoint + 1) % spawnPoints.Length;

            photonView.RPC(nameof(SpawnNonMasterPlayer), RpcTarget.Others, _nextSpawnTurn, _nextSpawnPoint);
        }
    }
}