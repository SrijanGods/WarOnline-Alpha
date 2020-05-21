using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MainScript : MonoBehaviourPunCallbacks
{
    [Header("Create Room Panel")]
    public GameObject createRoomPanel;
    [Header("Lobby Panel")]
    public GameObject battleCanvas;
    [Header("LoadingPanel")]
    public GameObject loadingPanel;


    //private vars
    private string lobbyNDummy;
    private string roomLevel;

    #region CallsFromPlayer
    public void Awake()
    {
        loadingPanel.SetActive(true);

        PhotonNetwork.AutomaticallySyncScene = true;
        
        PhotonNetwork.SendRate = 40;
        PhotonNetwork.SerializationRate = 14;
    }

    public void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void InstantiateCanvasForRoomCreation()
    {
        GameObject createRoom = Instantiate(createRoomPanel, battleCanvas.transform.position, battleCanvas.transform.rotation, battleCanvas.transform);
        PhotonNetwork.SendRate = 40;
        PhotonNetwork.SerializationRate = 14;
    }


    private int players;
    public void CreateRoom(string roomName, string lobbyName, int P, LobbyType lobbyType = LobbyType.Default)
    {
        Debug.Log("Joined a room: " + roomName + " " + lobbyName);
        TypedLobby sqlLobby = new TypedLobby(lobbyName, lobbyType);
        lobbyNDummy = lobbyName;
        players = P;
        roomLevel = roomName;
        RoomOptions roomOptions = this.GetRoomOptions();
        PhotonNetwork.CreateRoom(roomName + System.Guid.NewGuid().ToString(), roomOptions);

    }


    #endregion CallFromPlayers

    #region photonCallBacks

    //join random room fail here
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join Room Failed, create new room");
        InstantiateCanvasForRoomCreation();
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Client is in a room.");
        PhotonNetwork.LoadLevel(roomLevel);
    }

    public override void OnJoinedLobby()
    {
        loadingPanel.SetActive(false);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Disconnected", cause);
    }



    #endregion photonCallBacks

    #region RoomProps

    RoomOptions GetRoomOptions()
    {
        RoomOptions roomOptions = new RoomOptions();

        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "C", lobbyNDummy }, { "R", roomLevel + " " + lobbyNDummy }, { "T", (float)PhotonNetwork.Time } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "C", "R", "T" };

        roomOptions.MaxPlayers = (byte)players;
        roomOptions.EmptyRoomTtl = (byte)120;
        roomOptions.CleanupCacheOnLeave = false;
        roomOptions.DeleteNullProperties = true;
        /*
        _roomOptions.IsOpen = this.IsOpen;

        _roomOptions.IsVisible = this.IsVisible;
        
        roomOptions.PlayerTtl = this.PlayerTtl;

        //roomOptions.PublishUserId = this.PublishUserId;

        */

        return roomOptions;
    }
    #endregion RoomProps
}
