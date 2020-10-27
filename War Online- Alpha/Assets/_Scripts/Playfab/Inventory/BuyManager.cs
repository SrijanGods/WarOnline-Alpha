using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class BuyManager : MonoBehaviour
{
    [Header("Buy Panel")]
    public GameObject buyPanel;
    public TextMeshProUGUI objName;
    public TextMeshProUGUI objValue;
    public Button buyBtn;

    [Space]
    public GameObject lessPanel;
    public GameObject succPanel;

    private GettingProfil profile;
    private string ID;

    public void Awake()
    {
        profile = GameObject.FindGameObjectWithTag("GameController").GetComponent<GettingProfil>();
        buyBtn.onClick.AddListener(SetOrder);
    }

    public void GetOrder(string itemName, int itemPrice, string currID)
    {
        succPanel.SetActive(false);
        lessPanel.SetActive(false);
        ID = currID;
        if (currID == "GB")
        {
            if (profile.GBcurrency >= itemPrice)
            {
                objName.text = itemName;
                objValue.text = itemPrice.ToString();
                buyPanel.SetActive(true);
            }
            else
            {
                buyPanel.SetActive(false);
                lessPanel.SetActive(true);
            }
        }
        if(currID == "KR")
        {
            if (profile.KRcurrency >= itemPrice)
            {
                objName.text = itemName;
                objValue.text = itemPrice.ToString();
                buyPanel.SetActive(true);
            }
            else
            {
                buyPanel.SetActive(false);
                lessPanel.SetActive(true);
            }
        }
    }

    public void SetOrder()
    {
        profile.gameObject.GetComponent<InventorySelection>().BuyItem(objName.text, Int32.Parse(objValue.text), ID);
        buyPanel.SetActive(false);
        succPanel.SetActive(true);
    }
}
