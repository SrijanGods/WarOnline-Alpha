using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class MainScript : MonoBehaviourPunCallbacks
{
    [Header("Create Room Panel")]
    public GameObject createRoomPanel;

    [Header("Lobby Panel")]
    public GameObject lobbyPanel;


    //private vars
    private GateKeeper playfabLogin;

    //private vars for lobby
    private int xp;
    private TypedLobby sqlLobby;

    #region InitialCalls

    public void Start()
    {
        playfabLogin = GameObject.FindGameObjectWithTag("GameController").GetComponent<GateKeeper>();
        StartCoroutine(InitialiseConnection());
    }

    IEnumerator InitialiseConnection()
    {
        yield return new WaitUntil(() => playfabLogin.PlayfabConnected);

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.SendRate = 40;
        PhotonNetwork.SerializationRate = 14;
    }

    #endregion

    #region CallFromPlayers

    public void JoinLobby()
    {
        xp = playfabLogin.GetComponent<RankManager>().xpInInt;

        string lobbyName = "Null";

        if(xp <= 3)
        {
            lobbyName = "Noobs";
        }
        else if(xp > 3 && xp <= 8)
        {
            lobbyName = "Players";
        }
        else if (xp > 8 && xp <= 11)
        {
            lobbyName = "Pro";
        }

        sqlLobby = new TypedLobby(lobbyName, LobbyType.SqlLobby);
        PhotonNetwork.JoinLobby(sqlLobby);
    }

    private string battleMode;
    private int players;
    public void JoinRandomBattle(string BattleMode)
    {
        players = 12;
        battleMode = BattleMode;
        string conds = string.Format("BattleMode={0}", battleMode);
        PhotonNetwork.JoinRandomRoom(null, (byte)players, MatchmakingMode.FillRoom, sqlLobby, conds);
    }

    #endregion

    #region PhotonCallBacks

    //room joining shiiit
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int rand = Random.Range(2, 3);
            PhotonNetwork.LoadLevel(rand);
        }
    }

    public override void OnJoinedLobby()
    {
        lobbyPanel.SetActive(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom("", GetRoomOptions(), sqlLobby);
    }
    //shit over


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Disconnected", cause);
    }

    #endregion

    #region RoomProps

    RoomOptions GetRoomOptions()
    {
        RoomOptions roomOptions = new RoomOptions();

        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "C0", battleMode }, { "C1", "Hmm" }, { "C2", (float)PhotonNetwork.Time } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "C0", "C1", "c2" };

        roomOptions.MaxPlayers = (byte)players;
        roomOptions.EmptyRoomTtl = (byte)120;
        roomOptions.CleanupCacheOnLeave = false;
        roomOptions.DeleteNullProperties = true;

        return roomOptions;
    }
    #endregion RoomProps
}
