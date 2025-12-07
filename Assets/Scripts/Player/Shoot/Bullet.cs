using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviour
{
    public GameObject portalPrefab;
    public Collider2D playerCollider;
    public GameObject player;

    private bool isBluePortal = true;

    void Start()
    {
        if (!player.GetComponent<PlayerMovement>().canPickup)
        {
            Destroy(gameObject);
        }
        Collider2D bulletCol = GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(bulletCol, playerCollider);
        Collider2D[] playerColliders = player.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in playerColliders)
            Physics2D.IgnoreCollision(bulletCol, col);
        bulletCol.isTrigger = true;
    }

    public void setPortalType(bool isBlue)
    {
        isBluePortal = isBlue;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider);
        string t = collider.tag;
        float portalSize = (portalPrefab.GetComponent<Collider2D>() as BoxCollider2D).size.y / 2f;

        if (!string.IsNullOrEmpty(t) && t.StartsWith("PortalSurface"))
        {
            UnityEngine.Vector2 hitPoint;

            if (t.Equals("Portal"))
            {
                collider = collider.GetComponent<Portal>().sitsOn;
                t = collider.tag;
            }

            float colliderSizeY = (collider as BoxCollider2D).size.y / 2f;
            float colliderSizeX = (collider as BoxCollider2D).size.x / 2f;


            GameObject[] activePortals = GameObject.FindGameObjectsWithTag("Portal");

            foreach (GameObject port in activePortals)
            {
                if (port.GetComponent<Portal>().sitsOn == collider && port.GetComponent<Portal>().isBlue != isBluePortal)
                {
                    if (Mathf.Max(colliderSizeY, colliderSizeX) < portalSize * 2f)
                    {
                        Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        bool onTop = port.transform.rotation != new UnityEngine.Quaternion(0, 0, 0, 0) ? true : false;
                        if (Mathf.Abs(onTop ? (collider.transform.position.x - port.transform.position.x) : (collider.transform.position.y - port.transform.position.y)) + Mathf.Max(colliderSizeX, colliderSizeY) - portalSize < portalSize * 2f )
                        {
                            Destroy(gameObject);
                            return;
                        }
                    }
                }
            }


            if (t == "PortalSurfaceRight" || t == "PortalSurfaceLeft")
            {
                float offsetY;

                hitPoint = new UnityEngine.Vector2(collider.transform.position.x + ((collider as BoxCollider2D).size.x / 2f) * (t == "PortalSurfaceLeft" ? 1 : -1), this.transform.position.y);
                offsetY = Mathf.Abs(hitPoint.y - collider.transform.position.y);

                if (offsetY > colliderSizeY - portalSize)
                {
                    hitPoint.y = collider.transform.position.y + (colliderSizeY - portalSize) * Mathf.Sign(hitPoint.y - collider.transform.position.y);
                    Debug.Log("portal moved" + hitPoint.ToString());
                }
            }
            else
            {
                float offsetX;

                hitPoint = new UnityEngine.Vector2(this.transform.position.x, collider.transform.position.y + (collider as BoxCollider2D).size.y / 2f * (t == "PortalSurfaceBott" ? 1 : -1));
                offsetX = Mathf.Abs(hitPoint.x - collider.transform.position.x);

                if (offsetX > colliderSizeX - portalSize)
                {
                    hitPoint += UnityEngine.Vector2.left * (Mathf.Sign(hitPoint.x - collider.transform.position.x) * (offsetX - (colliderSizeX - portalSize)));
                    Debug.Log("portal moved");
                }
            }

            GameObject portalObj = Instantiate(portalPrefab, hitPoint, UnityEngine.Quaternion.identity);
            Portal newPortal = portalObj.GetComponent<Portal>();
            newPortal.isBlue = isBluePortal;
            newPortal.sitsOn = collider;

            switch (t)
            {
                case "PortalSurfaceRight":
                    newPortal.side = Portal.Side.Right;
                    break;
                case "PortalSurfaceLeft":
                    newPortal.side = Portal.Side.Left;
                    break;
                case "PortalSurfaceBott":
                    newPortal.side = Portal.Side.Bottom;
                    newPortal.transform.Rotate(0, 0, 90);
                    break;
                case "PortalSurfaceUp":
                    newPortal.side = Portal.Side.Top;
                    newPortal.transform.Rotate(0, 0, 90);
                    break;
                default:
                    Debug.LogError("!!! Untagged surface portal creating attempt" + t);
                    break;
            }

            Transform activeChild = portalObj.transform.Find(isBluePortal ? "PortalBlue" : "PortalOrange");
            if (activeChild != null)
            {
                SpriteRenderer sr = activeChild.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingLayerName = "Portal";
                    sr.sortingOrder = 100;
                }
            }

            Portal opposite = GunController.GetOppositePortal(isBluePortal);
            GunController.SetActivePortal(isBluePortal, newPortal);

            if (opposite != null)
            {
                newPortal.LinkTo(opposite);
            }

            if (newPortal.linkedPortal != null)
            {
                if (newPortal.sitsOn == newPortal.linkedPortal.sitsOn)
                {
                    if (newPortal.sitsOn.CompareTag("PortalSurfaceRight") || newPortal.sitsOn.CompareTag("PortalSurfaceLeft"))
                    {
                        if (Mathf.Abs(newPortal.transform.position.y - newPortal.linkedPortal.transform.position.y) < portalSize * 2f)
                        {
                            bool onTop = newPortal.transform.position.y - newPortal.linkedPortal.transform.position.y >= 0 ? true : false;
                            if (Mathf.Abs(newPortal.linkedPortal.transform.position.y + (portalSize * 2f * (onTop ? 1 : -1)) - collider.transform.position.y) > colliderSizeY - portalSize)
                            {
                                newPortal.transform.position = newPortal.linkedPortal.transform.position + new UnityEngine.Vector3(0, -portalSize * 2f * (onTop ? 1 : -1));
                            }
                            else
                            {
                                newPortal.transform.position = newPortal.linkedPortal.transform.position + new UnityEngine.Vector3(0, portalSize * 2f * (onTop ? 1 : -1));
                            }
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(newPortal.transform.position.x - newPortal.linkedPortal.transform.position.x) < portalSize * 2f)
                        {
                            bool onTop = newPortal.transform.position.x - newPortal.linkedPortal.transform.position.x >= 0 ? true : false;
                            if (Mathf.Abs(newPortal.linkedPortal.transform.position.x + (portalSize * 2f * (onTop ? 1 : -1)) - collider.transform.position.x) > colliderSizeX - portalSize)
                            {
                                newPortal.transform.position = newPortal.linkedPortal.transform.position + new UnityEngine.Vector3(-portalSize * 2f * (onTop ? 1 : -1), 0);
                            }
                            else
                            {
                                newPortal.transform.position = newPortal.linkedPortal.transform.position + new UnityEngine.Vector3(portalSize * 2f * (onTop ? 1 : -1), 0);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}