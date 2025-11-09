using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject portalPrefab;

    //void Start()
    //{
    //    GameObject player = GameObject.FindGameObjectWithTag("Player");
    //    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("PortalSurface"))
        {
            Vector2 hitPoint = collision.GetContact(0).point;

            Instantiate(portalPrefab, hitPoint, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (!collision.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
        }  
    }
}