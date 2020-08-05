using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemManager : MonoBehaviour
{
    public bool hull;
    public bool turret;

    private InventorySelection inventory;

    private TextMeshProUGUI damage;
    private TextMeshProUGUI reload;
    private GameObject buyBtn;
    private GameObject equipBtn;
    private GameObject equippedBtn;

    private void Start()
    {
        damage = transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        reload = transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        buyBtn = transform.GetChild(1).gameObject;
        equipBtn = transform.GetChild(2).gameObject;
        equippedBtn = transform.GetChild(3).gameObject;

        inventory = GameObject.FindGameObjectWithTag("GameController").GetComponent<InventorySelection>();
        StartCoroutine(StartSync());
    }

    IEnumerator StartSync()
    {
        yield return new WaitUntil(() => inventory.inventoryLoaded);
        CheckItem();
    }

    private void CheckItem()
    {
        
    }
}
