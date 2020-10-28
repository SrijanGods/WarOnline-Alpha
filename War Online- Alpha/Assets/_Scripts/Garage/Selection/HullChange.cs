using System.Collections;
using System;
using UnityEngine;
using Substance.Game;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HullChange : MonoBehaviour {
    public int selection;
    public GameObject[] hulls;
    public GameObject hull;
    public bool now = true;

    private InventorySelection inventory;

    void Start ()
    {
        UpdateHull();
        inventory = GameObject.FindGameObjectWithTag("GameController").GetComponent<InventorySelection>();
    }

    IEnumerator UpdateHull()
    {
        yield return new WaitForSeconds(2f);
        yield return new WaitUntil(() => GlobalValues.hull != null);
        DisableAll();
        // int selection = Array.FindIndex(hulls, g => g.name == GlobalValues.hull);
        // hulls[selection].SetActive(true);

        hull = Array.Find(hulls, g => g.name == GlobalValues.hull);
        hull.SetActive(true);

        string paint = GlobalValues.colour;
        if(paint.StartsWith("Matte") == true && now)
        {
            int i = Array.FindIndex(inventory.matteName, g => g == GlobalValues.colour);
            GameObject body = hull.transform.GetChild(0).GetChild(0).gameObject;
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

    private void Update()
    {
        if (now)
        {
            StartCoroutine(UpdateHull());
        }
    }

    void DisableAll()
    {
        for(int x = 0; x<hulls.Length; x++)
        {
            hulls[x].SetActive(false);
        }
      
    }

}
