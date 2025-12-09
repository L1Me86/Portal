using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private AudioSource buttonSound;
    public Transform baseTopPoint;
    public Transform baseBottomPoint;
    public float pressSpeedUp = 2f;
    public bool isPressed = false;
    public bool isSoundPlaying = false;

    private Rigidbody2D rb;
    private float minSpeed = 2f;

    void Start()
    {
        buttonSound.Stop();
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
                break;
            }
        }

        if (isPressed)
        {
            if (!isSoundPlaying) buttonSound.Play();
            isSoundPlaying = true;
            float pressSpeedDown = rb != null ? Mathf.Max(Mathf.Abs(rb.velocity.y), minSpeed) : minSpeed;
            if (transform.position.y > baseBottomPoint.position.y)
                MoveTowards(baseBottomPoint.position, pressSpeedDown);
        }
        else
        {
            isSoundPlaying = false;
            MoveTowards(baseTopPoint.position, pressSpeedUp);
        }
    }

    void MoveTowards(Vector3 target, float speed)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
    }
}