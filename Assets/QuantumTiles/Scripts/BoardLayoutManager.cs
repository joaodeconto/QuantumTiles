using System.Collections.Generic;
using UnityEngine;

public class BoardLayoutManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int _rows = 2;
    [SerializeField] private int _columns = 2;
    [SerializeField] private float _spacing = 0.1f;

    [Header("Card Settings")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardContainer; // Reference to the container holding the cards

    private List<GameObject> spawnedCards = new List<GameObject>();
    private int Slots { get { return (_rows * _columns) / 2; } }

    private void Start()
    {
        // Set Total Matches at GameManager
        GameManager.Instance.TotalMatches = Slots;
        ArrangeAndShuffleCards();
    }

    public void ArrangeAndShuffleCards()
    {
        // Clear any existing cards
        foreach (GameObject card in spawnedCards)
        {
            Destroy(card);
        }
        spawnedCards.Clear();

        // Generate a list of shuffled card IDs
        List<int> shuffledCardIDs = GenerateShuffledCardIDs();

        // Calculate card size based on container and grid settings
        Vector2 cardSize = CalculateCardSize();

        // Arrange cards in a grid and assign IDs
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                // Instantiate the card at the calculated position
                Vector3 position = CalculateCardPosition(i, j, cardSize);
                GameObject card = Instantiate(cardPrefab, position, Quaternion.identity, cardContainer);

                // Set card size
                card.transform.localScale = new Vector3(cardSize.x, 1, cardSize.y);

                // Set the card ID and store it
                CardController cardController = card.GetComponent<CardController>();
                cardController.CardID = shuffledCardIDs[i * _columns + j];

                // Add card to the spawned cards list
                spawnedCards.Add(card);
            }
        }
    }

    private List<int> GenerateShuffledCardIDs()
    {
        // Duplicate each card ID to create pairs
        List<int> deck = new List<int>();

        for (int i = 0; i < Slots; i++)
        { 
            deck.Add(i);
            deck.Add(i);
        }

        // Shuffle the list of IDs
        for (int i = 0; i < deck.Count; i++)
        {
            int randIndex = Random.Range(0, deck.Count);
            int temp = deck[i];
            deck[i] = deck[randIndex];
            deck[randIndex] = temp;
        }

        return deck;
    }

    private Vector2 CalculateCardSize()
    {
        // Calculate the width and height based on the container's size
        float containerWidth = cardContainer.GetComponent<Transform>().localScale.x;
        float containerHeight = cardContainer.GetComponent<Transform>().localScale.z;
        float cardWidth = (containerWidth - (_columns - 1) * _spacing) / _columns;
        float cardHeight = (containerHeight - (_rows - 1) * _spacing) / _rows;

        return new Vector2(.1f, .1f);
    }

    private Vector3 CalculateCardPosition(int row, int col, Vector2 cardSize)
    {
        // Calculate the starting position to center the grid
        float startX = -((_columns - 1) * (cardSize.x + _spacing)) / 2;
        float startY = ((_rows - 1) * (cardSize.y + _spacing)) / 2;

        // Calculate the exact position for each card
        float posX = startX + col * (cardSize.x + _spacing);
        float posY = startY - row * (cardSize.y + _spacing);

        return new Vector3(posX, 0, posY);
    }
}
