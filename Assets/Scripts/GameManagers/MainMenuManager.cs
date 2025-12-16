using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioSource ñlickSound;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) LoadLevel(1);
        if (Input.GetKeyDown(KeyCode.S)) LoadLevel(2);
        if (Input.GetKeyDown(KeyCode.Q)) QuitGame();
    }

    private void Start()
    {
        ñlickSound.Stop();        
        Application.targetFrameRate = PlayerPrefs.GetInt("SavedFPS", 60);
    }

    public void LoadLevel(int levelIndex)
    {
        ñlickSound.Play();
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(levelIndex);
    }

    public void QuitGame()
    {
        ñlickSound.Play();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}