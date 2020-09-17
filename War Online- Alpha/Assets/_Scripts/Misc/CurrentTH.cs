using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentTH : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentHullName;
    [SerializeField] TextMeshProUGUI currentTurrentName;
    [SerializeField] Image hullImage;
    [SerializeField] Image turrentImage;
    [SerializeField] Sprite[] turrentImages;
    [SerializeField] Sprite[] hullImages;
    [SerializeField] string hullName;
    [SerializeField] string turrentName;

    private void Start()
    {
        hullName = GlobalValues.hull;
        turrentName = GlobalValues.turret;
    }
    private void Update()
    {
        SetTHText();
        SetTurrentImage();
        SetHullImage();
    }

    private void SetTHText()
    {
        currentHullName.text = hullName;
        currentTurrentName.text = turrentName;
    }

    private void SetTurrentImage()
    {
        foreach (Sprite sprite in turrentImages)
        {
            if (sprite.name == turrentName)
            {

                turrentImage.sprite = sprite;
            }

            else {  return; }
            return;
        }
        return;
    }

    private void SetHullImage()
    {
        foreach (Sprite sprite in hullImages)
        {
            if (sprite.name == hullName)
            {
                hullImage.sprite = sprite;
            }

            else
            {
                return;
            }
            return;
        }
        return;
    }
}
