using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Transform baseTopPoint;
    public Transform baseBottomPoint;
    public float pressSpeedUp = 2f;
    public bool isPressed = false;

    private int objectsOnButton = 0;
    private Rigidbody2D rb;

    void Start()
    {
        transform.position = baseTopPoint.position;
    }

    void Update()
    {
        if (isPressed)
        {
            float pressSpeedDown = Mathf.Abs(rb.velocity.y);
            MoveTowards(baseBottomPoint.position, pressSpeedDown);
        }
        else
            MoveTowards(baseTopPoint.position, pressSpeedUp);
    }

    void MoveTowards(Vector3 target, float speed)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Cube"))
        {
            objectsOnButton++;
            isPressed = true;
            rb = col.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Cube"))
        {
            objectsOnButton--;

            if (objectsOnButton <= 0)
            {
                objectsOnButton = 0;
                isPressed = false;
                rb = null;
            }
        }
    }
}
