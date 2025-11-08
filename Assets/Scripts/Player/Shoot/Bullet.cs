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
        Collider2D hitCollider = collision.collider;
        Transform current = hitCollider.transform;

        while (current != null)
        {
            if (current.CompareTag("PortalSurface"))
            {
                Vector2 hitPoint = collision.GetContact(0).point;

                GameObject portalObj = Instantiate(portalPrefab, hitPoint, Quaternion.identity);
                Portal newPortal = portalObj.GetComponent<Portal>();
                newPortal.isBlue = isBluePortal;

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

                Debug.Log($"Portal created at {hitPoint} | Scale: {portalObj.transform.localScale}");

                Portal opposite = GunController.GetOppositePortal(isBluePortal);
                if(opposite != null)
                {
                    newPortal.LinkTo(opposite);
                }

                GunController.SetActivePortal(isBluePortal, newPortal);

                Destroy(gameObject);
                return;
            }
            current = current.parent;
        }
    }

}