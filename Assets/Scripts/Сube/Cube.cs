using UnityEngine;

public class Cube : MonoBehaviour
{
    public bool isPickable = true;
    public bool isPickedUp = false;
    public Transform holder;
    public PlayerMovement player;
    public Transform spawn;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool wasTrigger;
    private Collider2D[] playerCols;

    [Header("Tuning")]
    public float pullSpeed = 50f;
    public float skinWidth = 0.02f;
    public Vector2 holdLocalOffset = Vector2.zero;
    public float maxCatchDistance = 2.5f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        playerCols = player.transform.parent.GetComponentsInChildren<Collider2D>();
        GetComponent<Collider2D>().gameObject.transform.position = spawn.position;
    }


    void FixedUpdate()
    {
        if (!isPickedUp || holder == null) return;

        Vector2 desiredPos = (Vector2)holder.TransformPoint(holdLocalOffset);

        float distToHolder = Vector2.Distance(rb.position, desiredPos);
        if (distToHolder > maxCatchDistance)
        {
            Drop();
            return;
        }

        Vector2 moveVec = desiredPos - rb.position;
        float moveDist = moveVec.magnitude;

        Vector2 nextTargetPos;

        if (moveDist < 0.0001f)
        {
            rb.velocity = Vector2.zero;
            nextTargetPos = rb.position;
        }
        else
        {
            Vector2 dir = moveVec / moveDist;

            RaycastHit2D[] hits = new RaycastHit2D[8];
            int hitCount = rb.Cast(dir, hits, moveDist + skinWidth);

            if (hitCount > 0)
            {
                float minDist = float.MaxValue;
                for (int i = 0; i < hitCount; i++)
                {
                    bool isPlayerCollider = false;
                    foreach (var pcol in playerCols)
                    {
                        if (hits[i].collider == pcol)
                        {
                            isPlayerCollider = true;
                            break;
                        }
                    }
                    if (isPlayerCollider) continue;

                    if (hits[i].distance < minDist) minDist = hits[i].distance;
                }
                if (minDist == float.MaxValue)
                {
                    nextTargetPos = desiredPos;
                }
                else
                {
                    float allowed = Mathf.Max(0f, minDist - skinWidth);
                    nextTargetPos = rb.position + dir * allowed;
                }
            }
            else
            {
                nextTargetPos = desiredPos;
            }

            Vector2 moveTowards = Vector2.MoveTowards(rb.position, nextTargetPos, pullSpeed * Time.fixedDeltaTime);

            Vector2 neededVelocity = (moveTowards - rb.position) / Time.fixedDeltaTime;
            rb.velocity = neededVelocity;

            float holderAngle = holder.eulerAngles.z;
            rb.MoveRotation(holderAngle);
        }
    }

    public void PickUp(Transform newHolder)
    {
        if (!isPickable || isPickedUp) return;

        isPickedUp = true;
        holder = newHolder;

        rb.isKinematic = false;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        foreach (var pcol in playerCols)
            Physics2D.IgnoreCollision(pcol, col, true);

    }

    public void Drop()
    {
        if (!isPickedUp) return;

        isPickedUp = false;
        holder = null;

        rb.isKinematic = false;
        rb.gravityScale = 9.8f;
        rb.velocity = Vector2.zero;

        foreach (var pcol in playerCols)
            Physics2D.IgnoreCollision(pcol, col, false);

    }

}