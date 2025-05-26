using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float airControl = 0.6f;

    [Header("Jumping")]
    public float jumpForce = 12f;
    public float jumpBufferTime = 0.2f;
    public float coyoteTime = 0.2f;

    [Header("Gravity")]
    public float gravityMultiplier = 2f;
    public float lowJumpMultiplier = 1.5f;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    private bool grounded;
    private float lastGroundedTime;
    private float jumpBufferCounter;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;
    private Rigidbody rb;

    public DeckManager deckManager;
    public CardManager cardManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        // coyote time
        if (grounded)
        {
            lastGroundedTime = Time.time;
        }

        // jump buffering
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = Time.time + jumpBufferTime;
        }

        MyInput();
        SpeedControl();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyGravity();
        Jump();
    }

    private void MyInput()
    {
        // player input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        // movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // movement force
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airControl, ForceMode.Force);
        }
    }

    private void Jump()
    {
        // jump buffering/coyote time
        if (jumpBufferCounter > Time.time && (grounded || Time.time - lastGroundedTime < coyoteTime))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            jumpBufferCounter = 0;
            lastGroundedTime = 0;
        }
    }

    private void ApplyGravity()
    {
        // increase gravity while falling
        if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        }
        // small hop
        else if (rb.linearVelocity.y >= 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.down * lowJumpMultiplier, ForceMode.Acceleration);
        }
    }

    private void SpeedControl()
    {
        // limit player speed
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    public void DoubleJump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        cardManager.UpdateHandUI();
    }
}
