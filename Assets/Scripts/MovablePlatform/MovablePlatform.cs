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
    public bool stay;
    public bool isLaserReceiverOn1 = false;
    public bool isLaserReceiverOn2 = false;
    public Collider2D portalTriggerCol;

    private Vector3 targetPoint;
    private Vector3 lastPosition;
    private Cube cube;

    private HashSet<Rigidbody2D> riders = new HashSet<Rigidbody2D>();
    private Dictionary<Transform, Transform> originalParents = new Dictionary<Transform, Transform>();

    private void Start()
    {
        targetPoint = pointB.position;
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (stay)
        {
            if ((button != null && button.isPressed) || isLaserReceiverOn1 || isLaserReceiverOn2)
            {
                MovePlatformFixedStay(true);
            }
            else
            {
                MovePlatformFixedStay(false);
            }
        }
        else
        {
            if ((button != null && button.isPressed) || isLaserReceiverOn1 || isLaserReceiverOn2)
            {
                MovePlatformFixed();
            }
        }

        Vector3 displacement = transform.position - lastPosition;

        if (displacement != Vector3.zero)
        {
            foreach (var rb in riders)
            {
                if (rb == null) continue;
                rb.MovePosition(rb.position + (Vector2)displacement);
            }
        }

        lastPosition = transform.position;
    }

    public void MovePlatformFixed()
    {
        if (pointA == null || pointB == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint,
            speed * Time.fixedDeltaTime
        );

        if (Vector3.Distance(transform.position, targetPoint) < 0.05f)
        {
            targetPoint = targetPoint == pointA.position ? pointB.position : pointA.position;
        }
    }
    void MovePlatformFixedStay(bool toPoint)
    {
        if (toPoint)
        {
            if (Vector3.Distance(transform.position, targetPoint) < 0.01f)
            {
                if (portalTriggerCol != null)
                {
                    portalTriggerCol.enabled = true;
                }
                return;
            }

            if (portalTriggerCol != null)
            {
                GameObject[] activePortals = GameObject.FindGameObjectsWithTag("Portal");

                foreach (GameObject port in activePortals)
                {
                    if (port.GetComponent<Portal>().sitsOn == portalTriggerCol)
                    {
                        port.GetComponent<Portal>().linkedPortal.Unlink();
                        GameObject.Destroy(port);
                    }
                }
                portalTriggerCol.enabled = false;
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.fixedDeltaTime);
        }
        else
        {
            if (Vector3.Distance(transform.position, pointA.position) < 0.01f)
            {
                if (portalTriggerCol != null)
                {
                    portalTriggerCol.enabled = true;
                }
                return;
            }

            if (portalTriggerCol != null)
            {
                GameObject[] activePortals = GameObject.FindGameObjectsWithTag("Portal");

                foreach (GameObject port in activePortals)
                {
                    if (port.GetComponent<Portal>().sitsOn == portalTriggerCol)
                    {
                        port.GetComponent<Portal>().linkedPortal.Unlink();
                        GameObject.Destroy(port);
                    }
                }
                portalTriggerCol.enabled = false;
            }
            transform.position = Vector3.MoveTowards(transform.position, pointA.position, speed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;

        var colTransform = collision.transform;

        if (colTransform.CompareTag("Player"))
        {
            if (!originalParents.ContainsKey(colTransform))
                originalParents[colTransform] = colTransform.parent;

            colTransform.SetParent(transform, true);
            return;
        }

        var rb = collision.rigidbody;
        if (rb == null) return;

        if (rb.gameObject.CompareTag("Cube"))
        {
            var cube = rb.GetComponent<Cube>();
            if (cube == null || cube.isPickedUp) return;
            riders.Add(rb);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision == null) return;

        var colTransform = collision.transform;

        if (colTransform.CompareTag("Player"))
        {
            if (originalParents.TryGetValue(colTransform, out var origParent))
            {
                colTransform.SetParent(origParent, true);
                originalParents.Remove(colTransform);
            }
            else
            {
                colTransform.SetParent(null, true);
            }
            return;
        }

        var rb = collision.rigidbody;
        if (rb != null)
            riders.Remove(rb);
    }
}