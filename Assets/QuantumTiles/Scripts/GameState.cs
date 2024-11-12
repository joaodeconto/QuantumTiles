using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    public int Score;
    public int Combo;
    public int Attempts;
    public int CurrentMatches;
    public int MatchesRequired;
    public List<CardData> Cards = new List<CardData>();
}
