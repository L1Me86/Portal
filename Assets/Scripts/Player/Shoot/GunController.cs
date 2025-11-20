using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 15f;

    public Color blueColor = new Color(0.663f, 0.588f, 1f);
    public Color orangeColor = new Color(1f, 0.68f, 0.36f);

    private static Portal bluePortal;
    private static Portal orangePortal;

    void Update()
    {
        if (GameManager.gameIsPaused) return;
        if (Input.GetMouseButtonDown(0)) Shoot(true);
        if (Input.GetMouseButtonDown(1)) Shoot(false);
    }

    void Shoot(bool isBlue)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.setPortalType(isBlue);

        bullet.GetComponent<SpriteRenderer>().color = isBlue ? blueColor : orangeColor;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed;
    }

    public static void SetActivePortal(bool isBlue, Portal portal)
    {
        if (isBlue && bluePortal != null)
        {
            bluePortal.Unlink();
            Destroy(bluePortal.gameObject);
        }
        if (!isBlue && orangePortal != null)
        {
            orangePortal.Unlink();
            Destroy(orangePortal.gameObject);
        }

        if (isBlue) bluePortal = portal;
        else orangePortal = portal;
    }

    public static Portal GetOppositePortal(bool isBlue)
    {
        return isBlue ? orangePortal : bluePortal;
    }
}
