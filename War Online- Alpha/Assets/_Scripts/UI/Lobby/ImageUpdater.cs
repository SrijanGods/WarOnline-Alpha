﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageUpdater : MonoBehaviour
{
    public Sprite[] hullImages;
    public Sprite[] turretImages;

    public Image currHull;
    public Image currTurret;

    public TextMeshProUGUI turretText;
    public TextMeshProUGUI hullText;

    private void Start()
    {
        StartCoroutine(SyncHull());
        StartCoroutine(SyncTurret());
    }

    IEnumerator SyncHull()
    {
        yield return new WaitUntil(() => GlobalValues.hull != null);
        hullText.text = GlobalValues.hull;

        int i = Array.FindIndex(hullImages, g => g.name == hullText.text);
        //print(i + " " + hullText.text);
        currHull.sprite = hullImages[i];
    }

    IEnumerator SyncTurret()
    {
        yield return new WaitUntil(() => GlobalValues.turret != null);
        turretText.text = GlobalValues.turret;

        int i = Array.FindIndex(turretImages, g => g.name == turretText.text);
        currTurret.sprite = turretImages[i];
    }

    private void Update()
    {
        StartCoroutine(SyncImage());
    }

    IEnumerator SyncImage()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(SyncHull());
    }
}
