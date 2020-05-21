using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class InGameRoomManager : MonoBehaviourPunCallbacks
{

    //public static RoomManager roomManager;
    /*[Header("Spawn Points")]
    public GameObject[] spawnPoints;
    */
    [Header("UI")]
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI playersNoH;
    public TextMeshProUGUI maxPlayer;
    public TextMeshProUGUI timeLeftS;
    public TextMeshProUGUI timeLeftM;

    //change match time duration here
    private float timer = 100f;

    private int playerNos;
    private int maxPlayrs;
    private float timerIncrementValue;
    private float startTime;
    private float roomOpenTime;
    private float decTimer;

    private bool timerOn = false;

    #region PublicFunctions

    public void Start()
    {
    }
    public void Update()
    {
        StartCoroutine("TimelyStatsUpdate");

        if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            PhotonNetwork.CurrentRoom.RemovedFromList = true;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        else
        {
            PhotonNetwork.CurrentRoom.RemovedFromList = false;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }

        StartCoroutine("TimerUpdate");
    }
    #endregion PublicFunctions

    #region IENums

    IEnumerator TimelyStatsUpdate()
    {
        playerNos = PhotonNetwork.CurrentRoom.PlayerCount;
        playersNoH.SetText(playerNos.ToString());

        yield return new WaitForSeconds(0.5f);
    }

    private float diff;
    IEnumerator TimerUpdate()
    {
        yield return new WaitForSeconds(0.5f);
        float currentTime = (float)PhotonNetwork.Time;

        timerIncrementValue = currentTime - startTime;
        timerIncrementValue %= 1000;
        decTimer = timer - timerIncrementValue;
        decTimer = Mathf.Round(decTimer);

        if (decTimer >= 60)
        {
            timeLeftM.SetText(((int)decTimer / 60).ToString());
            timeLeftS.SetText((decTimer % 60).ToString());
        }
        else
        {
            timeLeftM.SetText("00");
            timeLeftS.SetText(decTimer.ToString());
        }

        //print(currentTime + " " + diff + " " + roomOpenTime +  " " + startTime + " " + decTimer + " " + timerIncrementValue);
        if (decTimer <= 0f)
        {
            print("Match Over");
        }

    }


    private object RoomName;
    private object time;
    public override void OnEnable()
    {

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("R", out RoomName))
        {
            roomName.SetText((string)RoomName);
        }
        else
        {
            Debug.Log("Noob");
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("T", out time))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine("GetTime", 2f);
            }
            else
            {
                object temp;
                object holder = PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("ST", out temp);
                startTime = (float)temp;
            }
        }
        else
        {
            Debug.Log("Noob");
        }


        playerNos = PhotonNetwork.CurrentRoom.PlayerCount;
        playersNoH.SetText(playerNos.ToString());

        maxPlayrs = PhotonNetwork.CurrentRoom.MaxPlayers;
        maxPlayer.SetText(maxPlayrs.ToString());
    }

    IEnumerator GetTime()
    {
        yield return new WaitForSeconds(0.5f);
        if (PhotonNetwork.Time == 0f)
        {
            OnEnable();
        }
        // else
        //{
        roomOpenTime = (float)time;
        roomOpenTime = Mathf.Round(roomOpenTime);
        float currentTime = (float)PhotonNetwork.Time;
        diff = currentTime - roomOpenTime;
        startTime = roomOpenTime + diff;
        timerOn = true;

        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "ST", startTime } });
        //}
    }

    #endregion IENums

    public override void OnLeftRoom()
    {
        // PhotonView pv = this.gameObject
        Destroy(gameObject);
    }
}
