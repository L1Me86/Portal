using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishButton : MonoBehaviour
{
    public Transform baseTopPoint;
    public Transform baseBottomPoint;
    public float pressSpeedUp = 2f;
    private float minSpeed = 2f;

    private bool isPressed = false;

    void Start()
    {
        transform.position = baseTopPoint.position;
    }

    void Update()
    {
        isPressed = false;
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, GetComponent<BoxCollider2D>().size, 0f);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                isPressed = true;

                // »грок нажал кнопку Ч вызываем меню паузы / конец игры
                GameManager.Instance.PauseGame(); // или свой метод дл€ конца игры

                break;
            }
        }

        Vector3 target = isPressed ? baseBottomPoint.position : baseTopPoint.position;
        float speed = isPressed ? minSpeed : pressSpeedUp;
        MoveTowards(target, speed);
    }

    void MoveTowards(Vector3 target, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}
