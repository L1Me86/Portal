using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRotation : MonoBehaviour
{
    public static ArmRotation Instance { get; private set; }

    public Transform arm;
    public Transform ghostArm;
    public Transform realArm;
    //public Transform player;
    //public float rotationSpeed = 15f;

    public PlayerMovement player;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (PlayerMovement.isInPortal)
        {
            realArm = ghostArm;
        }
        else
        {
            realArm = arm;
        }
        if (GameManager.gameIsPaused) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - realArm.position;
        direction.z = 0;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        arm.rotation = Quaternion.Euler(0, 0, angle);

        if (Input.GetAxis("Horizontal") == 0)
        {
            if ((mousePos.x < player.transform.position.x && player.facingRight) ||
                (mousePos.x > player.transform.position.x && !player.facingRight))
            {
                player.Flip();
            }
        }
    }
}