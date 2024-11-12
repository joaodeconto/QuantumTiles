using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // List to track flipped cards
    [Header("Flipped Debug")]
    [SerializeField] private List<CardController> _flippedCards = new List<CardController>();
    [Header("Options")]
    [SerializeField] private float _canSelectDelay = 2.0f;
    [SerializeField] private float _memorizeDelay = 3.0f;

    private GameStats _stats = new GameStats();

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
    public static UnityAction<GameStats> OnStatsUpdate;

    public int TotalMatches
    {
        get { return _totalMatchesRequired; }
        set { _totalMatchesRequired = value; }
    }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
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

        _stats._attemptsCount++;
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
        _stats._comboCount = 0;

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
        _stats._comboCount++;
        _stats._score += 10 * _stats._comboCount; // Increase score based on combo multiplier
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
}
