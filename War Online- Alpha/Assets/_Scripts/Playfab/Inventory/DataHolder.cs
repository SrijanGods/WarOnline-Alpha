using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder: MonoBehaviour
{
    [Header("Turrets")]
    public float[] damage;
    public float[] reload;
    public float[] distance;
    public float[] rotation;
    public float[] impact;

    [Header("Hull")]
    public float[] health;
    public float[] speed;
    public float[] turnRate;
    public float[] acc;
    public float[] deAcc;
    public float[] weight;

    [Header("Color")]
    public List<string> name = new List<string>();
    public List<Color> colour = new List<Color>();

    public Dictionary<string, Color> matte;

    private void Awake()
    {
        for(int i = 0; i < name.Count; i++)
        {
            matte.Add(name[i], colour[i]);
        }
    }
}
