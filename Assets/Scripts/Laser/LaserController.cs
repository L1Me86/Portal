using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public float laserLength = 50f;
    public LineRenderer lineRenderer;
    public float rotationSpeed = 30f;

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, laserLength);
        lineRenderer.SetPosition(0, transform.position);
        RaycastHit2D currentHit = hit;

        while (currentHit.collider != null && currentHit.collider.CompareTag("Bullet"))
        {
            float remainingDistance = laserLength - Vector2.Distance(transform.position, currentHit.point);
            currentHit = Physics2D.Raycast(currentHit.point + (Vector2)transform.right * 0.01f, transform.right, remainingDistance);
        }

        if (currentHit.collider != null)
        {
            lineRenderer.SetPosition(1, currentHit.point);
            if (currentHit.collider.CompareTag("Player")) FindObjectOfType<GameManager>().EndGame();
            if (!currentHit.collider.CompareTag("LaserReceiver")) MakeAction();
        }
        else lineRenderer.SetPosition(1, transform.position + transform.right * laserLength);
    }

    public void MakeAction()
    {
        transform.parent.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
