using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomList : MonoBehaviourPunCallbacks
{
    //PS: this is not create room script

    [Header("Common Objects")]
    public GameObject dropDown;
    public Transform contentDM;
    public Transform contentTDM;
    public TextMeshProUGUI errorMessage;


    [Header("Prefabs")]
    public GameObject roomBtn;

    [SerializeField]
    private string lobbyinType = "DM";
    private List<GameObject> roomInstantsDM = new List<GameObject>();
    private List<GameObject> roomInstantsTDM = new List<GameObject>();

    #region PublicFunctions
    public void Start()
    {
        GetRoomList();
    }
    public void Update()
    {
        StartCoroutine("LobbyCalls");
    }

    IEnumerator LobbyCalls()
    {
        yield return new WaitForSeconds(0.5f);
        if (lobbyinType == "DM" && contentTDM.gameObject.activeInHierarchy == true)
        {
            contentTDM.gameObject.SetActive(false);
            contentDM.gameObject.SetActive(true);
        }
        if (lobbyinType == "TDM" && contentDM.gameObject.activeInHierarchy == true)
        {
            contentDM.gameObject.SetActive(false);
            contentTDM.gameObject.SetActive(true);
        }

        if (roomInstantsTDM.Count == 0)
        {
            errorMessage.SetText("No rooms found for " + lobbyinType + ". You may create your own room.");
        }
        if (roomInstantsTDM.Count == 0)
        {
            errorMessage.SetText("No rooms found for " + lobbyinType + ". You may create your own room.");
        }
    }

    public void GetRoomList()
    {
        TMP_Dropdown dD = dropDown.GetComponent<TMP_Dropdown>();
        lobbyinType = dD.options[dD.value].text;

        TypedLobby sqlLobby = new TypedLobby(lobbyinType, LobbyType.Default);
        string correctedName = "C0 = " + "'" + lobbyinType + "'";
        PhotonNetwork.GetCustomRoomList(sqlLobby, correctedName);
    }

    #endregion PublicFunctions

    #region RoomListUpate

    private object lobbytype;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count == 0)
        {
            errorMessage.SetText("No rooms found for " + lobbyinType + ". You may create your own room.");
            foreach (GameObject objects in roomInstantsDM)
            {
                Destroy(objects.gameObject);
            }
            print("hmm");
        }
        else
        {
            errorMessage.SetText("");
        }

        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.CustomProperties.TryGetValue("C", out lobbytype))
            {
                print(lobbytype.ToString());
                errorMessage.SetText("");
            }
            else
            {
                print("noob");
                lobbytype = (string)"noVal";
            }

            if ((string)lobbytype != "noVal")
            {
                if (lobbytype.ToString() == "TDM")
                {
                    if (roomInfo.RemovedFromList == true || roomInfo.IsOpen == false || roomInfo.IsVisible == false)
                    {
                        int index = roomInstantsTDM.FindIndex(x => x.gameObject.name == roomInfo.Name);
                        if (index >= -1)
                        {
                            Destroy(roomInstantsTDM[index].gameObject);
                            roomInstantsTDM.RemoveAt(index);
                        }
                    }
                    else
                    {
                        print(roomInfo.Name + " " + roomInstantsTDM);
                        if (roomInstantsTDM.Count < 35)
                        {
                            GameObject temp = roomInstantsTDM.Where(x => x.name == roomInfo.Name).SingleOrDefault();
                            if (temp == null)
                            {
                                GameObject instBtn = Instantiate(roomBtn, contentTDM.position, contentTDM.rotation, contentTDM);
                                instBtn.name = roomInfo.Name;
                                instBtn.GetComponent<RoomJoinManager>().RoomSetInfo(roomInfo);

                                roomInstantsTDM.Add(instBtn);
                            }
                        }
                    }
                }

                if (lobbytype.ToString() == "DM")
                {
                    if (roomInfo.RemovedFromList == true || roomInfo.IsOpen == false || roomInfo.IsVisible == false)
                    {
                        int index = roomInstantsDM.FindIndex(x => x.gameObject.name == roomInfo.Name);
                        print("Closed " + index);
                        if (index > -1)
                        {
                            Destroy(roomInstantsDM[index].gameObject);
                            roomInstantsDM.RemoveAt(index);
                        }
                    }
                    else
                    {
                        print(roomInfo.Name + " " + roomInstantsDM);
                        if (roomInstantsDM.Count < 35)
                        {
                            GameObject temp = roomInstantsDM.Where(x => x.name == roomInfo.Name).SingleOrDefault();
                            if (temp == null)
                            {
                                GameObject instBtn = Instantiate(roomBtn, contentDM.position, contentDM.rotation, contentDM);
                                instBtn.name = roomInfo.Name;
                                instBtn.GetComponent<RoomJoinManager>().RoomSetInfo(roomInfo);

                                roomInstantsDM.Add(instBtn);
                            }
                            else
                            {
                                print("It's True" + (roomInstantsDM.Where(x => x.name == roomInfo.Name)));
                            }
                        }
                    }
                }



            }
            else
            {
                GameObject dmInst = roomInstantsDM.Where(x => x.name == roomInfo.Name).SingleOrDefault();
                if (dmInst != null)
                {
                    dmInst.SetActive(false);
                }

                GameObject tdmInst = roomInstantsTDM.Where(x => x.name == roomInfo.Name).SingleOrDefault();
                if (tdmInst != null)
                {
                    tdmInst.SetActive(false);
                }
            }
        }
    }
    #endregion RoomListUpdate
}
