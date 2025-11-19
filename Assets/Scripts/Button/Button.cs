using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Transform baseTopPoint;
    public Transform baseBottomPoint;
    public float pressSpeedUp = 2f;
    public bool isPressed = false;

    //private int objectsOnButton = 0;
    private Rigidbody2D rb;
    private float minSpeed = 2f;

    void Start()
    {
        transform.position = baseTopPoint.position;
    }

    void Update()
    {
        isPressed = false;
        rb = null;
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, GetComponent<BoxCollider2D>().size, 0f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Cube") || hit.CompareTag("Player"))
            {
                isPressed = true;
                rb = hit.attachedRigidbody;
                break; // берём первый объект
            }
        }

        if (isPressed)
        {
            float pressSpeedDown = rb != null ? Mathf.Max(Mathf.Abs(rb.velocity.y), minSpeed) : minSpeed;
            if (transform.position.y > baseBottomPoint.position.y)
                MoveTowards(baseBottomPoint.position, pressSpeedDown);
        }
        else
            MoveTowards(baseTopPoint.position, pressSpeedUp);
    }
    //if (isPressed)
    //{
    //    float pressSpeedDown = rb != null ? Mathf.Max(Mathf.Abs(rb.velocity.y), minSpeed) : minSpeed;
    //    if (transform.position.y > baseBottomPoint.position.y)
    //        MoveTowards(baseBottomPoint.position, pressSpeedDown);
    //}
    //else
    //    MoveTowards(baseTopPoint.position, pressSpeedUp);


    void MoveTowards(Vector3 target, float speed)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
    }
}
//    private void OnTriggerStay2D(Collider2D col)
//    {
//        if (col.CompareTag("Cube") || col.CompareTag("Player"))
//        {
//            //objectsOnButton++;
//            isPressed = true;
//            if (rb ==  null)
//                rb = col.GetComponent<Rigidbody2D>();
//        }
//    }

//    private void OnTriggerExit2D(Collider2D col)
//    {
//        if (col.CompareTag("Cube") || col.CompareTag("Player"))
//        {
//            //objectsOnButton--;

//            //if (objectsOnButton <= 0)
//            //{
//            //    objectsOnButton = 0;
//            isPressed = false;
//            rb = null;
//            //}
//        }
//    }
//}
