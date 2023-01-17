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
    [SerializeField] private float moveSpeed = 5f;

    [Header("Special abilities")]
    [SerializeField] private bool canClimb = false;
    [SerializeField] private bool wallClinging = false;
    [SerializeField] private float wallSpeed = 0f;
    [SerializeField] private bool airAcro = false;
    private bool isDashing;
    private bool canDash;
    [SerializeField] private float dashSpeed = 20f;
    private float dashTime = 0.2f;

    [Header("Jump parameters")]
    private bool isJumping;
    private bool doubleJumping;
    private bool jumpPressed;  
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
            animator.SetBool("Idle", false);
            animator.SetBool("Run", false);
        }

        if (Input.GetButtonDown("Fire3") && canDash)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Run", false);
            StartCoroutine(Dash());
        }
        if(isGrounded())
        {
            if (horizontal == 0)
            {
                animator.SetBool("Idle", true);
                animator.SetBool("Run", false);
            }
            else
            {
                animator.SetBool("Run", true);
                animator.SetBool("Idle", false);
            }
            isJumping = false;
            doubleJumping = false;
            if (!isDashing)
                canDash = true;
        }
        if(isJumping)
        {
            animator.SetBool("jump", true);
        }
        else
            animator.SetBool("jump", false);

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
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
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
                animator.SetBool("Djump", true);
            }
        }

        //fall code
        if (rb.velocity.y < 0.01)
        {
            rb.AddForce(Vector3.up * gravityAccel * gravityScale, ForceMode.Acceleration);

            if(rb.velocity.y < -1)
                animator.SetBool("Djump", false);


        }


    }

    //Dashing coroutine
    public IEnumerator Dash()
    {
        animator.SetBool("jump", true);
        canDash = false;
        isDashing = true;
        yield return new WaitForSeconds(dashTime);
        if(isGrounded())
            canDash = true;
        isDashing = false;
        animator.SetBool("jump", false);


    }

    //Check for on ground
    private bool isGrounded()
    {
        Vector3 halfExtents = bc.bounds.extents;
        float maxDistance = halfExtents.y;
        halfExtents.y = 0.01f;

        return Physics.BoxCast(bc.bounds.center, halfExtents, Vector3.down, Quaternion.identity, maxDistance, LayerMask.GetMask("Ground"));

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
