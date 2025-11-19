using UnityEngine;

public class Cube : MonoBehaviour
{
    public bool isPickable = true;
    public bool isPickedUp = false;
    public Transform holder;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool wasTrigger; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void PickUp(Transform newHolder)
    {
        if (!isPickable || isPickedUp) return;

        isPickedUp = true;
        holder = newHolder;

        // Отключаем физику
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

        // Делаем кубик дочерним объектом
        transform.SetParent(holder);
        transform.localPosition = Vector3.zero + Vector3.up * 0.5f; // Немного выше руки
    }

    public void Drop()
    {
        if (!isPickedUp) return;

        isPickedUp = false;

        // Включаем физику
        if (rb != null)
            rb.isKinematic = false;

        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false;
        }

        // Убираем из дочерних объектов
        transform.SetParent(null);
        holder = null;
    }
}