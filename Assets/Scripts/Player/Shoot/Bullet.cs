using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject portalPrefab;
    private bool isBluePortal = true;

    public void setPortalType(bool isBlue)
    {
        isBluePortal = isBlue;
    }

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
                if (opposite != null)
                {
                    newPortal.LinkTo(opposite);
                }

                GunController.SetActivePortal(isBluePortal, newPortal);

                placed = true;
                break;
            }
            current = current.parent;
        }
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }


}