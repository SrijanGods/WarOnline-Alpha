using _Scripts.Photon.Game;
using UnityEngine;

namespace _Scripts.Photon.Room
{
    public class BattleMode : MonoBehaviour
    {
        public MainScript ms;

        public void DeathMatch(bool teams)
        {
            GlobalValues.Session = teams ? GameSessionType.Teams : GameSessionType.Ffa;
            GlobalValues.GameMode = GameMode.DeathMatch;

            ms.JoinRandomBattle(GlobalValues.Session + "_" + GlobalValues.GameMode);
        }
    }
}