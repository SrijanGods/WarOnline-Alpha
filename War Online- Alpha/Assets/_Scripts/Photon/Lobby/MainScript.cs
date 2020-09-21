using UnityEngine;
using System.Collections;
using _Scripts.Photon.Game;
using Photon.Pun;
using Photon.Realtime;

public class MainScript : MonoBehaviourPunCallbacks
{
    [Header("Create Room Panel")] public GameObject createRoomPanel;

    [Header("Lobby Panel")] public GameObject lobbyPanel;


    //private vars
    private GateKeeper _playfabLogin;

    //private vars for lobby
    private int _xp;
    private TypedLobby _sqlLobby;

    #region InitialCalls

    public void Start()
    {
        _playfabLogin = GameObject.FindGameObjectWithTag("GameController").GetComponent<GateKeeper>();
        StartCoroutine(InitialiseConnection());
    }

    IEnumerator InitialiseConnection()
    {
        yield return new WaitUntil(() => _playfabLogin.PlayfabConnected);

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();

        PhotonNetwork.SendRate = 40;
        PhotonNetwork.SerializationRate = 14;
    }

    #endregion

    #region CallFromPlayers

    public void JoinLobby()
    {
        _xp = _playfabLogin.GetComponent<RankManager>().xpInInt;

        string lobbyName = "Advanced";

        if (_xp <= 3)
        {
            lobbyName = "Noobs";
        }
        else if (_xp <= 8)
        {
            lobbyName = "Players";
        }
        else if (_xp <= 11)
        {
            lobbyName = "Pro";
        }

        _sqlLobby = new TypedLobby(lobbyName, LobbyType.SqlLobby);
        PhotonNetwork.JoinLobby(_sqlLobby);
    }

    private string _battleMode;
    private int _players;

    public void JoinRandomBattle(string battleMode)
    {
        _players = 12;
        _battleMode = battleMode;
        string conds = $"BattleMode={_battleMode}";
        var expectedRoomProperties =
            new ExitGames.Client.Photon.Hashtable {{"C0", _battleMode}, {"C1", "Hmm"}};
        PhotonNetwork.JoinRandomRoom(expectedRoomProperties, (byte) _players, MatchmakingMode.FillRoom, _sqlLobby, conds);
    }

    #endregion

    #region PhotonCallBacks

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
        PhotonNetwork.CreateRoom("", GetRoomOptions(), _sqlLobby);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Disconnected", cause);
    }

    #endregion

    #region RoomProps

    RoomOptions GetRoomOptions()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            CustomRoomProperties =
                new ExitGames.Client.Photon.Hashtable()
                {
                    {"C0", _battleMode}, {"C1", "Hmm"}, {"C2", (float) PhotonNetwork.Time}
                },
            CustomRoomPropertiesForLobby = new string[] {"C0", "C1", "c2"},
            MaxPlayers = (byte) _players,
            EmptyRoomTtl = 120,
            CleanupCacheOnLeave = false,
            DeleteNullProperties = true
        };

        return roomOptions;
    }

    #endregion RoomProps
}