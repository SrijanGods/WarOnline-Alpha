using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
//using DPhysics;
using System;
using UnityEngine.UI;
using _Scripts.Photon.Room;

namespace _Scripts
{
    /// <summary>
    /// Manages PUN2 connection via verbose tokens. Some tokens are reset at times, to prevent erratic behaviour. Dont just throw random coins at the Machine!
    /// </summary>
    public class PUN2Connection : MonoBehaviourPunCallbacks

    {
        private static PUN2Connection _Instance;
        /// <summary>
        /// Singleton Pattern, access Instance from any script by using PUN2Connection.instance
        /// </summary>
        public static PUN2Connection Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = GameObject.FindObjectOfType<PUN2Connection>();
                    //Debug.LogWarning("Missing Board, Instantiate a Board");
                }
                return _Instance;
            }
        }


        #region PUN2_CONNECTION
        /// <summary>
        /// This property wraps PhotonNetwork.OfflineMode.
        /// </summary>
        public bool OfflineMode;

        [Header("Manual Control of the Network Connection, via tokens")]

        public bool Connect_To_Master_Token = false;
        public bool Disconnected_From_Master_Token = false;// This token is only adquired when the Pun2 Dissconects from master on OnDisconnected(DisconnectCause cause)
        public bool Disconnect_From_Master_Token = false;
        public bool Create_Room_Token = false;
        public bool Create_Room_Failed_Token = false;
        // public bool Close_Room_Token = false; If everyone Leaves Room, it closes. No need for Explicit closing Or opening of rooms.
        public bool Leave_Room_Token = false;
        [Header("Automatic Control of the Network Connection")]
        public bool Go_Online = false;

        //public override void OnEnable()
        //{

        //    base.OnEnable();
        //}

        public void Update()
        {
                Network_Controller();
        }

        public enum States { START, OFFLINE, CONNECTING, DISCONNECTING, ONLINE, CONNECTING_TO_ROOM, LEAVING_ROOM, CONNECTING_TO_RANDOM_ROOM, CREATING_ROOM, ONLINE_IN_ROOM, ONLINE_IN_GAME, OFFLINE_IN_GAME }

        public States state = States.START;
        public void Network_Controller()
        {
            DBG.BeginFSM("Network_Controller");
            switch (state)
            {
                case States.START:
                    {
                        PhotonNetwork.OfflineMode = OfflineMode;
                        Debug.Log(" PhotonNetwork.OfflineMode: " + PhotonNetwork.OfflineMode);
                    }
                    state = States.OFFLINE;
                    DBG.LogTransition("START -> OFFLINE");
                    break;
                case States.OFFLINE:
                    if (Connect_To_Master_Token.ConsumeToken() || Go_Online)
                    {
                        // Debug.Assert(PhotonNetwork.PlayerList != null);
                        //DBG.Log("PhotonNetwork.PlayerList.Count: " + PhotonNetwork.PlayerList.Length);
                        if(PhotonNetwork.OfflineMode == false)
                            DBG.Log("ConnectUsingSettings: " + PhotonNetwork.ConnectUsingSettings());
                        //DBG.Log("PhotonNetwork.PlayerList.Count: " + PhotonNetwork.PlayerList.Length);

                        state = States.CONNECTING;
                        DBG.LogTransition("OFFLINE -> CONNECTING");
                    }
                    //else if (Go_Online)
                    //{
                    //    PhotonNetwork.ConnectUsingSettings();
                    //    if (PhotonNetwork.CountOfRooms == 0)
                    //        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 20 });
                    //    else
                    //        PhotonNetwork.JoinRandomRoom();

                    //    state = States.CONNECTING;
                    //    DBG.LogTransition("OFFLINE -> CONNECTING");

                    //}
                    break;
                case States.CONNECTING:
                    if (PhotonNetwork.IsConnectedAndReady && Connected_To_Master)
                    {

                        if (PhotonNetwork.OfflineMode == false)
                        {
                            Debug.Log("Server:" + PhotonNetwork.ServerAddress.ToString());
                            Debug.Log("CloudRegion:" + PhotonNetwork.CloudRegion.ToString());
                        }
                        //if (Region != null)
                        //    Region.text = PhotonNetwork.CloudRegion.ToString();
                        //else
                        //    Debug.LogWarning("Region is null");

                        state = States.ONLINE;
                        DBG.LogTransition("CONNECTING -> ONLINE");
                    }
                    else if (Disconnected_From_Master_Token.ConsumeToken())
                    {
                        Go_Online = false;

                        state = States.OFFLINE;
                        DBG.LogTransition("CONNECTING -> OFFLINE");
                    }
                    break;
                case States.DISCONNECTING:
                    if (Disconnected_From_Master_Token.ConsumeToken())
                    {
                        Go_Online = false;

                        state = States.OFFLINE;
                        DBG.LogTransition("DISCONNECTING -> OFFLINE");
                    }
                    break;
                case States.ONLINE:
                    if (Disconnect_From_Master_Token.ConsumeToken())
                    {

                        PhotonNetwork.Disconnect();
                        state = States.DISCONNECTING;
                        DBG.LogTransition("ONLINE -> DISCONNECTING");
                    }
                    else if (Join_Random_Room || (Go_Online && PhotonNetwork.CountOfRooms > 0))
                    {
                        Go_Online = false;
                        Join_Random_Room = false;
                        PhotonNetwork.JoinRandomRoom();               //Join a random Room     - Error: OnJoinRandomRoomFailed  
                        state = States.CONNECTING_TO_RANDOM_ROOM;
                        Debug.Log("PhotonNetwork.CountOfRooms: " + PhotonNetwork.CountOfRooms);
                        DBG.LogTransition("ONLINE -> CONNECTING_TO_RANDOM_ROOM");
                    }
                    else if (Create_Room_Token.ConsumeToken() || (Go_Online && PhotonNetwork.CountOfRooms == 0))
                    {
                        Go_Online = false;
                        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 20 }, TypedLobby.Default);
                        state = States.CREATING_ROOM;
                        Debug.Log("PhotonNetwork.CountOfRooms: " + PhotonNetwork.CountOfRooms);
                        DBG.LogTransition("ONLINE -> CREATING_ROOM");
                    }
                    else if (!PhotonNetwork.IsConnected)
                    {
                        //Connection_Tab.GetComponentInChildren<Button>().onClick.Invoke();
                        state = States.OFFLINE;
                        DBG.LogTransition("LEAVIONLINENG_ROOM -> OFFLINE");
                    }
                    break;
                case States.CONNECTING_TO_RANDOM_ROOM:
                    if (Joined_Room)
                    {
                        Joined_Room = false;
                        Go_Online = false;

                        state = States.ONLINE_IN_ROOM;
                        DBG.LogTransition("CONNECTING_TO_RANDOM_ROOM -> ONLINE_IN_ROOM");
                    }
                    else if (Join_Random_Room_Fail)
                    {
                        Join_Random_Room_Fail = false;

                        state = States.ONLINE;
                        DBG.LogTransition("CONNECTING_TO_RANDOM_ROOM -> ONLINE");
                    }
                    break;
                case States.CREATING_ROOM:
                    if (Create_Room_Failed_Token)
                    {
                        state = States.ONLINE;
                        DBG.LogTransition("CREATING_ROOM -> ONLINE");
                    }
                    else if (Joined_Room)
                    {
                        Joined_Room = false;

                        Go_Online = false;
                        //if (DBG_Launch_Player2_Instance_Token.ConsumeToken())
                        //    RunPlayer2();
                        state = States.ONLINE_IN_ROOM;
                        DBG.LogTransition("CREATING_ROOM -> ONLINE_IN_ROOM");
                    }
                    break;
                case States.ONLINE_IN_ROOM:
                    if (Leave_Room_Token.ConsumeToken())
                    {
                        PhotonNetwork.LeaveRoom();
                        //PhotonNetwork.CurrentRoom.IsOpen = false;
                        state = States.LEAVING_ROOM;
                        DBG.LogTransition("ONLINE_IN_ROOM -> LEAVING_ROOM");
                    }
                    else if (!PhotonNetwork.IsConnected)
                    {
                        state = States.LEAVING_ROOM;
                        DBG.LogTransition("ONLINE_IN_ROOM -> LEAVING_ROOM");
                    }
                    break;
                case States.LEAVING_ROOM:
                    if (Left_Room)
                    {
                        Left_Room = false;

                        state = States.ONLINE;
                        DBG.LogTransition("LEAVING_ROOM -> ONLINE");
                    }
                    break;
                case States.ONLINE_IN_GAME:
                    if (Create_Room_Token.ConsumeToken())
                    {
                        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 20 });
                    }
                    else if (Join_Random_Room)
                    {
                        Join_Random_Room = false;
                        PhotonNetwork.JoinRandomRoom();               //Join a random Room     - Error: OnJoinRandomRoomFailed  
                    }
                    break;
                case States.OFFLINE_IN_GAME:
                    break;
                default:
                    DBG.LogTransition("INVALID STATE");
                    break;
            }
            DBG.EndFSM();
        }

        public bool Connected_To_Master = false;
        public override void OnConnectedToMaster()
        {
            DBG.BeginMethod("OnConnectedToMaster");
            base.OnConnectedToMaster();
            Connected_To_Master = true;
            DBG.EndMethod("OnConnectedToMaster");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            DBG.BeginMethod("OnDisconnected");
            Disconnected_From_Master_Token = true;
            Connected_To_Master = false;
            base.OnDisconnected(cause);
            Debug.Log(cause);
            DBG.EndMethod("OnDisconnected");
        }

        public bool Join_Random_Room = false;
        public bool Joined_Room = false;
        public override void OnJoinedRoom()
        {
            DBG.BeginMethod("OnJoinedRoom");
            Joined_Room = true;
            Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | RoomName: " + PhotonNetwork.CurrentRoom.Name);
            DBG.Log("PhotonNetwork.CurrentRoom.Players.Count: " + PhotonNetwork.CurrentRoom.Players.Count);
            Player PUN_player = PhotonNetwork.LocalPlayer;// null;
            base.OnJoinedRoom();
            //Debug.Assert(PhotonNetwork.InLobby);
            // DBG.Log("Lobby name an type: " + PhotonNetwork.CurrentLobby.Name + ", " + PhotonNetwork.CurrentLobby.Type.ToString());
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                DBG.Log(player.Value.ActorNumber + ",[local:" + player.Value.IsLocal + "]" + "[isMasterClient? " + player.Value.IsMasterClient + "]");

            }

            //This will start the session
            GameMap.Instance.StartSession_Token = true;




            DBG.EndMethod("OnJoinedRoom");
        }


        public bool Join_Random_Room_Fail = false;
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            DBG.BeginMethod("OnJoinRandomFailed");
            base.OnJoinRandomFailed(returnCode, message);
            Join_Random_Room_Fail = true;
            //no room available
            //create a room (null as a name means "does not matter")
            //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 20 });
            DBG.EndMethod("OnJoinRandomFailed");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            DBG.BeginMethod("OnCreateRoomFailed");
            base.OnCreateRoomFailed(returnCode, message);
            Debug.Log(message);
            Create_Room_Failed_Token = true;
            DBG.EndMethod("OnCreateRoomFailed");
        }

        public bool Left_Room = false;
        public override void OnLeftRoom()
        {
            DBG.BeginMethod("OnLeftRoom");
            base.OnLeftRoom();
            Left_Room = true;
            DBG.EndMethod("OnLeftRoom");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Color new_player_color;

        }




        #endregion
    }
}
