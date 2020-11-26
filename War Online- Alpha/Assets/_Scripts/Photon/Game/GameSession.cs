using System;
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

        [NonSerialized] public List<global::Photon.Realtime.Player> AllPlayers;

        [NonSerialized] public readonly Dictionary<int, int> PlayersTeamIndexByActorID = new Dictionary<int, int>(),
            PlayersKillsByActorID = new Dictionary<int, int>(),
            PlayersDeathsByActorID = new Dictionary<int, int>();

        [NonSerialized] public readonly Dictionary<int, float>
            PlayersScoresByActorID = new Dictionary<int, float>();

        protected int NextSpawnTurn;

        protected int NextSpawnPointIndex, NextTeamIndex, NextFfaColorIndex;

        protected bool AllSpawned, MatchStarted;

        protected TankAddOn LocalTank;

        protected float MatchEndTime;

        public virtual void StartSession()
        {
            DBG.BeginMethod("StartSession");
            map.gettingReady.SetActive(true);
  
            AllPlayers = new List<global::Photon.Realtime.Player>(PhotonNetwork.PlayerList);

            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
            {//Only master client may instantiate players
                Debug.Log("!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected");
                return;
            }
            //Debug.Log("PhotonNetwork.IsConnected");
            Debug.Assert(PhotonNetwork.IsConnected != false);
            Debug.Assert(AllPlayers.Count > 0, "");

            NextSpawnTurn = AllPlayers[0].ActorNumber;

            photonView.RPC(nameof(SpawnPlayerRPC), RpcTarget.All, NextSpawnTurn, NextSpawnPointIndex, NextTeamIndex,
                NextFfaColorIndex);
            DBG.EndMethod("StartSession");
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
            DBG.BeginMethod("SpawnPlayerRPC");
            SpawnPlayer(playerActor, pointIndex, teamIndex, ffaColorIndex);
            DBG.EndMethod("SpawnPlayerRPC");
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

        [PunRPC]
        public void OnGameEndRPC()
        {
            OnGameEnd();
        }

        public virtual void SpawnPlayer(int playerActor, int pointIndex, int teamIndex, int ffaColorIndex)
        {
            DBG.BeginMethod("OnPlayerSpawn");
            //if (PhotonNetwork.LocalPlayer.ActorNumber != playerActor)
            //{
            //    DBG.EndMethod("OnPlayerSpawn");
            //    return;
            //}
            DBG.Log("T0");

            var p = GlobalValues.Session == GameSessionType.Ffa
                ? map.ffaSpawnPoints[pointIndex]
                : map.teamSpawnPoints[teamIndex].points[pointIndex];

            DBG.Log("T.5");


            Debug.LogWarning("Dominator TankHealth yields warnings when instantiated, possibly in OnEnabled");

            GameObject Tank = PhotonNetwork.Instantiate(GlobalValues.PlayerPrefab, p.position, p.rotation);
            LocalTank = Tank.GetComponent<TankAddOn>();
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
            DBG.Log("T1");
            photonView.RPC(nameof(OnPlayerSpawnRPC), RpcTarget.All, playerActor,
                LocalTank.GetComponent<FactionID>().teamIndex, ffaColorIndex);
            DBG.Log("T4");

            DBG.EndMethod("OnPlayerSpawn");

        }

        public virtual void OnPlayerSpawn(int playerActorID, int teamIndex, int ffaColorIndex)
        {
            DBG.BeginMethod("OnPlayerSpawn");
            PlayersTeamIndexByActorID.Add(playerActorID, teamIndex);

            if (!PhotonNetwork.IsMasterClient)
            {
                DBG.EndMethod("OnPlayerSpawn");
                return;
            }


            var i = AllPlayers.FindIndex((player => player.ActorNumber == playerActorID));
            if (i == AllPlayers.Count - 1)
            {
                AllSpawned = true;
                float wait_time = (PUN2Connection.Instance.OfflineMode) ? 0 : 10;
                photonView.RPC(nameof(StartGameRPC), RpcTarget.All, Time.time + wait_time);
                DBG.EndMethod("OnPlayerSpawn");
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

            DBG.EndMethod("OnPlayerSpawn");
        }

        public virtual void OnPlayerDie(int dyingPlayerID, int killerID)
        {
            if (!PlayersKillsByActorID.ContainsKey(killerID)) PlayersKillsByActorID[killerID] = 0;
            PlayersKillsByActorID[killerID] += 1;

            if (!PlayersDeathsByActorID.ContainsKey(dyingPlayerID)) PlayersDeathsByActorID[dyingPlayerID] = 0;
            PlayersDeathsByActorID[dyingPlayerID] += 1;
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

            StartCoroutine(nameof(MatchRunning));
        }

        protected virtual IEnumerator MatchRunning()
        {
            map.matchRunning.SetActive(true);

            float counter = MatchEndTime;

            while (counter > 0)
            {
                yield return new WaitForEndOfFrame();
                counter -= Time.deltaTime;
                var mins = (int) (counter / 60);
                var secs = (int) (counter % 60);
                map.matchTimeCounter.text = mins + "m " + secs + "s";
            }

            if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
            {
                photonView.RPC(nameof(OnGameEndRPC), RpcTarget.All);
            }
        }

        protected virtual void OnGameEnd()
        {
            LocalTank.enabled = false;
            map.leaderboard.gameObject.SetActive(true);
        }
    }
}