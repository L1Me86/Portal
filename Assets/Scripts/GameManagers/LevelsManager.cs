using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsManager : MonoBehaviour
{
    [SerializeField] private AudioSource ñlickSound;

    private void Start()
    {
        ñlickSound.Stop();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) ToMainMenu();
    }

    public void LoadLevel(int levelIndex)
    {
        ñlickSound.Play();
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(levelIndex);
    }

    public void ToMainMenu()
    {
        ñlickSound.Play();
        SceneManager.LoadScene(0);
    }
}
