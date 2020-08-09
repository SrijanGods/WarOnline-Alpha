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

    private GettingProfil profile;
    private string ID;
    public void Start()
    {
        profile = GameObject.FindGameObjectWithTag("GameController").GetComponent<GettingProfil>();
        buyBtn.onClick.AddListener(SetOrder);
    }

    public void GetOrder(string itemName, int itemPrice, string currID)
    {
        ID = currID;
        if (currID == "GB")
        {
            if (itemPrice >= profile.GBValue)
            {
                objName.text = itemName;
                objValue.text = itemPrice.ToString();
                buyPanel.SetActive(true);
            }
            else
            {
                lessPanel.SetActive(true);
            }
        }
        if(currID == "KR")
        {
            if (itemPrice >= profile.KRValue)
            {
                objName.text = itemName;
                objValue.text = itemPrice.ToString();
                buyPanel.SetActive(true);
            }
            else
            {
                lessPanel.SetActive(true);
            }
        }
    }

    public void SetOrder()
    {
        profile.GetComponent<InventorySelection>().BuyItem(objName.text, Int32.Parse(objValue.text), ID);
        buyPanel.SetActive(false);
    }
}
