using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
<<<<<<< Updated upstream:Assets/Scripts/GameManager/MainMenuManager.cs
=======
    [SerializeField] private AudioSource сlickSound;

    private void Start()
    {
        сlickSound.Stop();        
        Application.targetFrameRate = PlayerPrefs.GetInt("SavedFPS", 60);
    }

>>>>>>> Stashed changes:Assets/Scripts/GameManagers/MainMenuManager.cs
    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.LogError($"Уровень с индексом {levelIndex} не найден!");
        }
    }

    // Метод для выхода из игры
    public void QuitGame()
    {
        Debug.Log("Игра завершена");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}