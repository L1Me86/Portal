using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusicController : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;

    void Start()
    {
        musicSource.Play();
        soundSource.Stop();
    }

    public void PlaySound()
    {
        soundSource.Play();
    }
}