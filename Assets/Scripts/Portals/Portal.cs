using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;
    public bool isBlue = true;
    public static HashSet<GameObject> activeVerticalTriggers = new HashSet<GameObject>();
    public static HashSet<GameObject> activeHorizontalTriggers = new HashSet<GameObject>();
    public Collider2D sitsOn;
    private bool unlock = true;
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
        Debug.Log($"{name} -> linked to -> {otherPortal.name}");
    }

    public void Unlink()
    {
        if (linkedPortal != null)
        {
            if (linkedPortal.linkedPortal == this)
                linkedPortal.linkedPortal = null;

            Debug.Log($"{name} -> unlinked from -> {linkedPortal.name}");
            linkedPortal = null;
        }
    }

    private void ReenableWallCollisionsForPlayer(GameObject player, Collider2D[] wallCols)
    {
        if (player == null) return;

        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider == null) return;

        foreach (var w in wallCols)
        {
            if (w == null) continue;
            Physics2D.IgnoreCollision(playerCollider, w, false);
        }
    }

    private Collider2D[] GetWallCollidersBySide(Side side)
    {
        string walltag;
        switch (side)
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
                break;
        }

        List<Collider2D> cols = new List<Collider2D>();
        GameObject[] triggerChildren = GameObject.FindGameObjectsWithTag(walltag);
        HashSet<GameObject> uniqueWalls = new HashSet<GameObject>();

        foreach (GameObject trigger in triggerChildren)
        {
            GameObject wallParent = trigger.transform.parent?.gameObject;

            if (wallParent != null && uniqueWalls.Add(wallParent))
            {
                cols.AddRange(wallParent.GetComponentsInChildren<Collider2D>());
            }
        }
        return cols.ToArray();
    }


    private IEnumerator ReenableWallsAfterFixed(GameObject player, Collider2D[] wallCols)
    {
        yield return new WaitForFixedUpdate();

        ReenableWallCollisionsForPlayer(player, wallCols);
    }
    private IEnumerator ApplyVelocityAfterFixed(Rigidbody2D rb, Vector2 velocity)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        if (rb != null)
        {
            rb.velocity = velocity; Debug.Log($"Velocity after apply in coroutine: {rb.velocity}");
        }
    }




    private void TeleportPlayer(Collider2D triggerCollider)
    {
        if (triggerCollider == null || this.linkedPortal == null) return;

        GameObject playerObj = triggerCollider.transform.parent != null
                               ? triggerCollider.transform.parent.gameObject
                               : triggerCollider.GetComponentInParent<Collider2D>()?.gameObject;


        if (playerObj == null) return;


        Collider2D[] playerColliders = playerObj.GetComponentsInChildren<Collider2D>();
        if (playerColliders == null || playerColliders.Length == 0) return;

        Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();

        Collider2D[] destWalls = GetWallCollidersBySide(this.linkedPortal.side);

        foreach (var pcol in playerColliders)
        {
            if (pcol == null) continue;
            foreach (var w in destWalls)
            {
                if (w == null) continue;
                Physics2D.IgnoreCollision(pcol, w, true);
            }
        }

        var playerParentCapsule = triggerCollider.GetComponentInParent<CapsuleCollider2D>();

        Vector2 oldVelocity = rb.velocity;

        Vector3 newPos;

        switch (GhostMovement.calc[0])
        {
            case Side.Left:
                switch (GhostMovement.calc[1])
                {
                    case Side.Right:
                        newPos = this.linkedPortal.transform.position - new Vector3(triggerCollider.transform.localPosition.x, 0f, 0f);
                        break;
                    case Side.Top:
                        newPos = this.linkedPortal.transform.position;
                        break;
                    case Side.Bottom:
                        newPos = this.linkedPortal.transform.position;
                        break;
                    case Side.Left:
                        newPos = this.linkedPortal.transform.position + new Vector3(triggerCollider.transform.localPosition.x, 0f, 0f);
                        break;
                    default:
                        newPos = Vector3.zero; break;
                }
                break;
            case Side.Right:
                switch (GhostMovement.calc[1])
                {
                    case Side.Right:
                        newPos = this.linkedPortal.transform.position + new Vector3(triggerCollider.transform.localPosition.x, 0f, 0f);
                        break;
                    case Side.Top:
                        newPos = this.linkedPortal.transform.position;
                        break;
                    case Side.Bottom:
                        newPos = this.linkedPortal.transform.position;
                        break;
                    case Side.Left:
                        newPos = this.linkedPortal.transform.position - new Vector3(triggerCollider.transform.localPosition.x, 0f, 0f);
                        break;
                    default:
                        newPos = Vector3.zero; break;
                }
                break;
            case Side.Bottom:
                switch (GhostMovement.calc[1])
                {
                    case Side.Right:
                        newPos = this.linkedPortal.transform.position - new Vector3(playerObj.transform.Find("PortalTriggerRight").localPosition.x, 0f, 0f);
                        break;
                    case Side.Top:
                        newPos = this.linkedPortal.transform.position;
                        break;
                    case Side.Bottom:
                        newPos = this.linkedPortal.transform.position;
                        break;
                    case Side.Left:
                        newPos = this.linkedPortal.transform.position - new Vector3(playerObj.transform.Find("PortalTriggerLeft").localPosition.x, 0f, 0f);
                        break;
                    default:
                        newPos = Vector3.zero; break;
                }
                break;
            case Side.Top:
                switch (GhostMovement.calc[1])
                {
                    case Side.Right:
                        newPos = this.linkedPortal.transform.position - new Vector3(playerObj.transform.Find("PortalTriggerRight").localPosition.x, 0f, 0f);
                        break;
                    case Side.Top:
                        newPos = this.linkedPortal.transform.position;
                        break;
                    case Side.Bottom:
                        newPos = this.linkedPortal.transform.position + new Vector3(0f, MathF.Abs(triggerCollider.transform.localPosition.y), 0f);
                        break;
                    case Side.Left:
                        newPos = this.linkedPortal.transform.position - new Vector3(playerObj.transform.Find("PortalTriggerLeft").localPosition.x, 0f, 0f);
                        break;
                    default:
                        newPos = Vector3.zero; break;
                }
                break;
            default:
                newPos = Vector3.zero; break;

        }


        if (rb != null)
        {
            rb.position = newPos;
            Physics2D.SyncTransforms();
        }

        Vector2 newVelocity = oldVelocity;


        if (GhostMovement.calc[0] == Side.Left)
        {
            if (GhostMovement.calc[1] == Side.Bottom)
            {
                newVelocity.y = Mathf.Abs(oldVelocity.x) + 10f;
            }
            else if (GhostMovement.calc[1] == Side.Left)
            {
                newVelocity.x = -oldVelocity.x;
            }
        }
        else if (GhostMovement.calc[0] == Side.Right)
        {
            if (GhostMovement.calc[1] == Side.Bottom)
            {
                newVelocity.y = Mathf.Abs(oldVelocity.x) + 10f;
            }
            else if (GhostMovement.calc[1] == Side.Right)
            {
                newVelocity.x = -oldVelocity.x;
            }
        }
        else if (GhostMovement.calc[0] == Side.Bottom)
        {
            if (GhostMovement.calc[1] == Side.Bottom)
            {
                newVelocity.y = -oldVelocity.y;
                newVelocity.x = 0f;
            }
            else if (GhostMovement.calc[1] == Side.Left)
            {
                newVelocity.x = -oldVelocity.y;
                newVelocity.y = 0;
            }
            else if (GhostMovement.calc[1] == Side.Right)
            {
                newVelocity.x = oldVelocity.y;
                newVelocity.y = 0;
            }
        }
        else if (GhostMovement.calc[0] == Side.Top)
        {
            if (GhostMovement.calc[1] == Side.Top)
            {
                newVelocity.y = -oldVelocity.y;
            }
            else if (GhostMovement.calc[1] == Side.Left)
            {
                newVelocity.x = oldVelocity.y;
                newVelocity.y = 0;
            }
            else if (GhostMovement.calc[1] == Side.Right)
            {
                newVelocity.x = -oldVelocity.y;
                newVelocity.y = 0;
            }
        }


        PlayerMovement pMovement = playerObj.GetComponent<PlayerMovement>();
        if (pMovement != null)
        {
            pMovement.JustTeleported();
        }


        StartCoroutine(ApplyVelocityAfterFixed(rb, newVelocity));

        if (this.side != this.linkedPortal.side)
        {
            StartCoroutine(ReenableWallsAfterFixed(playerObj, GetWallCollidersBySide(this.side)));
        }
        else
        {
            unlock = false;
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
                }
            }



            if (other.CompareTag("PortalTrigger"))
            {
                if ((other.gameObject.name == "PortalTriggerLeft" && this.side == Side.Right) || (other.gameObject.name == "PortalTriggerRight" && this.side == Side.Left) || (other.gameObject.name == "PortalTriggerBott" && this.side == Side.Top) || (other.gameObject.name == "PortalTriggerTop" && this.side == Side.Bottom))
                {
                    TeleportPlayer(other);
                    Debug.Log($"Portal at {this.linkedPortal.transform.position} | Teleported at: {other.transform.position}");
                    Debug.Log($"Unlock {unlock}");
                }
            }



            if (!string.IsNullOrEmpty(other.tag) && other.tag.StartsWith("PortalWallTrigger"))
            {
                if (other.tag.Substring(17, 1) == "V")
                    activeVerticalTriggers.Add(other.gameObject);
                else
                {
                    activeHorizontalTriggers.Add(other.gameObject);
                    Debug.Log(other.tag);
                }


                if ((activeVerticalTriggers.Count == 2) || (activeHorizontalTriggers.Count == 2))
                {
                    Collider2D[] walscols = GetWallCollidersBySide(this.side);

                    GameObject playerObj = other.transform.parent != null
                            ? other.transform.parent.gameObject
                            : other.GetComponentInParent<Collider2D>()?.gameObject;

                    Collider2D playerCollider = playerObj != null
                                                ? playerObj.GetComponent<Collider2D>()
                                                : other.GetComponentInParent<Collider2D>();

                    foreach (Collider2D wallCol in walscols)
                    {
                        if (wallCol != null && playerCollider != null)
                        {
                            Physics2D.IgnoreCollision(playerCollider, wallCol, true);
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
                else if (this.linkedPortal.side == this.side)
                {
                    float offset = this.transform.position.x - other.transform.position.x;
                    GhostMovement.offset = new Vector3(range.x + 2f * offset, range.y);
                }
            }
            else if (this.side == Side.Left) 
            {
                if (this.linkedPortal.side == Side.Top)
                {
                    float offset = (this.transform.position.x - other.transform.position.x);

                    Vector3 adjustedOffset = new Vector3(range.x + offset, range.y - offset);

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == Side.Bottom)
                {
                    float offset = (this.transform.position.x - other.transform.position.x);

                    Vector3 adjustedOffset = new Vector3(range.x + offset, range.y + offset);

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == this.side)
                {
                    float offset = this.transform.position.x - other.transform.position.x;
                    GhostMovement.offset = new Vector3(range.x + 2f * offset, range.y);
                }
            }
            else if (this.side == Side.Bottom)
            {
                if (this.linkedPortal.side == Side.Right)
                {
                    Vector3 offset = this.transform.position - other.transform.position;

                    Vector3 adjustedOffset = new Vector3(range.x + offset.x - offset.y, range.y + offset.y);

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == Side.Left)
                {
                    Vector3 offset = this.transform.position - other.transform.position;

                    Vector3 adjustedOffset = new Vector3(range.x + offset.x - MathF.Abs(offset.y), range.y + offset.y);

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == this.side)
                {
                    Vector3 offset = this.transform.position - other.transform.position;
                    GhostMovement.offset = new Vector3(range.x + offset.x, range.y + 2f * offset.y);
                }
            }
            else if (this.side == Side.Top)
            {
                if (this.linkedPortal.side == Side.Right)
                {
                    Vector3 offset = this.transform.position - other.transform.position;

                    Vector3 adjustedOffset = new Vector3(range.x + offset.x + MathF.Abs(offset.y), range.y + MathF.Abs(offset.y));

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == Side.Left)
                {
                    Vector3 offset = this.transform.position - other.transform.position;

                    Vector3 adjustedOffset = new Vector3(range.x + offset.x - MathF.Abs(offset.y), range.y + MathF.Abs(offset.y));

                    GhostMovement.offset = adjustedOffset;
                }
                else if (this.linkedPortal.side == this.side)
                {
                    Vector3 offset = this.transform.position - other.transform.position;
                    GhostMovement.offset = new Vector3(range.x + offset.x, range.y + 2f * offset.y);
                }
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return;

        if (other.CompareTag("Player") && this.linkedPortal != null)
        {
            GhostMovement.offset = Vector3.up * 25;
        }

        if (!string.IsNullOrEmpty(other.tag) && other.tag.Length > 17 && other.tag.StartsWith("PortalWallTrigger") && unlock)
        {
            if (other.tag.Substring(17, 1) == "V")
                activeVerticalTriggers.Remove(other.gameObject);
            else
                activeHorizontalTriggers.Remove(other.gameObject);

            if (activeVerticalTriggers.Count < 2)
            {
                GameObject playerObj = other.transform.parent != null
                                       ? other.transform.parent.gameObject
                                       : other.GetComponentInParent<Collider2D>()?.gameObject;

                if (playerObj != null)
                {
                    Collider2D[] walls = GetWallCollidersBySide(this.side);
                    ReenableWallCollisionsForPlayer(playerObj, walls);
                }

                Debug.Log("Wall phasing DISABLED");
            }
        }
        if (other.CompareTag("Player"))
            unlock = true;
    }
}