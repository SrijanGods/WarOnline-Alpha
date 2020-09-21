using _Scripts.Photon.Game.Scoring;
using UnityEngine;

namespace _Scripts.Photon.Game
{
    public class DeathMatch : GameSession
    {
        private ScoreBoard _sb;

        public override void StartSession()
        {
            base.StartSession();
            _sb = FindObjectOfType<ScoreBoard>();

            _sb.scoreType = ScoreType.Text;

            _sb.gameName.text = GlobalValues.Session + "DeathMatch";
            _sb.description.text = "The player with most kills at the end of the game wins.";

            if (GlobalValues.Session == GameSessionType.Teams)
            {
                _sb.description.text = "The team with most kills at the end of the game wins.";

                for (int i = 0; i < GlobalValues.TeamNames.Length; i++)
                {
                    var tn = GlobalValues.TeamNames[i];
                    var tc = GlobalValues.TeamColors[i];

                    var s = _sb.CreateScore(tn);
                    s.Init(tn, tc, Color.white);
                    s.tss.Score = "0";
                }

                Debug.Log(nameof(base.OnPlayerDie));
            }
        }

        public override void OnPlayerSpawn(int playerActorID, int teamIndex, int ffaColorIndex)
        {
            base.OnPlayerSpawn(playerActorID, teamIndex, ffaColorIndex);

            if (GlobalValues.Session == GameSessionType.Teams) return;

            var s = _sb.CreateScore(playerActorID.ToString());
            s.Init(AllPlayers.Find((player => player.ActorNumber == playerActorID)).NickName,
                GlobalValues.FfaColors[ffaColorIndex], Color.white);
            s.tss.Score = "0";
        }

        public override void OnPlayerDie(int dyingPlayerID, int killerID)
        {
            base.OnPlayerDie(dyingPlayerID, killerID);

            if (GlobalValues.Session == GameSessionType.Teams)
            {
                var tn = GlobalValues.TeamNames[playersTeamIndexByActorID[killerID]];

                _sb.ss[tn].tss.Score = (int.Parse(_sb.ss[tn].tss.Score) + 1).ToString();
            }
            else
            {
                var n = killerID.ToString();

                _sb.ss[n].tss.Score = (int.Parse(_sb.ss[n].tss.Score) + 1).ToString();
            }
        }
    }
}