using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;



public class GettingProfil : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI NickName;
    [HideInInspector]
    [SerializeField]
    private string playFabId;
    [SerializeField]
    private string actualUserName;
    [SerializeField]
    public string actualRankName;
    public TextMeshProUGUI GB;
    public TextMeshProUGUI KR;


    [HideInInspector]
    [SerializeField]
    private string GBCurrency;
    [HideInInspector]
    [SerializeField]
    private string KRCurrency;
    [HideInInspector]
    [SerializeField]
    private int GBcurrency;
    public int GBValue
    {
        get { return GBcurrency; }
        set { GBcurrency = value; }
    }
    [HideInInspector]
    [SerializeField]
    private int KRcurrency;

    public GameObject newsPanel;
    public GameObject newsView;

    public GetPlayerCombinedInfoRequestParams info;

    public GameObject GCanvas;

    private InventorySelection inventory;
    private GateKeeper gateKeeper;

    #region PublicMethods
    void Start()
    {
        inventory = GCanvas.GetComponent<InventorySelection>();
        gateKeeper = gameObject.GetComponent<GateKeeper>();
        StartCoroutine("StartStats");
    }

    IEnumerator StartStats()
    {
        yield return new WaitUntil(() => gateKeeper.PlayfabConnected);

        GetPlayerProfile();
        GetStats();
        GetNews();
        GetPlayerCombinedInfo();
    }

    #endregion PublicMethods

    #region PlayerProfile


    public void GetPlayerProfile()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest()
        {

        },
        result =>
        {
            actualUserName = result.AccountInfo.Username;
        },
        error => Debug.Log(error.GenerateErrorReport()));

        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        },
        result => {

            if (result.PlayerProfile.DisplayName == null)
            {
                result.PlayerProfile.DisplayName = actualUserName;
                NickName.text = actualRankName + " " + result.PlayerProfile.DisplayName;
            }
            else
            {
                NickName.text = actualRankName + " " + result.PlayerProfile.DisplayName;
            }
        },
        error => Debug.LogError(error.GenerateErrorReport()));
        
    }
    #endregion PlayerProfile

    #region PlayerStats

    [SerializeField]
    private int rankExp;
    [SerializeField]
    private int destroyed;

    public void SetStats()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate>
            {
            new StatisticUpdate { StatisticName = "RankExp", Value = 0},
            }

        },
    result => 
    { 
        Debug.Log("User statistics updated");
        GetStats();
    },
     error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    void GetStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            result => 
            { 
                if(result.Statistics.Count == 0)
                {
                    SetStats();
                }
                else
                {
                    foreach (var eachStat in result.Statistics)
                    {
                        switch (eachStat.StatisticName)
                        {
                            case "RankExp":
                                rankExp = eachStat.Value;
                                break;
                        }
                    }

                    RankManager rankManager = gameObject.GetComponent<RankManager>();
                    rankManager.currentExp = rankExp;
                    rankManager.AssignRank();

                    NickName.text = actualRankName + " " + actualUserName;
                }
            },
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }


    #endregion PlayerStats

    #region PlayerNews

    void GetNews()
    {
        GetTitleNewsRequest request = new GetTitleNewsRequest();
        request.Count = 5;
        PlayFabClientAPI.GetTitleNews(request, result => {
            List<TitleNewsItem> news = result.News;
            foreach (TitleNewsItem item in news)
            {
                GameObject p = Instantiate(newsPanel, newsView.gameObject.GetComponentInParent<Transform>().gameObject.GetComponentInParent<ScrollRect>().gameObject.transform.position, Quaternion.identity);
                p.transform.SetParent(newsView.transform);

                p.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.Title;
                p.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.Body;
            }
            
            }, 
            error => {

            });
    }

    #endregion PlayerNews

    #region PlayerCurrency

    public void GetPlayerCombinedInfo()
    {
        GetPlayerCombinedInfoRequest request = new GetPlayerCombinedInfoRequest()
        {
            InfoRequestParameters = info

        };
        PlayFabClientAPI.GetPlayerCombinedInfo(request, 
            result =>
            {
                request.InfoRequestParameters.GetUserVirtualCurrency = true;
                GBcurrency = result.InfoResultPayload.UserVirtualCurrency["GB"];
                KRcurrency = result.InfoResultPayload.UserVirtualCurrency["KR"];

                GBCurrency = GBcurrency.ToString();
                KRCurrency = KRcurrency.ToString();

                GB.text = GBCurrency;
                KR.text = KRCurrency;
            }, 
            error => 
            {
                Debug.Log(error.ErrorMessage);
            });
    }
    #endregion PlayerCurrency


}
