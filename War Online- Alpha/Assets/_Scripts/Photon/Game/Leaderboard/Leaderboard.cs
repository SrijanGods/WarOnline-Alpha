using System.Globalization;
using System.Linq;
using _Scripts.Photon.Room;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.Photon.Game.Leaderboard
{
    public class Leaderboard : MonoBehaviour
    {
        public Text winnerNameDisplay;

        public Transform scoreParent;

        public GameObject playerScorePrefab;

        public GameMap map;
        public GameSession Session => map.SessionObject;

        public float leaderBoardTime = 10;

        private void OnEnable()
        {
            // var playerIDsSortedDescendingByScores = from entry in Session.PlayersScoresByActorID
            //     orderby entry.Value descending
            //     select entry.Key;
            DBG.Log("LeaderBoard Enabled");
            Debug.Assert(Session != null);
            Debug.Assert(Session.PlayersScoresByActorID != null);

            var playerSortedDescendingByScores = Session.PlayersScoresByActorID.OrderByDescending(key => key.Value);

            int winnerID = 0;
            foreach (var keyValue in playerSortedDescendingByScores)
            {
                var id = keyValue.Key;

                var player = Session.AllPlayers.Find(p => p.ActorNumber == id);
                var pName = player.NickName;
                var pTeam = Session.PlayersTeamIndexByActorID[id];

                // way to get the most scoring player's name on top
                if (winnerID == 0)
                {
                    winnerID = id;

                    winnerNameDisplay.text = GlobalValues.Session == GameSessionType.Teams
                        ? pTeam + " Team Won the Game!"
                        : "Player " + pName + " Won the Game!";
                }

                var pScores = Session.PlayersScoresByActorID[id];
                var pKills = Session.PlayersKillsByActorID[id];
                var pDeaths = Session.PlayersDeathsByActorID[id];

                var pColor = GlobalValues.Session == GameSessionType.Teams
                    ? GlobalValues.TeamColors[pTeam]
                    : Color.black;

                SetScore(pName, pColor, pScores, pKills, pDeaths);
            }
            
            PhotonNetwork.LeaveRoom();

            Invoke(nameof(GoHome), leaderBoardTime);
        }

        private void SetScore(string playerName, Color nameColor, float scores, int kills, int deaths)
        {
            var i = Instantiate(playerScorePrefab, scoreParent);

            var c = i.GetComponent<LeaderboardPlayerScore>();

            c.playerName.text = playerName;
            c.playerName.color = nameColor;

            c.scores.text = scores.ToString(CultureInfo.InvariantCulture);
            c.kills.text = kills.ToString();
            c.deaths.text = deaths.ToString();
        }

        public void GoHome()
        {
            SceneManager.LoadScene(1);
        }
    }
}