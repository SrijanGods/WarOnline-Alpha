using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankManager : MonoBehaviour
{
    public Slider rankSlider;
    public string[] rankName;
    public Image[] rankImage;
    public Image currentRankImage;
    public string currentRank;
    public int currentExp, rankL, rankH;

    public void AssignRank()
    {
        if(currentExp < 100)
        {
            currentRankImage = rankImage[0];
            currentRank = rankName[0];
            rankL = 0; rankH = 99;
        }
        else if(currentExp >= 100 && currentExp < 500)
        {
            currentRankImage = rankImage[1];
            currentRank = rankName[1];
            rankL = 100; rankH = 499;
        }
        else if (currentExp >= 500 && currentExp < 1500)
        {
            currentRankImage = rankImage[2];
            currentRank = rankName[2];
            rankL = 500; rankH = 1499;
        }
        else if (currentExp >= 1500 && currentExp < 3000)
        {
            currentRankImage = rankImage[3];
            currentRank = rankName[3];
            rankL = 1500; rankH = 2999;
        }
        else if (currentExp >= 3000 && currentExp < 5500)
        {
            currentRankImage = rankImage[4];
            currentRank = rankName[4];
            rankL = 3000; rankH = 5499;
        }
        else if (currentExp >= 5500 && currentExp < 8000)
        {
            currentRankImage = rankImage[5];
            currentRank = rankName[5];
            rankL = 5500; rankH = 7999;
        }
        else if (currentExp >= 8000 && currentExp < 12000)
        {
            currentRankImage = rankImage[6];
            currentRank = rankName[6];
            rankL = 8000; rankH = 11999;
        }
        else if (currentExp >= 12000 && currentExp < 16500)
        {
            currentRankImage = rankImage[7];
            currentRank = rankName[7];
            rankL = 12000; rankH = 16499;
        }
        else if (currentExp >= 16500 && currentExp <20500)
        {
            currentRankImage = rankImage[8];
            currentRank = rankName[8];
            rankL = 16500; rankH = 20499;
        }
        else if (currentExp >= 20500 && currentExp < 35000)
        {
            currentRankImage = rankImage[9];
            currentRank = rankName[9];
            rankL = 20500; rankH = 35999;
        }
        else if (currentExp >= 35000)
        {
            currentRankImage = rankImage[10];
            currentRank = rankName[10];
            rankL = 35000; rankH = 100000;
        }

        rankSlider.minValue = rankL;
        rankSlider.maxValue = rankH;
        int intTofloat = currentExp;
        rankSlider.value = (float)intTofloat;
        gameObject.GetComponent<GettingProfil>().actualRankName = currentRank;
    }

    public void Update()
    {
        

    }
}
