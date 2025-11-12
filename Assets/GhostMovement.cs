using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public Transform head;
    public bool facingRight = true;
    public GameObject targetObject;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    void Start()
    {
        offset = Vector3.up * 5; // исправить на 15
        if (targetObject != null)
        {
            rb = targetObject.GetComponent<Rigidbody2D>();
            playerMovement = targetObject.GetComponent<PlayerMovement>();
        }
        else
        {
            Debug.LogError("Target Object isn't set");
        }
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + target.TransformDirection(offset);
            transform.rotation = target.rotation;
        }

        head.localScale = playerMovement.head.localScale;
    }


}
