using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;
    public bool isBlue = true;

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
        if (other.CompareTag("Player") && linkedPortal != null)
        {
            other.transform.position = linkedPortal.transform.position + Vector3.up * 0.5f;
        }
    }

    

    
}