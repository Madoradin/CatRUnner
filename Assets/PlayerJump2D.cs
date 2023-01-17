using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump2D : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 move;
    private BoxCollider bc;
    private bool isJumping;
    
    private bool jumpPressed;
    [SerializeField] private float jumpForce = 100f;
    [SerializeField] private float gravityScale = 20f;
    public static float gravityAccel = -9.81f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
        jumpPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump") & !jumpPressed)
        {
            jumpPressed = true;
            move = new Vector3(0, jumpForce, 0);
        }
        
    }

    void FixedUpdate()
    {
        if(jumpPressed && isGrounded())
        {
            Debug.Log("Jump");
            rb.AddForce(move, ForceMode.VelocityChange);
            jumpPressed = false;
            isJumping = true;
    
        }
        if(rb.velocity.y < 0.1 && isJumping)
        {
            rb.AddForce(Vector3.up * gravityAccel * gravityScale, ForceMode.Acceleration);
        }
    }

    private bool isGrounded()
    {
        Vector3 halfExtents = bc.bounds.extents;
        float maxDistance = halfExtents.y;
        halfExtents.y = 0.01f;

        Debug.Log(Physics.BoxCast(bc.bounds.center, halfExtents, Vector3.down, Quaternion.identity, maxDistance, LayerMask.GetMask("Ground")));
        return Physics.BoxCast(bc.bounds.center, halfExtents, Vector3.down, Quaternion.identity, maxDistance, LayerMask.GetMask("Ground"));
        
    }
}
