using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsLoader : MonoBehaviour
{
    public GameObject thisCanvas;
    public int indexNoToSet;
    private Button button;
    public bool forTurrets = false;

    private GameObject tHolder;
    private GameObject hHolder;
    private InventorySelection inventorySelection;
    private bool isDone = false;
    private GameObject ip;

    #region PublicMethods
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(Load);
        inventorySelection = GameObject.FindGameObjectWithTag("GarageCanvas").GetComponent<InventorySelection>();
        ip = tHolder.GetComponentInParent<RectTransform>().transform.gameObject;
    }

    public void Load()
    {
        while (!isDone)
        {

            if (thisCanvas.transform.childCount > 4)
            {
                GameObject g = thisCanvas.transform.GetChild(4).gameObject;
                g.SetActive(false);
                string s = g.name.Substring(g.name.Length - 1);

                if(s == "t")
                {
                    g.transform.SetParent(tHolder.transform);
                }
                if(s == "h")
                {
                    g.transform.SetParent(hHolder.transform);
                }
                
            }
            else
            {
                if (forTurrets)
                {
                    GameObject objectToActivate = tHolder.transform.Find("InfoPanel" + indexNoToSet + "t").gameObject;
                    objectToActivate.transform.SetParent(thisCanvas.transform);
                    objectToActivate.SetActive(true);
                    objectToActivate.GetComponent<clickvalue>().turret = true;
                    isDone = true;
                }
                else
                {
                    GameObject objectToActivate = hHolder.transform.Find("InfoPanel" + indexNoToSet + "h").gameObject;
                    objectToActivate.SetActive(true);
                    objectToActivate.transform.SetParent(thisCanvas.transform);
                    isDone = true;
                }
            }
        }
        isDone = false;

        //GameObject panel = Instantiate(statsPanel, thisCanvas.transform.position, Quaternion.identity, thisCanvas.transform);
        //thisCanvas.GetComponent<InventorySelection>().value = indexNoToSet;
        //panel.GetComponent<clickvalue>().turret = forTurrets;
        //thisCanvas.GetComponent<InventorySelection>().isturret = forTurrets;

    }
    #endregion PublicMethods
}
