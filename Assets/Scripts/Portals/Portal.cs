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
    public static HashSet<GameObject> activeHorizontalTriggers = new HashSet<GameObject>();
    public enum Side
    {
        Left,
        Right,
        Bottom,
        Top,
        Default
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
        if (this.linkedPortal != null)
        {
            range = this.linkedPortal.transform.position - this.transform.position;
            if (other.CompareTag("Player"))
            {
                GhostMovement.calc[0] = this.side;
                GhostMovement.calc[1] = this.linkedPortal.side;
                if ((this.side == Side.Left && this.linkedPortal.side == Side.Right) || (this.side == Side.Right && this.linkedPortal.side == Side.Left) || (this.side == Side.Top && this.linkedPortal.side == Side.Bottom) || (this.side == Side.Bottom && this.linkedPortal.side == Side.Top))
                {
                    GhostMovement.offset = range;
                    GhostMovement.calc[0] = Side.Default;
                }
            }
            if (other.CompareTag("PortalTrigger"))
            {
                if ((other.gameObject.name == "PortalTriggerLeft" && this.side == Side.Right) || (other.gameObject.name == "PortalTriggerRight" && this.side == Side.Left) || (other.gameObject.name == "PortalTriggerBott" && this.side == Side.Top) || (other.gameObject.name == "PortalTriggerTop" && this.side == Side.Bottom))
                {
                    other.GetComponentInParent<CapsuleCollider2D>().gameObject.transform.position += range;
                    Debug.Log($"Portal at {linkedPortal.transform.position} | Teleported at: {other.transform.position}");
                }
            }
            if (!string.IsNullOrEmpty(other.tag) && other.tag.StartsWith("PortalWallTrigger"))
            {
                if (other.tag.Length > 17 && other.tag[17] == 'V')
                    activeVerticalTriggers.Add(other.gameObject);
                else
                {
                    activeHorizontalTriggers.Add(other.gameObject);
                    Debug.Log(other.tag);
                }


                if ((activeVerticalTriggers.Count == 2) || (activeHorizontalTriggers.Count == 2))
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
                        case Side.Top:
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (GhostMovement.calc[0] != Side.Default && other.name == "RealPlayer")
        { 
            Vector3 range = this.linkedPortal.transform.position - this.transform.position;
            if (this.side == Side.Right)
            {
                if (this.linkedPortal.side == Side.Top)
                {
                    float offset = (this.transform.position.x - other.transform.position.x);

                    Vector3 adjustedOffset = new Vector3(range.x + offset, range.y + offset);

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == Side.Bottom)
                {
                    float offset = (this.transform.position.x - other.transform.position.x);

                    Vector3 adjustedOffset = new Vector3(range.x + offset, range.y - offset);

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == Side.Right)
                {
                    float offset = this.transform.position.x - other.transform.position.x;
                    GhostMovement.offset = new Vector3(range.x + 2 * offset, range.y);
                }
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && this.linkedPortal != null)
        {
            GhostMovement.offset = Vector3.up * 25;
            GhostMovement.calc[0] = Side.Default;
        }
        if (other.tag.Length > 17 && other.tag.Substring(0, 17) == "PortalWallTrigger")
        {
            if (other.tag.Substring(17, 1) == "V")
                activeVerticalTriggers.Remove(other.gameObject);
            else
                activeHorizontalTriggers.Remove(other.gameObject);

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
                    case Side.Top:
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