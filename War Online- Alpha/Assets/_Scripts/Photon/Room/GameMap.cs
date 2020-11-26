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
        private static GameMap _Instance;
        /// <summary>
        /// Singleton Pattern, access Instance from any script by using GameMap.instance
        /// </summary>
        public static GameMap Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = GameObject.FindObjectOfType<GameMap>();
                    //Debug.LogWarning("Missing Board, Instantiate a Board");
                }
                return _Instance;
            }
        }


        public Transform[] ffaSpawnPoints;
        public List<SpawnPoints> teamSpawnPoints;
        public GameObject gettingReady, matchStarting, matchRunning;
        public Text matchStartCounter, matchTimeCounter;
        public Leaderboard leaderboard;
        [NonSerialized] public GameSession SessionObject;

        void StartSession()
        {
            DBG.BeginMethod ("StartSession");
            switch (GlobalValues.GameMode)
            {
                case GameMode.DeathMatch:
                    DBG.Log("Session Created");
                    var i = gameObject.AddComponent<DeathMatch>();
                    SessionObject = i;
                    i.map = this;
                    i.StartSession();
                    break;
            }

            SessionHasStarted = true;// This is to prevent double execution if you set StartSession_Token, when in OnlineMode: Update()... [PUN2Connection.Instance.OfflineMode == false]
            DBG.EndMethod("StartSession");
        }

        public bool StartSession_Token;
        private bool SessionHasStarted = false;
        private void Start()
        {
            //If Running OfflineMode Start session right away because you dont need to connect to PUN2
            //if (PUN2Connection.Instance.OfflineMode == true)
            //{
            //    StartSession();
            //}
        }

        private void Update()
        {
            //If not running in Offline Mode. We wait until StartSession_Token is set. If a session is not started already
            //if (PUN2Connection.Instance.OfflineMode == false)
            {
                if (SessionHasStarted == false && StartSession_Token.ConsumeToken())
                {
                    StartSession();
                }
            }
        }

        



    }
}