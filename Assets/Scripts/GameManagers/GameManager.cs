using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioSource FinishLevelSound;
    [SerializeField] private AudioSource ButtonSound;
    [SerializeField] private AudioSource DeathSound;
    public static bool gameIsFinished = false;
    public static bool gameIsPaused = false;
    public static bool gameIsEnded = false;
    public static GameManager Instance;
    public GameObject finishPanel;
    public GameObject pausePanel;
    public GameObject endPanel;

    void Start()
    {
        Time.timeScale = 1f;
        FinishLevelSound.Stop();
        ButtonSound.Stop();
        DeathSound.Stop();
        gameIsFinished = false;
        gameIsPaused = false;
        gameIsEnded = false;
        if (finishPanel != null) finishPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) RestartGame();
        if (Input.GetKeyDown(KeyCode.N) && gameIsFinished && SceneManager.GetActiveScene().buildIndex != 7) NextLevel();
        if (Input.GetKeyDown(KeyCode.L) && (gameIsPaused || gameIsEnded || gameIsFinished)) ToMainMenu();
        if (Input.GetKeyDown(KeyCode.Escape) && !gameIsEnded && !gameIsFinished)
        if (gameIsPaused) ResumeGame();
        else PauseGame();
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ResumeGame()
    {   
        ButtonSound.Play();
        gameIsPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        Debug.Log("Игра продолжается");
    }

    public void PauseGame()
    {
        ButtonSound.Play();
        gameIsPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        Debug.Log("Игра на паузе");
    }

    public void RestartGame()
    {
        ButtonSound.Play();
        Time.timeScale = 1f;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players) if (player != null) player.transform.parent = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        ButtonSound.Play();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void ToMainMenu()
    {
        ButtonSound.Play();
        SceneManager.LoadScene(1);
    }

    public void EndGame()
    {
        if (!gameIsEnded) DeathSound.Play();
        gameIsEnded = true;
        Time.timeScale = 0f;
        if (endPanel != null) endPanel.SetActive(true);
        Debug.Log("Уровень не пройден");
    }

    public void FinishGame()
    {
        if (!gameIsFinished) FinishLevelSound.Play();
        gameIsFinished = true;
        Time.timeScale = 0f;
        if (finishPanel != null) finishPanel.SetActive(true);
        Debug.Log("Уровень пройден");
    }
}
