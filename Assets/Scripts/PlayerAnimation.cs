using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMove2D playerMove;



    // Update is called once per frame
    void Update()
    {
        animator.SetBool("dash", playerMove.isDashing);
        animator.SetBool("jump", playerMove.isJumping);
        animator.SetBool("Djump", playerMove.doubleJumping);
        animator.SetBool("wallCling", playerMove.wallClinging);

        if (playerMove.isGrounded())
        {
            animator.SetBool("Idle", !playerMove.isMoving);
            animator.SetBool("Run", playerMove.isMoving);

        }

        if (playerMove.jumpPressed || playerMove.isDashing)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Run", false);
        }


    }
}
