using UnityEngine;
using UnityEngine.Events;

public class GameSetup : MonoBehaviour
{
    [Header("Board Setup")]
    [SerializeField] private Vector2 _maxMinArray = new(2, 32);

    public static UnityAction<Vector2> OnGameStart;

    private void Start()
    {
        UIManager.Instance.ShowInitialMenu();

        // Initialize the UI with min/max values
        UIManager.Instance.SetupUI(_maxMinArray);

        // Subscribe to the Start button click event from UIManager
        UIManager.Instance.OnStartButtonClicked += StartNewGame;
    }

    private void StartNewGame(Vector2 rowsColumns)
    {
        // Trigger the game start event with slider values
        OnGameStart?.Invoke(rowsColumns);
    }

    private void OnDisable()
    {
        // Unsubscribe from the UIManager event
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnStartButtonClicked -= StartNewGame;
        }
    }
}
