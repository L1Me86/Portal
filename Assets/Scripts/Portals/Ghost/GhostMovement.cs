using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GhostMovement : MonoBehaviour
{
    public Transform target;
    static public Vector3 offset;
    public static Portal.Side[] calc = new Portal.Side[2];
    public Transform head;
    public bool facingRight = true;
    public GameObject targetObject;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    void Start()
    {
        offset = Vector3.up * 25;
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
        bool did = false;
        if (calc != null && calc[0] == Portal.Side.Right)
        {
            if (calc[1] == Portal.Side.Top || calc[1] == Portal.Side.Bottom)
            {
                this.transform.rotation = target.transform.rotation * Quaternion.Euler(0, 0, -90);
                transform.position = target.position + target.TransformDirection(offset);

                did = true;
            }
        }/*
        else if (calc == Portal.Side.Bottom)
        {

        }
        else if (calc == Portal.Side.Left)
        {

        }
        else if (calc == Portal.Side.Right)
        {

        }*/
        if (!did)
        {
            if (target != null)
            {
                transform.position = target.position + target.TransformDirection(offset);
                transform.rotation = target.rotation;
            }

            head.localScale = playerMovement.head.localScale;
        }
    }


}
