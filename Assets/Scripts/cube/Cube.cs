using UnityEngine;

public class Cube : MonoBehaviour
{
    public bool isPickable = true;
    public bool isPickedUp = false;
    public Transform holder;
    public PlayerMovement player;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool wasTrigger;
    private Vector2 addedVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.gravityScale = player.gravityScale;
    }

    public void PickUp(Transform newHolder)
    {
        if (!isPickable || isPickedUp) return;

        isPickedUp = true;
        holder = newHolder;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (col != null)
        {
            wasTrigger = col.isTrigger;
            col.isTrigger = true;
        }

        transform.SetParent(holder);
        transform.localPosition = Vector3.zero + Vector3.up * 0.5f;
    }

    public void Drop()
    {
        if (!isPickedUp) return;

        isPickedUp = false;

        if (rb != null)
            rb.isKinematic = false;

        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false;
        }

        transform.SetParent(null);
        holder = null;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            col.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            col.collider.transform.SetParent(null);
        }
    }
}