using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Anasininami : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    bool isFacingRight=true;
    [Header("Movement")]
    public float moveSpeed =7f;
    float horizontalMovement;

    [Header("Dashing")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 0.1f;
    bool isDashing;
    bool canDash = true;
    TrailRenderer trailRenderer;

    [Header("Jumping")]
    public float jumpPower =  10f;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize =new Vector2(0.5f,0.05f);
    public LayerMask groundLayer;
    bool isGrounded;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize =new Vector2(0.5f,0.05f);
    public LayerMask wallLayer;

    [Header("WallMovement")]
    public float wallSlideSpeed =2;
    bool isWallSliding;

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    void Update()
    {
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("magnitude", rb.linearVelocity.magnitude);
        animator.SetBool("isWallSliding", isWallSliding);
        if(isDashing)
        {
            return;
        }
        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();
        

        if(!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontalMovement*moveSpeed,rb.linearVelocity.y);
            Flip();
        }
    }



    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Dash(InputAction.CallbackContext context)
    {
       if(context.performed && canDash)
       {
        rb.gravityScale = 0;
        StartCoroutine(DashCoroutine());
       }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        trailRenderer.emitting = true;
        float dashDirection = isFacingRight ? 1f: -1f;

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);//dash movement

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = new Vector2(0f,rb.linearVelocity.y); //Reset Horizontal velocity

        isDashing = false;
        trailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        rb.gravityScale = baseGravity;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(jumpsRemaining >0)
        {
        if(context.performed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            jumpsRemaining--;
            animator.SetTrigger("jump");
        }
        else if (context.canceled)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,rb.linearVelocity.y*0.5f);
            jumpsRemaining--;
            animator.SetTrigger("jump");
        }
        }

        if(context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0;
            animator.SetTrigger("jump");

            if(transform.localScale.x != wallJumpDirection)
            {
            isFacingRight= !isFacingRight;
            Vector3 Is =transform.localScale;
            Is.x *= -1f;
            transform.localScale = Is;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime +0.1f);
        }
    }

    private void GroundCheck()
    {
        if(Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded =false;
        }
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }
    
    private void ProcessGravity()
    {
        if (rb.linearVelocity.y <0)
        {
            rb.gravityScale = baseGravity*fallSpeedMultiplier;
            rb.linearVelocity= new Vector2(rb.linearVelocity.x,Mathf.Max(rb.linearVelocity.y,-maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        if(!isGrounded & WallCheck() & horizontalMovement!=0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void ProcessWallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if(wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if(isFacingRight && horizontalMovement <0 || !isFacingRight && horizontalMovement >0)
        {
            isFacingRight= !isFacingRight;
            Vector3 Is =transform.localScale;
            Is.x *= -1f;
            transform.localScale = Is;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(groundCheckPos.position,groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(wallCheckPos.position,wallCheckSize);
    }
}
