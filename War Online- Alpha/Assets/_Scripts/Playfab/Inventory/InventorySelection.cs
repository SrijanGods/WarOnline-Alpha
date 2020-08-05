using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class InventorySelection : MonoBehaviour
{

    [Header("TurretList")]
    public GameObject[] turretList;
    public int[] turretCost;
    public float[] turretLevel;
    public bool[] tActive;
    public string[] tID;

    [Header("HullList")]
    public GameObject[] hullList;
    public int[] hullCost;
    public float[] hullLevel;
    public bool[] hActive;
    public string[] hID;

    [Header("Gameobjects")]
    public GameObject loadingPanel;

    [HideInInspector]
    public bool inventoryLoaded;
    private GateKeeper playfabLogin;

    #region PublicMethods
    void Awake()
    {
        playfabLogin = GameObject.FindGameObjectWithTag("GameController").GetComponent<GateKeeper>();

        int hullno = hullList.Length;
        hullCost = new int[hullno];
        hActive = new bool[hullno];
        hID = new string[hullno];
        hullLevel = new float[hullno];

        int turretno = turretList.Length;
        turretCost = new int[turretno];
        tActive = new bool[turretno];
        tID = new string[turretno];
        turretLevel = new float[turretno];
    }

    private void Start()
    {
        StartCoroutine(LinkInventory());
    }

    IEnumerator LinkInventory()
    {
        yield return new WaitUntil(() => playfabLogin.PlayfabConnected);
        GetItems();
    }
    #endregion PublicMethods

    #region GettingUserInventory

    public void GetItems()
    {
        GetCatalogItemsRequest request = new GetCatalogItemsRequest();
        request.CatalogVersion = "Cat";

        PlayFabClientAPI.GetCatalogItems(request,
            result => {

                List<CatalogItem> items = result.Catalog;
                foreach (CatalogItem item in items)
                {
                    uint cost = item.VirtualCurrencyPrices["GB"];

                    if (item.ItemClass == "Turrets")
                    {
                        foreach (GameObject turret in turretList)
                        {
                            
                            if (turret.name == item.ItemId)
                            {
                                for (int i = 0; i < turretList.Length; i++)
                                {
                                    int turretno = System.Array.IndexOf(turretList, turret);
                                    turretCost.SetValue((int)cost, turretno);
                                    ++turretno;
                                }
                            }

                        }
                    }
                    else if (item.ItemClass == "Hull")
                    {
                        foreach(GameObject hull in hullList)
                        {
                           if (hull.name == item.ItemId)
                           {
                                for (int i = 0; i < hullList.Length; i++)
                                {
                                    int hullno = System.Array.IndexOf(hullList, hull);
                                    hullCost.SetValue((int)cost, hullno);
                                    break;
                                }
                           }

                        }
                    }
                }
            },
            error => {
                Debug.Log(error.ErrorDetails);
            });

        GetPlayerInventory();
    }

    private bool given;
    public void GetPlayerInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest { }, 
            res => 
            {
                List<ItemInstance> userItems = res.Inventory;
                if (userItems.Count == 0)
                {
                    if (!given)
                    {
                        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
                        {
                            ItemId = "Initial.Bundle",
                            Price = 0,
                            VirtualCurrency = "GB"
                        },
                        result2 =>
                        {
                            given = true;
                            GetItems();
                        },
                        error2 =>
                        {
                            Debug.Log(error2.ErrorMessage);
                        });
                    }
                }
                else
                {
                    foreach(ItemInstance item in userItems)
                    {
                        if(item.ItemClass == "Turrets")
                        {
                            foreach (GameObject turret in turretList)
                            {
                                if (turret.name == item.ItemId)
                                {
                                    for (int i = 0; i < turretList.Length; i++)
                                    {
                                        int tno = System.Array.IndexOf(turretList, turret);
                                        tActive.SetValue(true, tno);
                                        tID.SetValue(item.ItemInstanceId, tno);

                                        Dictionary<string, string> cd = item.CustomData;

                                        //if there is no custom data of the hoop, I execute cloud script to sync values
                                        if (cd == null)
                                        {
                                            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
                                            {
                                                FunctionName = "UpdatePlayerInventoryData",
                                                FunctionParameter = new { InstID = tID[tno], Level = 1f }
                                            },
                                            resultcs =>
                                            {
                                                turretLevel.SetValue(1f, tno);
                                            },
                                            error =>
                                            {
                                                print(error.Error);
                                            });
                                        }
                                        else
                                        {
                                            //else I take out the values and store them in the list
                                            string lvl;
                                            cd.TryGetValue("Level", out lvl);

                                            turretLevel.SetValue(float.Parse(lvl), tno);
                                        }
                                        ++tno;
                                    }
                                }
                            }
                        }
                        if(item.ItemClass == "Hull")
                        {
                            foreach (GameObject hull in hullList)
                            {
                                if (hull.name == item.ItemId)
                                {
                                    for (int i = 0; i < hullList.Length; i++)
                                    {
                                        int hno = System.Array.IndexOf(hullList, hull);
                                        hActive.SetValue(true, hno);
                                        hID.SetValue(item.ItemInstanceId, hno);

                                        Dictionary<string, string> cd = item.CustomData;

                                        //if there is no custom data of the hoop, I execute cloud script to sync values
                                        if (cd == null)
                                        {
                                            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
                                            {
                                                FunctionName = "UpdatePlayerInventoryData",
                                                FunctionParameter = new { InstID = hID[hno], Level = 1f }
                                            },
                                            resultcs =>
                                            {
                                            },
                                            error =>
                                            {
                                                print(error.Error);
                                            });
                                            hullLevel.SetValue(1f, hno);
                                        }
                                        else
                                        {
                                            //else I take out the values and store them in the list
                                            string lvl;
                                            cd.TryGetValue("Level", out lvl);
                                            
                                            hullLevel.SetValue(float.Parse(lvl), hno);
                                        }
                                        ++hno;
                                    }
                                }
                            }
                        }
                    }
                }
            }, 
            err => 
            { 
            
            });

        loadingPanel.SetActive(false);
    }

    #endregion GettingUserInventory
}
