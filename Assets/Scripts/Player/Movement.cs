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
   

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
}
