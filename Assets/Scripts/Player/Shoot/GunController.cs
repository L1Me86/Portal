using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 15f;

    public Color portalAColor = Color.blue;
    public Color portalBColor = new Color(1f, 0.5f, 0f);

    private bool lastShotWasA = false; // чтобы чередовать цвета

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Shoot();
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        bool isA = !lastShotWasA;
        lastShotWasA = isA;
        bullet.GetComponent<SpriteRenderer>().color = isA ? portalAColor : portalBColor;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = firePoint.right * bulletSpeed;
    }
}
