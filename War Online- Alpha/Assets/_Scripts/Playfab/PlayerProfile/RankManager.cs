using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankManager : MonoBehaviour
{
    public string[] rankName;
    public Sprite[] rankImage;
    public Image currentRankImage;
    public string currentRank;
    public int currentExp, rankL, rankH;

    [HideInInspector]
    public int xpInInt;

    public void AssignRank()
    {
        if(currentExp < 100)
        {
            currentRankImage.sprite = rankImage[0];
            currentRank = rankName[0];
            rankL = 0; rankH = 99;
            xpInInt = 1;
        }
        else if(currentExp >= 100 && currentExp < 500)
        {
            currentRankImage.sprite = rankImage[1];
            currentRank = rankName[1];
            rankL = 100; rankH = 499;

            xpInInt = 2;
        }
        else if (currentExp >= 500 && currentExp < 1500)
        {
            currentRankImage.sprite = rankImage[2];
            currentRank = rankName[2];
            rankL = 500; rankH = 1499;

            xpInInt = 3;
        }
        else if (currentExp >= 1500 && currentExp < 3000)
        {
            currentRankImage.sprite = rankImage[3];
            currentRank = rankName[3];
            rankL = 1500; rankH = 2999;

            xpInInt = 4;
        }
        else if (currentExp >= 3000 && currentExp < 5500)
        {
            currentRankImage.sprite = rankImage[4];
            currentRank = rankName[4];
            rankL = 3000; rankH = 5499;

            xpInInt = 5;
        }
        else if (currentExp >= 5500 && currentExp < 8000)
        {
            currentRankImage.sprite = rankImage[5];
            currentRank = rankName[5];
            rankL = 5500; rankH = 7999;

            xpInInt = 6;
        }
        else if (currentExp >= 8000 && currentExp < 12000)
        {
            currentRankImage.sprite = rankImage[6];
            currentRank = rankName[6];
            rankL = 8000; rankH = 11999;

            xpInInt = 7;
        }
        else if (currentExp >= 12000 && currentExp < 16500)
        {
            currentRankImage.sprite = rankImage[7];
            currentRank = rankName[7];
            rankL = 12000; rankH = 16499;

            xpInInt = 8;
        }
        else if (currentExp >= 16500 && currentExp <20500)
        {
            currentRankImage.sprite = rankImage[8];
            currentRank = rankName[8];
            rankL = 16500; rankH = 20499;

            xpInInt = 9;
        }
        else if (currentExp >= 20500 && currentExp < 35000)
        {
            currentRankImage.sprite = rankImage[9];
            currentRank = rankName[9];
            rankL = 20500; rankH = 35999;

            xpInInt = 10;
        }
        else if (currentExp >= 35000)
        {
            currentRankImage.sprite = rankImage[10];
            currentRank = rankName[10];
            rankL = 35000; rankH = 100000;

            xpInInt = 11;
        }

        int intTofloat = currentExp;
        gameObject.GetComponent<GettingProfil>().rankName.text = currentRank;
    }

    public void Update()
    {
        

    }
}
