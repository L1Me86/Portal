using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioSource ñlickSound;

    private void Start()
    {
        ñlickSound.Stop();        
        Application.targetFrameRate = PlayerPrefs.GetInt("SavedFPS", 60);
    }

    public void LoadLevel(int levelIndex)
    {
        ñlickSound.Play();
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(levelIndex);
        else Debug.LogError($"Óðîâåíü ñ èíäåêñîì {levelIndex} íå íàéäåí!");
    }

    public void QuitGame()
    {
        ñlickSound.Play();
        Debug.Log("Èãðà çàâåðøåíà");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}