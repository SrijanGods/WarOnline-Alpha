using UnityEngine;
using TMPro;

public class CreateRoom : MonoBehaviour
{
    [Header("Room Props")]
    public GameObject mapSelector;
    public GameObject maxPlayers;
    public GameObject dropDown;

    private MainScript photonConnect;
    private string lobbyType = "DM";
    private string mapType;
    private string maxPlayersN;

    public void Start()
    {
        photonConnect = GameObject.FindGameObjectWithTag("Photon").GetComponent<MainScript>();
    }

    #region SendInfo
    public void SendInfo()
    {
        TMP_Dropdown dD2 = mapSelector.GetComponent<TMP_Dropdown>();
        mapType = dD2.options[dD2.value].text;
        string roomName = mapType;

        TMP_Dropdown dD3 = maxPlayers.GetComponent<TMP_Dropdown>();
        maxPlayersN = dD3.options[dD3.value].text;
        int maxPlayer;
        int.TryParse(maxPlayersN, out maxPlayer);

        TMP_Dropdown dD = dropDown.GetComponent<TMP_Dropdown>();
        lobbyType = dD.options[dD.value].text;
        photonConnect.CreateRoom(roomName, lobbyType, maxPlayer);
    }
    #endregion SendInfo

}
