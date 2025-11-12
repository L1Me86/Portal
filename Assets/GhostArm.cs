using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostArm : MonoBehaviour
{
    public Transform playerArm;
    public Transform ghostArm;
    void Start()
    {
        if (playerArm == null)
            Debug.LogError("playerArm isn't set");
        if (ghostArm == null)
            Debug.LogError("ghostArm isn't set");
    }

    void Update()
    {
        ghostArm.rotation = playerArm.rotation;
    }
}
