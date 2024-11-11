using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // List to track flipped cards
    [SerializeField] private List<CardController> _flippedCards = new List<CardController>();

    // Scoring
    private int _score = 0;
    private int _comboCount = 0;
    private int _attemptsCount = 0;

    // UI references (score display, game over panel, etc.)
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _comboText;
    [SerializeField] private TMP_Text _attemptsText;
    [SerializeField] private GameObject _gameOverPanel;
    //[SerializeField] GameObject _board;


    // Total number of matched pairs to win the game
    private int _totalMatchesRequired;
    private int _currentMatches;

    private void Start()
    {
        // Calculate the number of pairs based on total cards in the scene
        _totalMatchesRequired = GameObject.FindGameObjectsWithTag("Card").Length / 2;
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
        // Add the flipped card to the list
        _flippedCards.Add(card);

        // Check if we have two cards flipped
        if (_flippedCards.Count == 2)
        {
            CheckForMatch();
        }
    }

    private void CheckForMatch()
    {
        _attemptsCount++;
        // Get the two flipped cards
        CardController firstCard = _flippedCards[0];
        CardController secondCard = _flippedCards[1];

        // Check if their IDs match
        if (firstCard.CardID == secondCard.CardID)
        {
            // Cards match
            firstCard.MatchCard();
            secondCard.MatchCard();
            IncreaseScore();
            _currentMatches++;
            Debug.Log("Got Match " + firstCard.CardID);
            // Check for game completion
            if (_currentMatches == _totalMatchesRequired)
            {
                GameOver();
            }
        }
        else
        {
            Debug.Log("No Match " + firstCard.CardID + " ! " + secondCard.CardID );
            // Cards don't match; reset combo and flip them back
            _comboCount = 0;
            Invoke(nameof(ResetCards), 1.0f); // Wait a second before flipping back
            return;
        }

        // Clear the flippedCards list for the next attempt
        _flippedCards.Clear();
        UpdateUI();
    }

    private void ResetCards()
    {
        foreach (CardController card in _flippedCards)
        {
            card.UnflipCard();
        }
        _flippedCards.Clear();
        UpdateUI();
    }

    private void IncreaseScore()
    {
        _comboCount++;
        _score += 10 * _comboCount; // Increase score based on combo multiplier
    }

    private void GameOver()
    {
        _gameOverPanel.SetActive(true);
    }

    private void UpdateUI()
    {
        _comboText.text = "Combo " + _comboCount;
        _attemptsText.text = "Attempts " + _attemptsCount;
        _scoreText.text = "Score: " + _score;
    }
}
