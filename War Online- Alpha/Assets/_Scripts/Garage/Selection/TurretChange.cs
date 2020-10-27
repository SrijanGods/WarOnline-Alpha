using System.Collections;
using System;
using UnityEngine;
using Substance.Game;
using UnityEngine.UI;

public class TurretChange : MonoBehaviour
{
    public int selection;
    public GameObject[] turrets;
    public GameObject turret;
    public bool now = true;

    private InventorySelection inventory;

    void Start()
    {
        StartCoroutine(UpdateTurret());
        inventory = GameObject.FindGameObjectWithTag("GameController").GetComponent<InventorySelection>();
    }

    IEnumerator UpdateTurret()
    {
        yield return new WaitForSeconds(2f);
        yield return new WaitUntil(() => GlobalValues.turret != null);
        yield return new WaitUntil(() => now);
        DisableAll();
        // int selection = Array.FindIndex(turrets, g => g.name == GlobalValues.turret);
        // turrets[selection].SetActive(true);
        turret = Array.Find(turrets, g => g.name == GlobalValues.turret);
        turret.SetActive(true);

        string paint = GlobalValues.colour;
        if (paint.StartsWith("Matte") == true)
        {
            int i = Array.FindIndex(inventory.matteName, g => g == GlobalValues.colour);
            GameObject body = turret.transform.GetChild(0).gameObject;
            body.GetComponent<Renderer>().material = inventory.matte.material;
            Material mat = body.GetComponent<MeshRenderer>().sharedMaterial;
            SubstanceGraph gr = inventory.matte;
            gr.SetInputColor("Color1", inventory.color1[i]);
            gr.SetInputColor("Color2", inventory.color2[i]);
            gr.QueueForRender();
            Substance.Game.Substance.RenderSubstancesAsync();
            mat = gr.material;
        }
    }
    void DisableAll()
    {
        for (int x = 0; x < turrets.Length; x++)
        {
            turrets[x].SetActive(false);
        }

    }
    private void Update()
    {
        StartCoroutine(UpdateTurret());
    }
}
