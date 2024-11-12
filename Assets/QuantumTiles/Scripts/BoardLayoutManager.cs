using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class BoardLayoutManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int _rows = 2;
    [SerializeField] private int _columns = 2;
    [SerializeField] private float _spacing = 0.1f;

    [Header("Card Settings")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private Transform boardPlane;

    private List<GameObject> spawnedCards = new List<GameObject>();
    private int TotalSlots => _rows * _columns;


    private void OnEnable()
    {
        GameSetup.OnGameStart += ArrangeAndShuffleCards;
    }

    private void OnDisable()
    {
        GameSetup.OnGameStart -= ArrangeAndShuffleCards;
    }


    public void ArrangeAndShuffleCards(int rows, int columns)
    {
        _rows = rows;
        _columns = columns;
        
        // Clear any existing cards
        foreach (GameObject card in spawnedCards)
        {
            Destroy(card);
        }
        spawnedCards.Clear();

        GameManager.Instance.StartNewGame(TotalSlots / 2);

        // Generate a list of shuffled card IDs
        List<int> shuffledCardIDs = GenerateShuffledCardIDs();

        Vector3 cardSize = CalculateCardSize();

        // Arrange cards in a grid and assign IDs
        int index = 0;
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                // Stop if we reach the number of shuffled IDs after adjustment
                if (index >= shuffledCardIDs.Count) break;

                Vector3 position = CalculateCardPosition(i, j, cardSize);
                GameObject card = Instantiate(cardPrefab, position, Quaternion.identity, cardContainer);

                card.transform.localScale = cardSize * 0.1f;

                CardController cardController = card.GetComponent<CardController>();
                cardController.CardID = shuffledCardIDs[index];
                index++;

                spawnedCards.Add(card);
            }
        }
    }

    private List<int> GenerateShuffledCardIDs()
    {
        List<int> deck = new List<int>();
        for (int i = 0; i < TotalSlots / 2; i++)
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

    private Vector3 CalculateCardSize()
    {
        Renderer boardRenderer = boardPlane.GetComponent<Renderer>();
        float boardWidth = boardRenderer.bounds.size.x;
        float boardHeight = boardRenderer.bounds.size.z;

        float cardWidth = (boardWidth - (_columns - 1) * _spacing) / _columns;
        float cardHeight = (boardHeight - (_rows - 1) * _spacing) / _rows;

        return new Vector3(cardWidth, 1, cardHeight);
    }

    private Vector3 CalculateCardPosition(int row, int col, Vector3 cardSize)
    {
        Renderer boardRenderer = boardPlane.GetComponent<Renderer>();
        Vector3 boardCenter = boardRenderer.bounds.center;

        float startX = boardCenter.x - (boardRenderer.bounds.size.x / 2) + (cardSize.x / 2);
        float startZ = boardCenter.z + (boardRenderer.bounds.size.z / 2) - (cardSize.z / 2);

        float posX = startX + col * (cardSize.x + _spacing);
        float posZ = startZ - row * (cardSize.z + _spacing);

        return new Vector3(posX, boardCenter.y, posZ);
    }
}
