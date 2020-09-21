using System;

namespace _Scripts.Photon.Game
{
    [Serializable]
    public enum GameSessionType
    {
        Ffa,
        Teams
    }

    [Serializable]
    public enum GameMode
    {
        DeathMatch, Elimination, TeamConquest, KingOfTheHill
    }
}