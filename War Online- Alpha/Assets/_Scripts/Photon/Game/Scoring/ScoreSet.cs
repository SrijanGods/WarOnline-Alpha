using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Photon.Game.Scoring
{
    [Serializable]
    public class TextScoreSet
    {
        public Text scoreName, scoreVisual;

        public string Score
        {
            get => scoreVisual.text;
            set => scoreVisual.text = value;
        }
    }

    [Serializable]
    public class SliderScoreSet
    {
        public Text scoreName;

        public Slider scoreVisual;

        public float Score
        {
            get => scoreVisual.value;
            set => scoreVisual.value = value;
        }
    }

    public class ScoreSet : MonoBehaviour
    {
        public TextScoreSet tss;
        public SliderScoreSet sss;

        public GameObject tssObj, sssObj;

        public void Init(string scoresName, Color nameColor, Color valueColor)
        {
            tss.scoreName.text = scoresName;

            tss.scoreName.color = nameColor;
            tss.scoreVisual.color = valueColor;

            tssObj.SetActive(true);

            gameObject.name = scoresName;
        }

        public void Init(string scoresName, int maxScore, Color nameColor, Color valueColor)
        {
            sss.scoreName.text = scoresName;
            sss.scoreVisual.maxValue = maxScore;

            sss.scoreName.color = nameColor;
            sss.scoreVisual.fillRect.GetComponent<Image>().color = valueColor;

            sssObj.SetActive(true);

            gameObject.name = scoresName;
        }
    }
}