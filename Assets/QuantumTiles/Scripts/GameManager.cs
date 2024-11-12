using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    [Header("Options")]

    [SerializeField] private float _canSelectDelay = 2.0f;
    [SerializeField] private float _memorizeDelay = 3.0f;

    private List<CardController> _flippedCards = new List<CardController>();
    private GameStats _stats;

    // Total number of matched pairs to win the game
    private int _totalMatchesRequired;
    private int _currentMatches;

    // Lock for card selection
    private bool _canSelectCard = true;

    public static GameManager Instance;

    public static UnityAction OnFlip;
    public static UnityAction OnMatch;
    public static UnityAction OnMismatch;
    public static UnityAction OnGameOver;
    public static UnityAction<GameStats> OnGameLoad;
    public static UnityAction<GameStats> OnStatsUpdate;


    private void Awake()
    {
        Instance = this;
    }
    public void StartNewGame(int slots)
    {
        _totalMatchesRequired = slots;
        _currentMatches = 0;
        _stats = new GameStats();
        UpdateUI();
    }

    private void OnEnable()
    {
        CardController.OnCardFlipped += HandleCardFlip;
    }

    private void OnDisable()
    {
        CardController.OnCardFlipped -= HandleCardFlip;
    }

    private void HandleCardFlip(CardController card)
    {

        OnFlip?.Invoke();

        // Add the flipped card to the list
        _flippedCards.Add(card);
        if (_flippedCards.Count == 2)
        {
            // Disable further card flipping
            _canSelectCard = false;
            StartCoroutine(CheckForMatchCoroutine());
        }
    }

    private IEnumerator CheckForMatchCoroutine()
    {
        yield return new WaitForSeconds(_memorizeDelay); // Wait before checking

        _stats.attempts++;
        CardController firstCard = _flippedCards[0];
        CardController secondCard = _flippedCards[1];

        if (firstCard.CardID == secondCard.CardID)
        {
            Debug.Log("Got Match " + firstCard.CardID);
            firstCard.MatchCard();
            secondCard.MatchCard();
            MatchedCards();
        }
        else
        {
            Debug.Log("No Match " + firstCard.CardID + " ! " + secondCard.CardID);
            MismatchedCards();
        }

        // Delay
        yield return new WaitForSeconds(_canSelectDelay);
        // Re-enable card selection
        _canSelectCard = true;
        UpdateUI();
    }

    private void MatchedCards()
    {
        IncreaseScore();
        _currentMatches++;
        OnMatch?.Invoke();
        //AudioManager.Instance.PlayMatchSound();
        _flippedCards.Clear();

        if (_currentMatches == _totalMatchesRequired)
        {
            GameOver();
        }
    }

    private void MismatchedCards()
    {
        _stats.combo = 0;

        foreach (CardController card in _flippedCards)
        {
            card.UnflipCard();
        }
        OnMismatch?.Invoke();
        //AudioManager.Instance.PlayMismatchSound();
        _flippedCards.Clear();
    }

    private void IncreaseScore()
    {
        _stats.combo++;
        _stats.score += 10 * _stats.combo; // Increase score based on combo multiplier
    }

    private void GameOver()
    {
        OnGameOver?.Invoke();
    }

    private void UpdateUI()
    {
        OnStatsUpdate?.Invoke(_stats);
    }

    public bool CanSelectCard()
    {
        return _canSelectCard;
    }

    public void SaveGame()
    {
       StartCoroutine(SaveRoutine());
    }

    private IEnumerator SaveRoutine()
    {
        
         yield return new WaitForSeconds(.5f);

        GameStats gameState = new GameStats
        {
            score = _stats.score,
            attempts = _stats.attempts,
            combo = _stats.combo,
            cards = new List<CardData>(),
            matchesRequired = _totalMatchesRequired,
            currentMatches = _currentMatches
        };

        // Gather data for each card
        foreach (var card in FindObjectsOfType<CardController>())
        {
            CardData cardData = new CardData
            {
                cardID = card.CardID,
                isFlipped = card.IsFlipped(),
                isMatched = card.IsMatched(),
                position = card.transform.position,
                scale = card.transform.localScale
            };
            gameState.cards.Add(cardData);
        }

        string json = JsonUtility.ToJson(gameState);
        File.WriteAllText(Application.persistentDataPath + "/gameState.json", json);
        Debug.Log("Game saved.");
    }

    public void LoadGame()
    {
        StopAllCoroutines();
        _flippedCards.Clear();
        string path = Application.persistentDataPath + "/gameState.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameStats gameStat = JsonUtility.FromJson<GameStats>(json);

            // Restore game data
            _stats.score = gameStat.score;
            _stats.attempts = gameStat.attempts;
            _stats.combo = gameStat.combo;
            _totalMatchesRequired = gameStat.matchesRequired;
            _currentMatches = gameStat.currentMatches;

            OnGameLoad?.Invoke(gameStat);
            Debug.Log("Game loaded.");
            OnStatsUpdate?.Invoke(gameStat);
        }
        else
        {
            Debug.Log("No saved game found.");
        }
    }
}
