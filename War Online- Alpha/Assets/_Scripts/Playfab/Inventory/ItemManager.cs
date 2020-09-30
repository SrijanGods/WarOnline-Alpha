using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public bool hull;
    public bool turret;
    public bool typePaints;

    private InventorySelection inventory;

    private TextMeshProUGUI damage;
    private TextMeshProUGUI reload;
    private GameObject buyBtn;
    private GameObject equipBtn;
    private GameObject equippedBtn;

    private GameObject previewBtn;

    private void Start()
    {
        if (turret || hull)
        {
            damage = transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            reload = transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            buyBtn = transform.GetChild(1).gameObject;
            equipBtn = transform.GetChild(2).gameObject;
            equippedBtn = transform.GetChild(3).gameObject;

            if (turret)
            {
                equipBtn.GetComponent<Button>().onClick.AddListener(SetTurretState);
            }
            if (hull)
            {
                equipBtn.GetComponent<Button>().onClick.AddListener(SetHullState);
            }
            buyBtn.GetComponent<Button>().onClick.AddListener(BuyFlow);
        }
        if (typePaints)
        {
            buyBtn = transform.GetChild(0).gameObject;
            previewBtn = transform.GetChild(1).gameObject;
            equipBtn = transform.GetChild(2).gameObject;
            equippedBtn = transform.GetChild(3).gameObject;

            equipBtn.GetComponent<Button>().onClick.AddListener(SetPaintState);
        }
        inventory = GameObject.FindGameObjectWithTag("GameController").GetComponent<InventorySelection>();

        StartCoroutine(StartSync());
    }

    IEnumerator StartSync()
    {
        yield return new WaitUntil(() => inventory.inventoryLoaded);
        CheckItem();
    }

    public void Update()
    {
        StartCoroutine("UpdateItems");
    }

    IEnumerator UpdateItems()
    {
        yield return new WaitForSeconds(3.5f);
        CheckItem();
    }

    private void CheckItem()
    {
        if (turret)
        {
            for (int i = 0; i < inventory.turretList.Length; i++)
            {
                if(inventory.turretList[i].name == gameObject.name)
                {
                    if(inventory.tActive[i] == true)
                    {
                        buyBtn.SetActive(false);
                        equipBtn.SetActive(true);
                        equippedBtn.SetActive(false);

                        if(GlobalValues.turret == gameObject.name)
                        {
                            equippedBtn.SetActive(true);
                            equipBtn.SetActive(false);

                            GlobalValues.Instance.Reload = inventory.tReload[i];
                            GlobalValues.Instance.Damage = inventory.tDamage[i];
                            GlobalValues.Instance.Dist = inventory.tDist[i];
                            GlobalValues.Instance.Impact = inventory.tImpact[i];
                            GlobalValues.Instance.Rotation = inventory.tRotation[i];

                            damage.text = inventory.tDamage[i].ToString();
                            reload.text = inventory.tReload[i].ToString();
                        }
                    }
                }
            }
        }

        if (hull)
        {
            for (int i = 0; i < inventory.hullList.Length; i++)
            {
                if (inventory.hullList[i].name == gameObject.name)
                {
                    if (inventory.hActive[i] == true)
                    {
                        buyBtn.SetActive(false);
                        equipBtn.SetActive(true);
                        equippedBtn.SetActive(false);

                        if (GlobalValues.hull == gameObject.name)
                        {
                            equippedBtn.SetActive(true);
                            equipBtn.SetActive(false);

                            GlobalValues.Instance.Health = inventory.hHealth[i];
                            GlobalValues.Instance.Speed = inventory.hSpeed[i];
                            GlobalValues.Instance.Turn = inventory.hTurn[i];
                            GlobalValues.Instance.Acc = inventory.hAcc[i];
                            GlobalValues.Instance.Deacc = inventory.hDeacc[i];
                        }
                    }
                }
            }
        }

        if (typePaints)
        {
            if(gameObject.name.StartsWith("Matte") == true)
            {
                for (int i = 0; i < inventory.matteName.Length; i++)
                {
                    if (inventory.matteName[i] == gameObject.name)
                    {
                        if (inventory.matteActive[i] == true)
                        {
                            buyBtn.SetActive(false);
                            equipBtn.SetActive(true);
                            equippedBtn.SetActive(false);

                            if (GlobalValues.colour == gameObject.name)
                            {
                                equippedBtn.SetActive(true);
                                equipBtn.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }

    private void SetTurretState()
    {
        GlobalValues.turret = gameObject.name;
        PlayerPrefs.DeleteKey("CurrentTurret");
        PlayerPrefs.SetString("CurrentTurret", gameObject.name);

        equipBtn.SetActive(false);
        equippedBtn.SetActive(true);
    }

    private void SetHullState()
    {
        GlobalValues.hull = gameObject.name;
        PlayerPrefs.DeleteKey("CurrentHull");
        PlayerPrefs.SetString("CurrentHull", gameObject.name);

        equipBtn.SetActive(false);
        equippedBtn.SetActive(true);
    }

    private void SetPaintState()
    {
        GlobalValues.colour = gameObject.name;
        PlayerPrefs.DeleteKey("CurrentColour");
        PlayerPrefs.SetString("CurrentColour", gameObject.name);

        equipBtn.SetActive(false);
        equippedBtn.SetActive(true);
    }

    private void BuyFlow()
    {
        GameObject buyP = inventory.buyPanel;
        buyP.SetActive(true);

        if (turret) 
        {
            int i = System.Array.FindIndex(inventory.turretDisplayName, s => s == gameObject.name);
            buyP.GetComponent<BuyManager>().GetOrder(gameObject.name, inventory.turretCost[i], "GB");
        }
        if (hull)
        {
            int i = System.Array.FindIndex(inventory.hullDisplayName, s => s == gameObject.name);
            buyP.GetComponent<BuyManager>().GetOrder(gameObject.name, inventory.hullCost[i], "GB");
        }
    }
}
