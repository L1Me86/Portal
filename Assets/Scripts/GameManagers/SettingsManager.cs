using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private AudioSource ñlickSound;
    [SerializeField] private TMP_Dropdown fpsDropdown;
    private List<int> FPSValues = new List<int> { 30, 60, 120 };


    void Start()
    {
        ñlickSound.Stop();
        fpsDropdown.value = FPSValues.IndexOf(Application.targetFrameRate);
        Debug.Log($"Ôàéëû ñîõğàíåíèé çäåñü: {Application.persistentDataPath}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) ToMainMenu();
    }

    public void ChangeFPS()
    {
        ñlickSound.Play();
        int targetFPS = FPSValues[fpsDropdown.value];
        Application.targetFrameRate = FPSValues[fpsDropdown.value];
        PlayerPrefs.SetInt("SavedFPS", targetFPS);
        PlayerPrefs.Save();
        Debug.Log($"Óñòàíîâëåí FPS: {Application.targetFrameRate}");
    }   

    public void ToMainMenu()
    {
        ñlickSound.Play();
        SceneManager.LoadScene(0);
    }
}
