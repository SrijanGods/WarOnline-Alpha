using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Photon.Game.Scoring
{
    public enum ScoreType
    {
        Text,
        Slider
    }

    public class ScoreBoard : MonoBehaviourPun
    {
        public Transform setParent;
        public GameObject scorePrefab;

        public Text gameName, description;

        public ScoreType scoreType;

        public Dictionary<string, ScoreSet> ss = new Dictionary<string, ScoreSet>();

        public ScoreSet CreateScore(string key)
        {
            var s = Instantiate(scorePrefab, setParent).GetComponent<ScoreSet>();
            ss.Add(key, s);
            return s;
        }
    }
}