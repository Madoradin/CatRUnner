using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove2D : MonoBehaviour
{
    private Rigidbody rb;
    private BoxCollider bc;

    [SerializeField] private Animator animator;

    private int faceDirection;
    private float horizontal;
    public bool isMoving;
    [SerializeField] private float moveSpeed = 5f;
    private RaycastHit groundInfo;
    private Vector3 velocity;

    [Header("Special abilities")]
    [SerializeField] private bool canClimb = false;
    public bool wallClinging = false;
    [SerializeField] private float wallSpeed = 0f;
    [SerializeField] private bool airAcro = false;
    public bool isDashing;
    public bool canDash;
    [SerializeField] private float dashSpeed = 20f;
    private float dashTime = 0.2f;

    [Header("Jump parameters")]
    public bool isJumping;
    public bool doubleJumping;
    public bool jumpPressed;  
    public static float gravityAccel = -9.81f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private float maxJumpVelocity = 5f;

    
    // Initialize default settings
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();

        faceDirection = 1;
        isDashing = false;
        jumpPressed = false;
        doubleJumping = false;
        isJumping = false;
        canDash = true;
    }

    //Update is for controls and booleans
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        velocity = rb.velocity;

        if (horizontal > 0)
        {
            faceDirection = 1;
            if(transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        if (horizontal < 0)
        {
            faceDirection = -1;
            if (transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        if (Input.GetButtonDown("Jump") && (isGrounded() || wallClinging || (airAcro && !doubleJumping)))
        {
            jumpPressed = true;

        }

        if (Input.GetButtonDown("Fire3") && canDash)
        {
            StartCoroutine(Dash());
        }
        if(isGrounded())
        {
            float slopeAngle = Vector2.Angle(groundInfo.normal, Vector2.up);
            if(slopeAngle != 0)
            {
                Transform spriteObject = gameObject.transform.Find("naokoSprite");

                var slopeVector = Quaternion.AngleAxis(slopeAngle, Vector3.forward);
                spriteObject.localRotation *= slopeVector;
                ClimbSlope(ref velocity, slopeAngle);
                rb.velocity = velocity;
            }

            isMoving = horizontal != 0;
            isJumping = false;
            doubleJumping = false;
            if (!isDashing)
                canDash = true;
        }

        if(canClimb)
            WallCling();
    }

    // FixedUpdate is for physics based statements
    void FixedUpdate()
    {
        //update velocity and clamp vertical velocity so player doesn't go flying
        rb.velocity = new Vector3(horizontal  * moveSpeed, Mathf.Clamp(rb.velocity.y,-maxJumpVelocity*3,maxJumpVelocity), 0);

        //trigger dash
        if(isDashing)
        {
            rb.velocity = Vector3.right * dashSpeed * faceDirection;
        }

        //normal jump
        if(jumpPressed && isGrounded() && !wallClinging)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + jumpForce, 0);
            isJumping = true;
            jumpPressed = false;

        }

        //speacial jumps
        if (!isGrounded())
        {
            //wall climb code
            if (jumpPressed && wallClinging)
            {
                rb.velocity = new Vector3(-faceDirection * dashSpeed * 2, jumpForce * 2, 0);
                isJumping = true;
                jumpPressed = false;
            }
            //double jump code
            else if (jumpPressed && airAcro && !doubleJumping)
            {
                rb.AddForce(new Vector3(0, jumpForce * 2, 0), ForceMode.Impulse);
                isJumping = true;
                jumpPressed = false;
                doubleJumping = true;
            }

            //fall code
            if (rb.velocity.y < 0.01)
            {
                rb.AddForce(Vector3.up * gravityAccel * gravityScale, ForceMode.Acceleration);

            }
        }




    }

    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        velocity.y = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
    }

    void DescendSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        velocity.y -= Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
    }

    //Dashing coroutine
    public IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        yield return new WaitForSeconds(dashTime);
        if(isGrounded())
            canDash = true;
        isDashing = false;


    }

    //Check for on ground
    public bool isGrounded()
    {
        Vector3 halfExtents = bc.bounds.extents;
        float maxDistance = halfExtents.y;
        halfExtents.y = 0.01f;

        return Physics.BoxCast(bc.bounds.center,
                               halfExtents,
                               Vector3.down,
                               out groundInfo,
                               Quaternion.identity,
                               maxDistance,
                               LayerMask.GetMask("Ground"));

    }

    //check for wall touch
    private bool touchingWall()
    {
        Vector3 halfExtents = bc.bounds.extents;
        float maxDistance = halfExtents.x;
        halfExtents.x = 0.01f;

        return Physics.BoxCast(bc.bounds.center, halfExtents, Vector3.right * faceDirection, Quaternion.identity, maxDistance, LayerMask.GetMask("Ground"));
    }

    //wall slide code
    private void WallCling()
    {
        if(touchingWall() && !isGrounded() && horizontal != 0f)
        {
            wallClinging = true;
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSpeed, float.MaxValue), 0);
        }
        else
        {
            wallClinging = false;
        }
    }
}
