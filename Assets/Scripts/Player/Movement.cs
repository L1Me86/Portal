using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }
    [SerializeField] private Transform shoulder;
    public Transform Shoulder => shoulder;

    public Transform ghostFirePoint;

    public float moveSpeed = 5f;
    public float accelerationTime = 0.2f;
    public Transform head;
    public float jumpForce = 7f;
    public float gravityScale = 3f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    public bool facingRight = true;
    public float moveInput;
    public float currentSpeed;
    public float jumpHeight = 0;
    public static bool isInPortal = false;
    public static bool transformBulletToGhost = false;
    public static Portal linked;

    private float accelerationTimer;
    private bool isGrounded;
    private Rigidbody2D rb;
    private Vector2 addedVelocity;
    private bool justTeleported = false;
    private int teleportFrames = 3;

    [Header("Cube Pickup")]
    public Transform cubeHoldPoint;
    public float pickupRange = 4f;
    public KeyCode pickupKey = KeyCode.E;

    private Cube carriedCube;
    public bool canPickup = true;

    [Header("Air Control")]
    public float airSpeed = 3f;
    public float groundFriction = 0.9f;
    public float airDeceleration = 0.5f;
    public float groundDeceleration = 50f;

    [Header("Speed Limits")]
    public float maxGroundSpeed = 20f;
    public float maxAirSpeed = 100f;
    public float maxFallSpeed = 40f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeReferences();
    }
    void InitializeReferences()
    {
        if (shoulder == null) shoulder = transform.Find("Shoulder");
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<Collider2D>().gameObject.transform.position = Vector3.up * 20;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

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

        if (!isGrounded)
        {
            if (rb.position.y > jumpHeight)
                jumpHeight = rb.position.y;
        }
        else
        {
            jumpHeight = 0;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
            Jump();

        currentSpeed = moveSpeed;

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
        if (justTeleported)
        {
            teleportFrames--;
            if (teleportFrames <= 0) justTeleported = false;
            return;
        }

        float targetSpeed = moveInput * moveSpeed;
        float accel = isGrounded ? 20f : 10f;

        float newX = Mathf.Lerp(rb.velocity.x, targetSpeed, accel * Time.fixedDeltaTime);

        if (moveInput == 0)
        {
            float decel = isGrounded ? groundDeceleration : airDeceleration;

            newX = Mathf.Lerp(newX, 0, decel * Time.fixedDeltaTime);
        }

        float maxSpeed = isGrounded ? maxGroundSpeed : maxAirSpeed;
        newX = Mathf.Clamp(newX, -maxSpeed, maxSpeed);

        rb.velocity = new Vector2(newX, rb.velocity.y);

        if (rb.velocity.y < -maxFallSpeed) rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);

        if (facingRight == false && moveInput > 0)
        {
            Flip();
        }
        else if (facingRight == true && moveInput < 0)
        {
            Flip();
        }

        if (addedVelocity != Vector2.zero)
        {
            rb.position += addedVelocity * Time.fixedDeltaTime;
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

    public void JustTeleported()
    {
        justTeleported = true;
        teleportFrames = 2;
    }
    void PickupCube(Cube cube)
    {
        carriedCube = cube;
        cube.PickUp(cubeHoldPoint);
        cube.transform.rotation = cubeHoldPoint.transform.parent.parent.rotation;
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

    public static bool bulletTransform()
    {
        if (isInPortal)
        {
            switch (linked.side)
            {
                case Portal.Side.Left:
                    if (Instance.ghostFirePoint.position.x > linked.transform.position.x)
                    {
                        transformBulletToGhost = true;
                        return true;
                    }
                    break;
                case Portal.Side.Right:
                    if (Instance.ghostFirePoint.position.x < linked.transform.position.x)
                    {
                        transformBulletToGhost = true;
                        return true;
                    }
                    break;
                case Portal.Side.Top:
                    if (Instance.ghostFirePoint.position.y < linked.transform.position.y)
                    {
                        transformBulletToGhost = true;
                        return true;
                    }
                    break;
                case Portal.Side.Bottom:
                    if (Instance.ghostFirePoint.position.y > linked.transform.position.y)
                    {
                        transformBulletToGhost = true;
                        return true;
                    }
                    break;
                default:
                    return false;
            }
        }
        transformBulletToGhost = false;
        return false;
    }
    /*
    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.collider.CompareTag("MovablePlatform"))
        {
            MovingPlatform plat = col.collider.GetComponent<MovingPlatform>();
            if (plat != null)
            {
                addedVelocity = plat.platformVelocity;
            }
        }
        else
        {
            addedVelocity = Vector2.zero;
        }
    }*/
}