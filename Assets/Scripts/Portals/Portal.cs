using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;
    public bool isBlue = true;
    public static HashSet<GameObject> activeVerticalTriggers = new HashSet<GameObject>();
    public enum Side
    {
        Left,
        Right,
        Bottom,
        Up
    }
    public Side side;

    private Vector3 range;

    void Start()
    {
        CreateChildrenIfMissing();

        Transform blueChild = transform.Find("PortalBlue");
        Transform orangeChild = transform.Find("PortalOrange");

        if (blueChild != null) blueChild.gameObject.SetActive(isBlue);
        if (orangeChild != null) orangeChild.gameObject.SetActive(!isBlue);

    }

    void CreateChildrenIfMissing()
    {
        if (transform.Find("PortalBlue") == null)
        {
            GameObject blue = new GameObject("PortalBlue");
            blue.transform.SetParent(transform, false);
            blue.transform.localPosition = Vector3.zero;
            blue.AddComponent<SpriteRenderer>().color = new Color(0.663f, 0.588f, 1f);
        }

        if (transform.Find("PortalOrange") == null)
        {
            GameObject orange = new GameObject("PortalOrange");
            orange.transform.SetParent(transform, false);
            orange.transform.localPosition = Vector3.zero;
            orange.AddComponent<SpriteRenderer>().color = new Color(1f, 0.68f, 0.36f);
        }
    }

    public void LinkTo(Portal otherPortal)
    {
        linkedPortal = otherPortal;
        otherPortal.linkedPortal = this;
    }

    public void Unlink()
    {
        if (linkedPortal != null)
        {
            linkedPortal.linkedPortal = null;
            linkedPortal = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (linkedPortal != null)
        {
            range = linkedPortal.transform.position - this.transform.position;
            if (other.CompareTag("Player"))
            {
                GhostMovement.offset = range;
            }
            if (other.CompareTag("PortalTrigger"))
            {
                other.GetComponentInParent<CapsuleCollider2D>().gameObject.transform.position += Vector3.right * 0.11f + range;
                Debug.Log($"Portal at {linkedPortal.transform.position} | Teleported at: {other.transform.position}");
            }
            if (other.CompareTag("PortalWallTriggerVertical"))
            {
                activeVerticalTriggers.Add(other.gameObject);

                if (activeVerticalTriggers.Count == 2)
                {
                    string walltag;
                    switch (this.side)
                    {
                        case Side.Right:
                            walltag = "PortalSurfaceRight";
                            break;
                        case Side.Left:
                            walltag = "PortalSurfaceLeft";
                            break;
                        case Side.Bottom:
                            walltag = "PortalSurfaceBott";
                            break;
                        case Side.Up:
                            walltag = "PortalSurfaceUp";
                            break;
                        default:
                            walltag = "PortalSurface";
                            Debug.LogError("Wrong Wall Tag");
                            break;
                    }
                    GameObject[] walls = GameObject.FindGameObjectsWithTag(walltag);

                    foreach (GameObject wall in walls)
                    {
                        Collider2D[] wallColliders = wall.GetComponentsInChildren<Collider2D>();
                        Collider2D playerCollider = other.GetComponentInParent<Collider2D>();
                        foreach (Collider2D wallCol in wallColliders)
                        {
                            wallCol.enabled = false;
                            if (wallCol != null && playerCollider != null)
                            {
                                Physics2D.IgnoreCollision(playerCollider, wallCol, true);
                            }
                        }
                    }

                    Debug.Log("Wall phasing ENABLED");
                }
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && linkedPortal != null)
            GhostMovement.offset = Vector3.up * 25;
        if (other.CompareTag("PortalWallTriggerVertical"))
        {
            activeVerticalTriggers.Remove(other.gameObject);

            if (activeVerticalTriggers.Count < 2)
            {
                string walltag;
                switch (this.side)
                {
                    case Side.Right:
                        walltag = "PortalSurfaceRight";
                        break;
                    case Side.Left:
                        walltag = "PortalSurfaceLeft";
                        break;
                    case Side.Bottom:
                        walltag = "PortalSurfaceBott";
                        break;
                    case Side.Up:
                        walltag = "PortalSurfaceUp";
                        break;
                    default:
                        walltag = "PortalSurface";
                        Debug.LogError("Wrong Wall Tag");
                        break;
                }
                GameObject[] walls = GameObject.FindGameObjectsWithTag(walltag);

                foreach (GameObject wall in walls)
                {
                    Collider2D[] wallColliders = wall.GetComponentsInChildren<Collider2D>();
                    Collider2D playerCollider = other.GetComponentInParent<Collider2D>();
                    foreach (Collider2D wallCol in wallColliders)
                    {
                        wallCol.enabled = true;
                        if (wallCol != null && playerCollider != null)
                        {
                            Physics2D.IgnoreCollision(playerCollider, wallCol, false);
                        }
                    }
                }

                Debug.Log("Wall phasing DISABLED");
            }
        }
    }




}