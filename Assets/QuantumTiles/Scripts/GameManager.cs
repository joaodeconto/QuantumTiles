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
    private GameState _gameState = new GameState();

    // Total number of matched pairs to win the game
    private int _totalMatchesRequired;
    private int _currentMatches;

    // Lock for card selection
    private bool _canSelectCard = true;

    private bool _persistantSave = true;

    public bool PersistentSave { get { return _persistantSave; } set { _persistantSave = value; } }
    public static GameManager Instance;

    public static UnityAction OnFlip;
    public static UnityAction OnMatch;
    public static UnityAction OnMismatch;
    public static UnityAction OnGameOver;
    public static UnityAction<GameState> OnGameLoad;
    public static UnityAction<GameState> OnStatsUpdate;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void StartNewGame(int slots)
    {
        _totalMatchesRequired = slots;
        _currentMatches = 0;
        _gameState = new();
        UpdateUI();
    }

    private void OnEnable()
    {
        CardController.OnCardFlipped += HandleCardFlip;
    }

    private void OnDestroy()
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

    public void ClearFlipedCards()
    {
        _flippedCards.Clear();
    }

    private IEnumerator CheckForMatchCoroutine()
    {
        yield return new WaitForSeconds(_memorizeDelay); // Wait before checking

        _gameState.Attempts++;
        CardController firstCard = _flippedCards[0];
        CardController secondCard = _flippedCards[1];

        if (firstCard.CardID == secondCard.CardID)
        {
            //Match
            Debug.Log("Got Match " + firstCard.CardID);
            firstCard.MatchCard();
            secondCard.MatchCard();
            MatchedCards();
        }
        else
        {
            //Mismatch
            Debug.Log("No Match " + firstCard.CardID + " ! " + secondCard.CardID);
            MismatchedCards();
        }

        // Delay
        yield return new WaitForSeconds(_canSelectDelay);
        // Re-enable card selection
        _canSelectCard = true;
        UpdateUI();

        if(_persistantSave) 
            SaveGame();
    }

    private void MatchedCards()
    {
        IncreaseScore();
        _currentMatches++;
        OnMatch?.Invoke();
        _flippedCards.Clear();

        if (_currentMatches == _totalMatchesRequired)
        {
            GameOver();
        }
    }

    private void MismatchedCards()
    {
        _gameState.Combo = 0;

        foreach (CardController card in _flippedCards)
        {
            card.UnflipCard();
        }
        OnMismatch?.Invoke();
        _flippedCards.Clear();
    }

    private void IncreaseScore()
    {
        _gameState.Combo++;
        _gameState.Score += 10 * _gameState.Combo;
    }

    private void GameOver()
    {
        OnGameOver?.Invoke();
    }

    private void UpdateUI()
    {
        OnStatsUpdate?.Invoke(_gameState);
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
        // Delay to end coroutines
         yield return new WaitForSeconds(1f);

        GameState gameState = GetGameStateSnapshot();

        // Gather data for each card
        foreach (var card in FindObjectsOfType<CardController>())
        {
            CardData cardData = new()
            {
                ID = card.CardID,
                IsFlipped = card.IsFlipped(),
                IsMatched = card.IsMatched(),
                Position = card.transform.position,
                Scale = card.transform.localScale
            };
            gameState.Cards.Add(cardData);
        }

        string json = JsonUtility.ToJson(gameState);
        File.WriteAllText(Application.persistentDataPath + "/gameSave.json", json);
        Debug.Log(json);
    }

    private GameState GetGameStateSnapshot()
    {
        GameState gameState = new()
        {
            Score = _gameState.Score,
            Attempts = _gameState.Attempts,
            Combo = _gameState.Combo,
            Cards = new List<CardData>(),
            MatchesRequired = _totalMatchesRequired,
            CurrentMatches = _currentMatches
        };
        return gameState;
    }

    public void LoadGame()
    {
        StopAllCoroutines();
        _flippedCards.Clear();
        string path = Application.persistentDataPath + "/gameSave.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameState gameStat = JsonUtility.FromJson<GameState>(json);

            // Restore game data
            _gameState.Score = gameStat.Score;
            _gameState.Attempts = gameStat.Attempts;
            _gameState.Combo = gameStat.Combo;
            _totalMatchesRequired = gameStat.MatchesRequired;
            _currentMatches = gameStat.CurrentMatches;

            OnGameLoad?.Invoke(gameStat);
            Debug.Log("Game loaded.");
            OnStatsUpdate?.Invoke(gameStat);
            UpdateUI();
        }
        else
        {
            Debug.Log("No saved game found.");
        }
    }
}
