using System;
using System.Collections.Generic;
using _Scripts.Photon.Game;
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
        public GameObject gettingReady, matchStarting;
        public Text matchStartCounter;
        private GameSession _sessionObject;

        private void Start()
        {
            switch (GlobalValues.GameMode)
            {
                case GameMode.DeathMatch:
                    var i = gameObject.AddComponent<DeathMatch>();
                    i.map = this;
                    i.StartSession();
                    break;
            }
        }
    }
}