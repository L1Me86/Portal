using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
    // Убираем дублирование - оставляем только одно поле
    [SerializeField] private MovingPlatform _connectedPlatform;

    void Start()
    {
        // Можно добавить проверку, что тег правильный
        if (!gameObject.CompareTag("LaserReceiver"))
        {
            Debug.LogWarning("LaserReceiver должен иметь тег 'LaserReceiver'", gameObject);
        }
    }

    // Public property для доступа
    public MovingPlatform connectedPlatform
    {
        get { return _connectedPlatform; }
        set { _connectedPlatform = value; }
    }
}