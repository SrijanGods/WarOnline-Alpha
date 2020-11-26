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
            DBG.BeginMethod("CreateScore");
            GameObject ScoreSet_object = Instantiate(scorePrefab, setParent);
            DBG.Log("scorePrefab.name: " + scorePrefab.name);
            DBG.Log("ScoreSet_object.name: " + ScoreSet_object.name);

            ScoreSet ScoreSet_component = ScoreSet_object.GetComponent<ScoreSet>();
            ss.Add(key, ScoreSet_component);

            DBG.EndMethod("CreateScore");
            return ScoreSet_component;
        }
    }
}