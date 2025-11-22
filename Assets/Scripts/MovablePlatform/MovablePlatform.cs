using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public Vector2 platformVelocity;
    public Button button;

    private Vector3 targetPoint;
    private Vector3 lastPosition;

    private void Start()
    {
        targetPoint = pointB.position;
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (button.isPressed) {
            MovePlatform();
            CalculateVelocity();
        } 
    }

    void MovePlatform()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPoint) < 0.05f)
        {
            targetPoint = targetPoint == pointA.position ? pointB.position : pointA.position;
        }
    }

    void CalculateVelocity()
    {
        platformVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }
}