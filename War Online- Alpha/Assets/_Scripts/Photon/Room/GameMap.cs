using System;
using System.Collections.Generic;
using _Scripts.Photon.Game;
using _Scripts.Photon.Game.Leaderboard;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Photon.Room
{
    [Serializable]
    public struct SpawnPoints
    {
        public Transform[] points;
    }

    [RequireComponent(typeof(PhotonView))]
    public class GameMap : MonoBehaviourPunCallbacks
    {
        public Transform[] ffaSpawnPoints;
        public List<SpawnPoints> teamSpawnPoints;
        public GameObject gettingReady, matchStarting, matchRunning;
        public Text matchStartCounter, matchTimeCounter;
        public Leaderboard leaderboard;
        [NonSerialized] public GameSession SessionObject;

        private void Start()
        {
            switch (GlobalValues.GameMode)
            {
                case GameMode.DeathMatch:
                    var i = gameObject.AddComponent<DeathMatch>();
                    SessionObject = i;
                    i.map = this;
                    i.StartSession();
                    break;
            }
        }
    }
}