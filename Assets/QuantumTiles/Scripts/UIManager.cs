using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text attemptsText;
    [SerializeField] private GameObject gameOverPanel;

    public static UIManager Instance { get; private set; }

    private void OnEnable()
    {
        GameManager.OnGameOver += ShowGameOverPanel;
        GameManager.OnStatsUpdate += UpdateScoreUI;
    }

    private void Start()
    {
        Instance = this;

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
}
