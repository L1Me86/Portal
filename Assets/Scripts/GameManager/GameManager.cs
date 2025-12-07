using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public static bool gameIsEnded = false;
    public static GameManager Instance;
    public GameObject pausePanel;

    void Start()
    {
        Time.timeScale = 1f;
        gameIsPaused = false;
        gameIsEnded = false;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && gameIsPaused) QuitGame();
        if (Input.GetKeyDown(KeyCode.R) && gameIsPaused) RestartGame();
        if (Input.GetKeyDown(KeyCode.Escape) && !gameIsEnded)
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
        gameIsPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        Debug.Log("Игра продолжается");
    }

    public void PauseGame()
    {
        gameIsPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        Debug.Log("Игра на паузе");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void EndGame()
    {
        gameIsEnded = true;
        PauseGame();
    }
}
