using System.Numerics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject portalPrefab;
    public Collider2D playerCollider;
    public GameObject player;

    private bool isBluePortal = true;

    void Start()
    {
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
        if (!string.IsNullOrEmpty(t) && t.StartsWith("PortalSurface"))
        {
            UnityEngine.Vector2 hitPoint;
            float portalSize = (portalPrefab.GetComponent<Collider2D>() as BoxCollider2D).size.y / 2f;
            if (t == "PortalSurfaceRight" || t == "PortalSurfaceLeft")
            {
                float colliderSize = (collider as BoxCollider2D).size.y / 2f;
                float offsetY;

                hitPoint = new UnityEngine.Vector2(collider.transform.position.x + ((collider as BoxCollider2D).size.x / 2f) * (t == "PortalSurfaceLeft" ? 1 : -1), this.transform.position.y);
                offsetY = Mathf.Abs(hitPoint.y - collider.transform.position.y);

                if (offsetY > colliderSize - portalSize)
                {
                    hitPoint += UnityEngine.Vector2.down * (Mathf.Sign(hitPoint.y - collider.transform.position.y) * (offsetY - (colliderSize - portalSize)));
                    Debug.Log("portal moved");
                }
            }
            else
            {
                float colliderSize = (collider as BoxCollider2D).size.x / 2f;
                float offsetX;

                hitPoint = new UnityEngine.Vector2(this.transform.position.x, collider.transform.position.y + (collider as BoxCollider2D).size.y / 2f * (t == "PortalSurfaceBott" ? 1 : -1));
                offsetX = Mathf.Abs(hitPoint.x - collider.transform.position.x);

                if (offsetX > colliderSize - portalSize)
                {
                    hitPoint += UnityEngine.Vector2.left * (Mathf.Sign(hitPoint.x - collider.transform.position.x) * (offsetX - (colliderSize - portalSize)));
                    Debug.Log("portal moved");
                }
            }

            GameObject portalObj = Instantiate(portalPrefab, hitPoint, UnityEngine.Quaternion.identity);
            Portal newPortal = portalObj.GetComponent<Portal>();
            newPortal.isBlue = isBluePortal;

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
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Transform current = collision.collider.transform;
        bool placed = false;

        while (current != null)
        {
            string t = current.tag;
            if (!string.IsNullOrEmpty(t) && t.StartsWith("PortalSurface"))
            {
                Vector2 hitPoint = collision.GetContact(0).point;

                GameObject portalObj = Instantiate(portalPrefab, hitPoint, Quaternion.identity);
                Portal newPortal = portalObj.GetComponent<Portal>();
                newPortal.isBlue = isBluePortal;
                
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


                placed = true;
                break;
            }
            current = current.parent;
        }
        Destroy(gameObject);
    }*/

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}