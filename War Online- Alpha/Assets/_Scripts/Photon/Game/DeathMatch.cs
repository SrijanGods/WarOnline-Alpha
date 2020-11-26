using _Scripts.Photon.Game.Scoring;
using UnityEngine;

namespace _Scripts.Photon.Game
{
    public class DeathMatch : GameSession
    {
        private ScoreBoard _sb;

        public override void StartSession()
        {
            DBG.BeginMethod("StartSession");
            MatchEndTime = GlobalValues.Session == GameSessionType.Teams ? 450 : 600;
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
            base.StartSession();
            DBG.EndMethod("StartSession");
        }

        public override void OnPlayerSpawn(int playerActorID, int teamIndex, int ffaColorIndex)
        {
            DBG.BeginMethod("OnPlayerSpawn");
            base.OnPlayerSpawn(playerActorID, teamIndex, ffaColorIndex);

            if (GlobalValues.Session == GameSessionType.Teams) return;

            var s = _sb.CreateScore(playerActorID.ToString());
            s.Init(AllPlayers.Find((player => player.ActorNumber == playerActorID)).NickName,
                GlobalValues.FfaColors[ffaColorIndex], Color.white);
            s.tss.Score = "0";
            DBG.EndMethod("OnPlayerSpawn");
        }

        public override void OnPlayerDie(int dyingPlayerID, int killerID)
        {
            base.OnPlayerDie(dyingPlayerID, killerID);

            if (!PlayersScoresByActorID.ContainsKey(killerID)) PlayersScoresByActorID[killerID] = 0;
            PlayersScoresByActorID[killerID] += 5;

            if (GlobalValues.Session == GameSessionType.Teams)
            {
                var tn = GlobalValues.TeamNames[PlayersTeamIndexByActorID[killerID]];

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