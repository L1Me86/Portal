using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public float minY = 10f;
    public float maxY = 30f;
    public GameManager gameManager;

    void Update()
    {
        if (transform.position.y < minY || transform.position.y > maxY)
        {
            gameManager.EndGame();
        }
    }
}
