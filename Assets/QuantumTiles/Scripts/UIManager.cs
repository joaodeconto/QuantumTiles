using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    [Header("In-Game UI References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text attemptsText;

    [Header("Setup UI References")]
    [SerializeField] private Slider rowsSlider;
    [SerializeField] private Slider columnsSlider;
    [SerializeField] private TMP_Text rowsValue;
    [SerializeField] private TMP_Text columnsValue;

    [Header("Buttons")]
    [SerializeField] private Button startGameplayButton;
    [SerializeField] private Button highScoreButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button restartButton;

    [Header("Panels")]
    [SerializeField] private GameObject initialMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject highScorePanel;

    public static UIManager Instance { get; private set; }
    public Action<int, int> OnStartButtonClicked;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += ShowGameOverPanel;
        GameManager.OnStatsUpdate += UpdateScoreUI;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= ShowGameOverPanel;
        GameManager.OnStatsUpdate -= UpdateScoreUI;
    }


    public void InitializeSetupUI(Vector2 maxMinArray)
    {
        rowsSlider.minValue = maxMinArray.x;
        rowsSlider.maxValue = maxMinArray.y;
        columnsSlider.minValue = maxMinArray.x;
        columnsSlider.maxValue = maxMinArray.y;

        rowsSlider.onValueChanged.AddListener((value) => rowsValue.text = rowsSlider.value.ToString());
        columnsSlider.onValueChanged.AddListener((value) => columnsValue.text = columnsSlider.value.ToString());

        rowsValue.text = maxMinArray.x.ToString();
        columnsValue.text = maxMinArray.x.ToString();

        startGameplayButton.onClick.AddListener(() => StartGame());
        restartButton.onClick.AddListener(() => RestartGame());

    }

    private void StartGame()
    {
        OnStartButtonClicked?.Invoke((int)rowsSlider.value, (int)columnsSlider.value);
        initialMenuPanel.SetActive(false);
    }

    private void RestartGame()
    {
        gameOverPanel.SetActive(false);
        initialMenuPanel.SetActive(true);
    }

    public void UpdateScoreUI(GameStats stats)
    {
        comboText.text = "Combo " + stats._comboCount;
        attemptsText.text = "Attempts " + stats._attemptsCount;
        scoreText.text = "Score: " + stats._score;
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    public void ShowInitialMenu()
    {
        initialMenuPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    public void HideInitialMenu()
    {
        initialMenuPanel.SetActive(false);
    }
}
