using System.Collections.Generic;
using UnityEngine;

public class BoardLayoutManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 2;
    public int columns = 2;
    public float spacing = 0.1f;

    [Header("Card Settings")]
    public GameObject cardPrefab;
    public Transform cardContainer; // Reference to the container holding the cards
    public List<int> cardIDs; // List of unique card IDs to match

    private List<GameObject> spawnedCards = new List<GameObject>();

    private void Start()
    {
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
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Instantiate the card at the calculated position
                Vector3 position = CalculateCardPosition(i, j, cardSize);
                GameObject card = Instantiate(cardPrefab, position, Quaternion.identity, cardContainer);

                // Set card size
                card.transform.localScale = new Vector3(cardSize.x, 1, cardSize.y);

                // Set the card ID and store it
                CardController cardController = card.GetComponent<CardController>();
                cardController.CardID = shuffledCardIDs[i * columns + j];

                // Add card to the spawned cards list
                spawnedCards.Add(card);
            }
        }
    }

    private List<int> GenerateShuffledCardIDs()
    {
        // Duplicate each card ID to create pairs
        List<int> deck = new List<int>();
        foreach (int id in cardIDs)
        {
            deck.Add(id);
            deck.Add(id);
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
        float cardWidth = (containerWidth - (columns - 1) * spacing) / columns;
        float cardHeight = (containerHeight - (rows - 1) * spacing) / rows;

        return new Vector2(.1f, .1f);
    }

    private Vector3 CalculateCardPosition(int row, int col, Vector2 cardSize)
    {
        // Calculate the starting position to center the grid
        float startX = -((columns - 1) * (cardSize.x + spacing)) / 2;
        float startY = ((rows - 1) * (cardSize.y + spacing)) / 2;

        // Calculate the exact position for each card
        float posX = startX + col * (cardSize.x + spacing);
        float posY = startY - row * (cardSize.y + spacing);

        return new Vector3(posX, 0, posY);
    }
}
