using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform firePoint;
    public Transform ghostFirePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 100f;
    public Collider2D playerCollider;

    public Color blueColor = new Color(0.663f, 0.588f, 1f);
    public Color orangeColor = new Color(1f, 0.68f, 0.36f);

    private static Portal bluePortal;
    private static Portal orangePortal;

    void Update()
    {
        if (GameManager.gameIsPaused || GameManager.gameIsEnded || GameManager.gameIsFinished) return;
        if (Input.GetMouseButtonDown(0)) Shoot(true);
        if (Input.GetMouseButtonDown(1)) Shoot(false);
    }

    void Shoot(bool isBlue)
    {
        bool transformToGhost = PlayerMovement.bulletTransform();

        GameObject bullet = Instantiate(bulletPrefab, transformToGhost ? ghostFirePoint.position : firePoint.position, firePoint.rotation);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.setPortalType(isBlue);
        bulletScript.playerCollider = playerCollider;
        bulletScript.player = playerCollider.gameObject;

        bullet.GetComponent<SpriteRenderer>().color = isBlue ? blueColor : orangeColor;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed;
    }

    public static void SetActivePortal(bool isBlue, Portal portal)
    {
        if (isBlue)
        {
            if (bluePortal != null)
            {
                bluePortal.Unlink();
                Destroy(bluePortal.gameObject);
            }

            bluePortal = portal;
        }
        else
        {
            if (orangePortal != null)
            {
                orangePortal.Unlink();
                Destroy(orangePortal.gameObject);
            }

            orangePortal = portal;
        }
    }

    public static Portal GetActivePortal(bool isBlue)
    {
        return isBlue ? bluePortal : orangePortal;
    }

    public static Portal GetOppositePortal(bool isBlue)
    {
        return isBlue ? orangePortal : bluePortal;
    }
}