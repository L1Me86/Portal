using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Portal;

public class LaserClone : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float laserLength = 50f;
    private Vector3 initialPosition = new Vector3(1000, 1000, 0);

    void Start()
    {
        initialPosition = transform.position;
        lineRenderer.enabled = false;
    }

    public void ActivateClone(Portal exitPortal, Vector3 incomingDirection)
    {
        lineRenderer.enabled = true;
        Vector3 exitOffset = CalculateExitOffset(exitPortal.side);
        transform.position = exitPortal.transform.position + exitOffset;
        Vector3 exitDirection = CalculateExitDirection(exitPortal.side);
        float angle = Mathf.Atan2(exitDirection.y, exitDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        UpdateLaser();
    }

    public void HideClone()
    {
        lineRenderer.enabled = false;
        transform.position = initialPosition;
    }

    void UpdateLaser()
    {
        if (!lineRenderer.enabled) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, laserLength);
        RaycastHit2D currentHit = hit;
        lineRenderer.SetPosition(0, transform.position);

        while (currentHit.collider != null && currentHit.collider.CompareTag("Bullet"))
        {
            float remainingDistance = laserLength - Vector2.Distance(transform.position, currentHit.point);
            currentHit = Physics2D.Raycast(currentHit.point + (Vector2)transform.right * 0.05f, transform.right, remainingDistance);
        }

        if (currentHit.collider != null)
        {
            lineRenderer.SetPosition(1, currentHit.point);
            if (currentHit.collider.CompareTag("Player")) FindObjectOfType<GameManager>().EndGame();

            // Проверяем, есть ли компонент LaserReceiver и активируем связанную платформу
            LaserReceiver receiver = currentHit.collider.GetComponent<LaserReceiver>();
            if (receiver != null && receiver.connectedPlatform != null)
            {
                receiver.connectedPlatform.isActivated = true;
            }
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
        switch (portalSide)
        {
            case Side.Right:
                return Vector3.left;
            case Side.Left:
                return Vector3.right;
            case Side.Top:
                return Vector3.down;
            case Side.Bottom:
                return Vector3.up;
            default:
                return Vector3.right;
        }
    }

    void FixedUpdate()
    {
        if (lineRenderer.enabled)
        {
            UpdateLaser();
        }
    }
}