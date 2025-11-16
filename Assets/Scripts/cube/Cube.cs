using UnityEngine;

public class Cube : MonoBehaviour
{
    public bool isPickable = true;
    public bool isPickedUp = false;
    public Transform holder;
    public PlayerMovement player;

    private Rigidbody2D rb;
    private Collider2D col;

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
            col.enabled = false;

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
            col.enabled = true;

        transform.SetParent(null);
        holder = null;
    }
}