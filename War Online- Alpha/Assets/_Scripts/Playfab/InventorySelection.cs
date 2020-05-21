using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class InventorySelection : MonoBehaviour
{

    [Header("TurretList")]
    public GameObject[] turretList;
    private TurretChange turretNames;
    public int[] turretCost;
    public int[] turretCostU;
    public string[] turretDisplayN;
    public string[] turretDes;
    public string[] turretID;

    [Header("HullList")]
    private HullChange hullNames;
    public GameObject[] hullList;
    public int[] hullCost;
    public int[] hullCostU;
    public string[] hullDisplayN;
    public string[] hullID;
    public string[] hullDes;
    public bool isturret;
    private clickvalue script;

    [Header("GameObject")]
    public GameObject turretHolder;
    public GameObject hullHolder;
    private GameObject statsPanel;

    #region PublicMethods
    void Awake()
    {
        hullNames = GameObject.FindWithTag("ChoiceController").GetComponent<HullChange>();
        int hullno = hullNames.hulls.Length;
        hullList = hullNames.hulls;
        hullCost = new int[hullno];
        hullCostU = new int[hullno];
        hullDisplayN = new string[hullno];
        hullDes = new string[hullno];
        hullID = new string[hullno];

        //turret script calls
        turretNames = GameObject.FindWithTag("ChoiceController").GetComponent<TurretChange>();
        int turretno = turretNames.turrets.Length;
        turretList = turretNames.turrets;
        turretCost = new int[turretno];
        turretCostU = new int[turretno];
        turretDisplayN = new string[turretno];
        turretDes = new string[turretno];
        turretID = new string[turretno];
    }

    private void Start()
    {
        statsPanel = Resources.Load("HullTurretSet") as GameObject;
        GetItemsPrices();

        StartCoroutine(UserInventoryInfo());
    }

    void Update()
    { /*
        if(this.gameObject.transform.childCount > 3)
        {
            if (isturret)
            {
                script = gameObject.transform.GetChild(3).GetComponent<clickvalue>();
                script.displayName.text = (string)turretDisplayN.GetValue(value);
                script.assetCost = (int)turretCost.GetValue(value);
                script.assetCostU = (int)turretCostU.GetValue(value);
                script.itemID = (string)turretID.GetValue(value);
                script.value = (int)value;
            }
            else
            {
                script = gameObject.transform.GetChild(3).GetComponent<clickvalue>();
                script.displayName.text = (string)hullDisplayN.GetValue(value);
                script.assetCost = (int)hullCost.GetValue(value);
                script.assetCostU = (int)hullCostU.GetValue(value);
                script.itemID = (string)hullID.GetValue(value);
                script.value = (int)value;
            }
        } */
    }
    #endregion PublicMethods
    

    #region GettingUserInventory

    public void GetItemsPrices()
    {
        GetCatalogItemsRequest request = new GetCatalogItemsRequest();
        request.CatalogVersion = "Cat";

        PlayFabClientAPI.GetCatalogItems(request,
            result => {

                List<CatalogItem> items = result.Catalog;
                foreach (CatalogItem item in items)
                {
                    uint cost = item.VirtualCurrencyPrices["GB"];
                    string displayName = item.DisplayName;
                    string description = item.Description;
                    string ID = item.ItemId;

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
                                    // turretCostU.SetValue((int)costU, turretno);
                                    turretDisplayN.SetValue(displayName, turretno);
                                    turretDes.SetValue(description, turretno);
                                    turretID.SetValue(ID, turretno);
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
                                    Debug.Log(hull.name + " " + item.ItemId + " " + hullno);
                                    hullCost.SetValue((int)cost, hullno);
                                    // hullCostU.SetValue((int)costU, hullno);
                                    hullDisplayN.SetValue(displayName, hullno);
                                    hullDes.SetValue(description, hullno);
                                    hullID.SetValue(ID, hullno);
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
    }

    #endregion GettingUserInventory

    #region UserInventoryInfo

    private int tNo = 0;
    private int hNo = 0;

    public IEnumerator UserInventoryInfo()
    {
        yield return new WaitForSeconds(3f);

        foreach (GameObject turretAsset in turretList)
        {
            GameObject load = Instantiate(statsPanel, turretHolder.transform.position, Quaternion.identity, turretHolder.transform);
            clickvalue script = load.GetComponent<clickvalue>();
            int value = tNo;
            load.transform.gameObject.name = "InfoPanel" + value + "t";
            script.displayName.text = (string)turretDisplayN.GetValue(value);
            script.description.text = (string)turretDes.GetValue(value);
            script.assetCost = (int)turretCost.GetValue(value);
            script.assetCostU = (int)turretCostU.GetValue(value);
            script.itemID = (string)turretID.GetValue(value);
            script.value = (int)value;
            tNo += 1;
            load.SetActive(false);
        }

        foreach (GameObject hullAsset in hullList)
        {
            GameObject load = Instantiate(statsPanel, hullHolder.transform.position, Quaternion.identity, hullHolder.transform);
            clickvalue script = load.GetComponent<clickvalue>();
            int value = hNo;
            load.transform.gameObject.name = "InfoPanel" + value + "h";
            script.displayName.text = (string)hullDisplayN.GetValue(value);
            script.description.text = (string)hullDes.GetValue(value);
            script.assetCost = (int)hullCost.GetValue(value);
            script.assetCostU = (int)hullCostU.GetValue(value);
            script.itemID = (string)hullID.GetValue(value);
            script.value = (int)value;
            hNo += 1;
            load.SetActive(false);
        }
    }
    #endregion
}
