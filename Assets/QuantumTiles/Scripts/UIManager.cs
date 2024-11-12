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
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    [Header("Toggles")]
    [SerializeField] private Toggle _audioToggle;
    [SerializeField] private Toggle _persistentSaveToggle;

    [Header("Panels")]
    [SerializeField] private GameObject initialMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject highScorePanel;

    public static UIManager Instance { get; private set; }
    public Action<Vector2> OnStartButtonClicked;
    public Action<bool> OnAudioToggle;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        GameManager.OnGameOver += ShowGameOverPanel;
        GameManager.OnStatsUpdate += UpdateScoreUI;
    }

    private void OnDestroy()
    {
        GameManager.OnGameOver -= ShowGameOverPanel;
        GameManager.OnStatsUpdate -= UpdateScoreUI;
    }

    public void SetupUI(Vector2 maxMinArray)
    {
        //Slider Values Setup
        rowsSlider.minValue = maxMinArray.x;
        rowsSlider.maxValue = maxMinArray.y;
        columnsSlider.minValue = maxMinArray.x;
        columnsSlider.maxValue = maxMinArray.y;
        rowsValue.text = maxMinArray.x.ToString();
        columnsValue.text = maxMinArray.x.ToString();

        //Slider Setup
        rowsSlider.onValueChanged.AddListener((value) => rowsValue.text = rowsSlider.value.ToString());
        columnsSlider.onValueChanged.AddListener((value) => columnsValue.text = columnsSlider.value.ToString());

        // Buttons Setup
        restartButton.onClick.AddListener(() => RestartGame());
        closeButton.onClick.AddListener(() => HideOptionsPanel());
        startGameplayButton.onClick.AddListener(() => StartGame());
        optionsButton.onClick.AddListener(() => ShowOptionsPanel());


        quitButton.onClick.AddListener(delegate { RestartGame(); GameManager.Instance.ClearFlipedCards();});
        saveButton.onClick.AddListener(delegate { GameManager.Instance.SaveGame(); HideOptionsPanel(); });
        loadButton.onClick.AddListener(delegate { GameManager.Instance.LoadGame(); HideOptionsPanel(); });
        continueButton.onClick.AddListener(delegate { GameManager.Instance.LoadGame(); HideInitialMenu(); });

        // Toggle Setup
        _persistentSaveToggle.onValueChanged.AddListener((value) => { GameManager.Instance.PersistentSave = value;});
        _audioToggle.onValueChanged.AddListener((value) => { OnAudioToggle?.Invoke(value); });
    }

    private void StartGame()
    {
        OnStartButtonClicked?.Invoke(new Vector2(rowsSlider.value, columnsSlider.value));
        initialMenuPanel.SetActive(false);
    }

    private void RestartGame()
    {
        HideOptionsPanel();
        HideGameOverPanel();
        ShowInitialMenu();
    }

    public void UpdateScoreUI(GameState stats)
    {
        comboText.text = "Combo " + stats.Combo;
        attemptsText.text = "Attempts " + stats.Attempts;
        scoreText.text = "Score: " + stats.Score;
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    public void HideGameOverPanel()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowOptionsPanel()
    {
        optionsPanel.SetActive(true);
    }

    public void HideOptionsPanel()
    {
        optionsPanel.SetActive(false);
    }

    public void ShowInitialMenu()
    {
        initialMenuPanel.SetActive(true);
        HideGameOverPanel();
    }

    public void HideInitialMenu()
    {
        initialMenuPanel.SetActive(false);
    }
}
