using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float accelerationTime = 0.5f;
    public Transform head;
    public float jumpForce = 7f;
    public float gravityScale = 3f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public bool facingRight = true;
    public float moveInput;
    public float currentSpeed;

    private float accelerationTimer;
    private bool isGrounded;
    private Rigidbody2D rb;

    [Header("Cube Pickup")]
    public Transform cubeHoldPoint;
    public float pickupRange = 4f;
    public KeyCode pickupKey = KeyCode.E;

    private Cube carriedCube;
    private bool canPickup = true;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<Collider2D>().gameObject.transform.position = Vector3.up * 20;
    }

    void Update()
    {
        moveInput = Input.GetAxis("Horizontal");

        if (moveInput != 0)
        {
            accelerationTimer += Time.deltaTime;
        }
        else
        {
            accelerationTimer = 0;
        }

        rb.gravityScale = gravityScale;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
            Jump();

        float accelerationProgress = Mathf.Clamp01(accelerationTimer / accelerationTime);
        currentSpeed = Mathf.Lerp(0, moveSpeed, accelerationProgress);

        if (Input.GetKeyDown(pickupKey))
        {
            if (carriedCube == null)
            {
                TryPickupCube();
            }
            else
            {
                DropCube();
            }
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);

        if (facingRight == false && moveInput > 0)
        {
            Flip();
        }
        else if (facingRight == true && moveInput < 0)
        {
            Flip();
        }
    }

    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 headScaler = head.localScale;
        headScaler.x *= -1;
        head.localScale = headScaler;
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isGrounded = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    void TryPickupCube()
    {
        if (!canPickup) return;

        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, pickupRange);

        foreach (Collider2D col in nearby)
        {
            if (col.CompareTag("Cube"))
            {
                Cube cube = col.GetComponent<Cube>();
                if (cube != null && cube.isPickable && !cube.isPickedUp)
                {
                    PickupCube(cube);
                    break;
                }
            }
        }
    }

    void PickupCube(Cube cube)
    {
        carriedCube = cube;
        cube.PickUp(cubeHoldPoint);
        canPickup = false;
    }

    void DropCube()
    {
        if (carriedCube != null)
        {
            carriedCube.Drop();

            Rigidbody2D cubeRb = carriedCube.GetComponent<Rigidbody2D>();
            if (cubeRb != null)
            {
                float throwForce = facingRight ? 3f : -3f;
                cubeRb.velocity = new Vector2(throwForce, 5f);
            }

            carriedCube = null;
        }

        canPickup = true;
    }
}
