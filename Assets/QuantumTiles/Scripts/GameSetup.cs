using UnityEngine;
using UnityEngine.Events;

public class GameSetup : MonoBehaviour
{
    [Header("Board Setup")]
    [SerializeField] private Vector2 _maxMinArray = new(2, 32);

    public static UnityAction<int, int> OnGameStart;

    private void Start()
    {
        // Initialize the UI with min/max values
        UIManager.Instance.InitializeSetupUI(_maxMinArray);

        // Subscribe to the Start button click event from UIManager
        UIManager.Instance.OnStartButtonClicked += StartGame;
    }

    private void StartGame(int rows, int columns)
    {
        // Trigger the game start event with slider values
        OnGameStart?.Invoke(rows, columns);
    }

    private void OnDisable()
    {
        // Unsubscribe from the UIManager event
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnStartButtonClicked -= StartGame;
        }
    }
}
