using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    public void Start()
    {
        profile = GameObject.FindGameObjectWithTag("GameController").GetComponent<GettingProfil>();
        buyBtn.onClick.AddListener(SetOrder);
    }

    public void GetOrder(string itemName, int itemPrice)
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

    public void SetOrder()
    {

    }
}
