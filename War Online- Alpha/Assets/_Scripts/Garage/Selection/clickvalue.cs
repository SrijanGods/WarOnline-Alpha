using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class clickvalue : MonoBehaviour

{

    public TextMeshProUGUI displayName;
    public TextMeshProUGUI description;
    public string itemID;
    public int assetCost;
    public int assetCostU;
    private GameObject scriptToChange;
    public bool turret;
    public int value;
    public Button equipButton;
    public Button actualBuyButton;
    public Button buyButton;
    
    public GameObject errorPanel;
    public GameObject buyPanel;

    [SerializeField]
    private bool canEquip;
    private int currentGB;

    #region PublicMethods
    public void Start()
    {
        UserInventoryCheck();
        scriptToChange = GameObject.FindWithTag("ChoiceController");
        currentGB = GameObject.FindWithTag("GameController").GetComponent<GettingProfil>().GBValue;
        equipButton.onClick.AddListener(Execute);
    }

    public void Update()
    {
        if (canEquip)
        {
            equipButton.gameObject.SetActive(true);
            buyButton.gameObject.SetActive(false);
        }
        else
        {
            equipButton.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(true);
            actualBuyButton.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Are You Sure you want to buy this item for: " + "GB " + assetCost.ToString();
        }

    }

    #endregion PublicMethod

    #region CheckUserInventory

    public void UserInventoryCheck()
    {
        GetUserInventoryRequest request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request,
            result => {

                List<ItemInstance> items = result.Inventory;
                if (items.Count == 0)
                {
                    GrantItem();
                }
                else
                {
                    foreach (ItemInstance item in items)
                    {
                        string itemName = item.DisplayName;
                        if (itemName == displayName.text)
                        {
                            canEquip = true;
                            break;
                        }
                        else if (itemName != displayName.text)
                        {
                            canEquip = false;
                        }
                    }
                }

            },
            error => {
                Debug.Log(error.ErrorMessage);
            });

        equipButton.onClick.AddListener(Execute);
        actualBuyButton.onClick.AddListener(BuyAsset);
    }

    #endregion CheckUserInventory

    #region AssetBuyCall
    public void BuyAsset()
    {
        if(assetCost > currentGB)
        {
            errorPanel.SetActive(true);
            errorPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Not enough GB to buy " + displayName.text;
        }
        else if (assetCost <= currentGB)
        {
            MakePurchase();
        }
    }
    #endregion AssetBuyCall

    #region MakePuchase
    void MakePurchase()
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            CatalogVersion = "Cat",
            ItemId = itemID,
            Price = assetCost,
            VirtualCurrency = "GB"
        }, 
        result => {
            buyPanel.SetActive(true);
            buyPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Asset Successfully Bought: " + displayName.text;
            canEquip = true;
            GameObject.FindWithTag("ChoiceController").GetComponent<GettingProfil>().GetPlayerCombinedInfo();
        }, 
        error=> {
            Debug.Log("Buying Asset Error" + " " + error.ErrorMessage);
        });
    }
    #endregion MakePurchase

    #region GrantingItem
    void GrantItem() {
        
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
        {
            ItemId = "NewPlayer",
            Price = 0,
            VirtualCurrency = "GB"
        },
        result =>
        {
            Debug.Log("Default Item Bought");
        },
        error => {
            Debug.Log(error.ErrorMessage);
        });
}
    #endregion GrantingItem

    #region Execute
    [SerializeField]
    private void Execute()
    {
        if (turret)
        {
            scriptToChange.GetComponent<TurretChange>().selection = value;
        }
        else
        {
            scriptToChange.GetComponent<HullChange>().selection = value;
        }
    }
    #endregion Execute
}
