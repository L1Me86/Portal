using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Portal;

public class LaserClone : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float laserLength = 50f;
    private Vector3 initialPosition = new Vector3(1000, 1000, 0); // Позиция за сценой

    void Start()
    {
        // Сохраняем начальную позицию (за сценой)
        initialPosition = transform.position;
        lineRenderer.enabled = false;
    }

    public void ActivateClone(Portal exitPortal, Vector3 incomingDirection)
    {
        // Активируем видимость
        lineRenderer.enabled = true;

        // Вычисляем позицию клона у выхода из портала
        Vector3 exitOffset = CalculateExitOffset(exitPortal.side);
        transform.position = exitPortal.transform.position + exitOffset;

        // Вычисляем направление (перпендикулярно выходу из портала)
        Vector3 exitDirection = CalculateExitDirection(exitPortal.side);

        // Устанавливаем вращение для правильного направления
        float angle = Mathf.Atan2(exitDirection.y, exitDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Обновляем луч
        UpdateLaser();
    }

    public void HideClone()
    {
        lineRenderer.enabled = false;
        transform.position = initialPosition; // Возвращаем за сцену
    }

    void UpdateLaser()
    {
        if (!lineRenderer.enabled) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, laserLength);
        lineRenderer.SetPosition(0, transform.position);
        RaycastHit2D currentHit = hit;

        while (currentHit.collider != null && currentHit.collider.CompareTag("Bullet"))
        {
            float remainingDistance = laserLength - Vector2.Distance(transform.position, currentHit.point);
            currentHit = Physics2D.Raycast(currentHit.point + (Vector2)transform.right * 0.05f, transform.right, remainingDistance);
        }

        if (currentHit.collider != null)
        {
            lineRenderer.SetPosition(1, currentHit.point);
            if (currentHit.collider.CompareTag("LaserReceiver")) MakeAction();
            if (currentHit.collider.CompareTag("Player")) FindObjectOfType<GameManager>().EndGame();
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + transform.right * laserLength);
        }
    }

    private Vector3 CalculateExitOffset(Side portalSide)
    {
        float exitDistance = 0.07f;

        switch (portalSide)
        {
            case Side.Right:
                return Vector3.left * exitDistance;
            case Side.Left:
                return Vector3.right * exitDistance;
            case Side.Top:
                return Vector3.down * exitDistance;
            case Side.Bottom:
                return Vector3.up * exitDistance;
            default:
                return Vector3.zero;
        }
    }

    private Vector3 CalculateExitDirection(Side portalSide)
    {
        // Перпендикулярное направление относительно стороны портала
        switch (portalSide)
        {
            case Side.Right:
                return Vector3.left; // Направлен влево из левого портала
            case Side.Left:
                return Vector3.right; // Направлен вправо из правого портала
            case Side.Top:
                return Vector3.down; // Направлен вниз из нижнего портала
            case Side.Bottom:
                return Vector3.up; // Направлен вверх из верхнего портала
            default:
                return Vector3.right;
        }
    }

    public void MakeAction()
    {
        FindObjectOfType<MovingPlatform>().MovePlatformFixed();
    }

    // Обновляем каждый кадр, когда активен
    void Update()
    {
        if (lineRenderer.enabled)
        {
            UpdateLaser();
        }
    }
}