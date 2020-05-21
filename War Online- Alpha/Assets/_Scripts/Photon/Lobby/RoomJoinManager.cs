using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class RoomJoinManager : MonoBehaviour
{
    public Image mapImg;
    public List<Sprite> sprites = new List<Sprite>();
    public GameObject infoPanel;
    public Button joinBtn;

    public void Start()
    {
        //mapImg.GetComponent<Image>().sprite = sprites[1];
    }

    #region JoinToRoom
    public void OnClick_JoinRoom()
    {
        Photon.Pun.PhotonNetwork.JoinRoom(copyInfo.Name);
    }

    #endregion JoinToRoom

    #region RoomInfo

    private object RoomName;
    private RoomInfo copyInfo;

    public void RoomSetInfo(RoomInfo info)
    {
        copyInfo = info;
        TextMeshProUGUI roomN = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        if (info.CustomProperties.TryGetValue("R", out RoomName))
        {
            roomN.SetText((string)RoomName);
        }
        else
        {
            Debug.Log("Noob");
        }
    }

    private int index;
    public void OnClick_CreateInfoInst()
    {
        if (transform.parent.GetComponent<ObjectHolder>().objectToHold.transform.childCount == 2)
        {
            Destroy(transform.parent.GetComponent<ObjectHolder>().objectToHold.transform.GetChild(1).gameObject);
        }

        GameObject iPanel = Instantiate(infoPanel, transform.parent.GetComponent<ObjectHolder>().objectToHold.transform.position,
                                              transform.parent.GetComponent<ObjectHolder>().objectToHold.transform.rotation,
                                              transform.parent.GetComponent<ObjectHolder>().objectToHold.transform);

        //Transform blocker = iPanel.gameObject.GetComponentInParent<Transform>().transform.GetChild(0);
        //Debug.Log(blocker.name);
        //blocker.gameObject.SetActive(true);

        mapImg = iPanel.transform.GetChild(0).GetComponent<Image>();
        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == RoomName.ToString())
            {
                index = sprites.IndexOf(sprite);
            }
        }

        mapImg.sprite = sprites.ToArray().GetValue(index) as Sprite;

        joinBtn = iPanel.transform.GetChild(1).GetComponent<Button>();
        joinBtn.onClick.AddListener(OnClick_JoinRoom);
    }

    #endregion RoomInfo
}
