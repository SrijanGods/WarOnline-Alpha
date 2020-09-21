using System.Collections;
using System.Collections.Generic;
using _Scripts.Photon.Room;
using _Scripts.Tank;
using Photon.Pun;
using UnityEngine;

namespace _Scripts.Photon.Game
{
    public abstract class GameSession : MonoBehaviourPunCallbacks
    {
        public GameMap map;

        protected List<global::Photon.Realtime.Player> AllPlayers;
        protected Dictionary<int, int> playersTeamIndexByActorID = new Dictionary<int, int>();

        protected int NextSpawnTurn;

        protected int NextSpawnPointIndex, NextTeamIndex, NextFfaColorIndex;

        protected bool AllSpawned, MatchStarted;

        protected TankAddOn LocalTank;

        public virtual void StartSession()
        {
            map.gettingReady.SetActive(true);

            AllPlayers = new List<global::Photon.Realtime.Player>(PhotonNetwork.PlayerList);

            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) return;

            NextSpawnTurn = AllPlayers[0].ActorNumber;

            photonView.RPC(nameof(SpawnPlayerRPC), RpcTarget.All, NextSpawnTurn, NextSpawnPointIndex, NextTeamIndex,
                NextFfaColorIndex);
        }

        public override void OnPlayerEnteredRoom(global::Photon.Realtime.Player newPlayer)
        {
            AllPlayers.Add(newPlayer);
        }

        public override void OnPlayerLeftRoom(global::Photon.Realtime.Player otherPlayer)
        {
            AllPlayers.Remove(otherPlayer);
        }

        [PunRPC]
        public void SpawnPlayerRPC(int playerActor, int pointIndex, int teamIndex, int ffaColorIndex)
        {
            SpawnPlayer(playerActor, pointIndex, teamIndex, ffaColorIndex);
        }

        [PunRPC]
        public void OnPlayerSpawnRPC(int playerActor, int teamIndex, int ffaColorIndex)
        {
            OnPlayerSpawn(playerActor, teamIndex, ffaColorIndex);
        }


        [PunRPC]
        public void OnPlayerDieRPC(int dyingPlayerID, int killerID)
        {
            OnPlayerDie(dyingPlayerID, killerID);
        }

        [PunRPC]
        public void StartGameRPC(float t)
        {
            StartGame(t);
        }

        public virtual void SpawnPlayer(int playerActor, int pointIndex, int teamIndex, int ffaColorIndex)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != playerActor) return;

            var p = GlobalValues.Session == GameSessionType.Ffa
                ? map.ffaSpawnPoints[pointIndex]
                : map.teamSpawnPoints[teamIndex].points[pointIndex];

            LocalTank = PhotonNetwork.Instantiate(GlobalValues.PlayerPrefab, p.position, p.rotation)
                .GetComponent<TankAddOn>();
            LocalTank.enabled = false;

            switch (GlobalValues.Session)
            {
                case GameSessionType.Ffa:
                    LocalTank.GetComponent<FactionID>().SetFFA(playerActor, GlobalValues.FfaColors[ffaColorIndex]);
                    NextSpawnPointIndex = (pointIndex + 1) % map.ffaSpawnPoints.Length;
                    break;
                case GameSessionType.Teams:
                    LocalTank.GetComponent<FactionID>().SetTeam(teamIndex);
                    NextTeamIndex = (teamIndex + 1) % GlobalValues.TeamColors.Length;
                    NextSpawnPointIndex = (pointIndex + 1) % map.teamSpawnPoints[NextTeamIndex].points.Length;
                    break;
            }

            photonView.RPC(nameof(OnPlayerSpawnRPC), RpcTarget.All, playerActor,
                LocalTank.GetComponent<FactionID>().teamIndex, ffaColorIndex);
        }

        public virtual void OnPlayerSpawn(int playerActorID, int teamIndex, int ffaColorIndex)
        {
            playersTeamIndexByActorID.Add(playerActorID, teamIndex);

            if (!PhotonNetwork.IsMasterClient) return;

            var i = AllPlayers.FindIndex((player => player.ActorNumber == playerActorID));
            if (i == AllPlayers.Count - 1)
            {
                AllSpawned = true;
                photonView.RPC(nameof(StartGameRPC), RpcTarget.All, Time.time + 10);
                return;
            }

            NextSpawnTurn = AllPlayers[i + 1].ActorNumber;

            switch (GlobalValues.Session)
            {
                case GameSessionType.Ffa:
                    NextSpawnPointIndex = (NextSpawnPointIndex + 1) % map.ffaSpawnPoints.Length;
                    break;
                case GameSessionType.Teams:
                    NextTeamIndex = (NextTeamIndex + 1) % GlobalValues.TeamColors.Length;
                    NextSpawnPointIndex = (NextSpawnPointIndex + 1) % map.teamSpawnPoints[NextTeamIndex].points.Length;
                    break;
            }

            NextFfaColorIndex = (ffaColorIndex + 1) % GlobalValues.FfaColors.Length;
            photonView.RPC(nameof(SpawnPlayerRPC), RpcTarget.Others, NextSpawnTurn, NextSpawnPointIndex, NextTeamIndex,
                NextFfaColorIndex);
        }

        public virtual void OnPlayerDie(int dyingPlayerID, int killerID)
        {
        }

        public virtual void StartGame(float t)
        {
            map.gettingReady.SetActive(false);
            map.matchStarting.SetActive(true);

            StartCoroutine(MatchStarting(t));
        }

        protected virtual IEnumerator MatchStarting(float startTime)
        {
            while (Time.time < startTime)
            {
                map.matchStartCounter.text = ((int) (startTime - Time.time)).ToString();
                yield return new WaitForSeconds(.25f);
            }

            map.matchStarting.SetActive(false);
            MatchStarted = true;
            LocalTank.enabled = true;
        }
    }
}