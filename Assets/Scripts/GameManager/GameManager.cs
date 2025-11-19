using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static bool IsPaused = false;
    public static bool IsEnded = false;
    public GameObject pausePanel;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        IsPaused = false;
        IsEnded = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
        if (Input.GetKeyDown(KeyCode.R) && IsPaused) RestartGame();
        if (Input.GetKeyDown(KeyCode.Q) && IsPaused) QuitGame();
    }

    public void EndGame()
    {
        IsEnded = true;
        IsPaused = true;
        TogglePause();
    }

    public void TogglePause()
    {
        if (!IsEnded) IsPaused = !IsPaused;

        if (IsPaused)
        {
            Time.timeScale = 0f;
            if (pausePanel != null) pausePanel.SetActive(true);
            Debug.Log("Игра на паузе");
        }
        else
        {
            Time.timeScale = 1f;
            if (pausePanel != null) pausePanel.SetActive(false);
            Debug.Log("Игра продолжается");
        }
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
}