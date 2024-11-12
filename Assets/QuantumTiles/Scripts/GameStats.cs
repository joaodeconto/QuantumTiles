using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameStats
{
    public int score = 0;
    public int combo = 0;
    public int attempts = 0;
    public int timeSpam = 0;
    public List<CardData> cards;
    public int matchesRequired;
    public int currentMatches;
}
