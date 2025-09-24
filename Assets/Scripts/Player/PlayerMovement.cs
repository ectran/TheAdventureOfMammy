using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator anim;
    private PlayerAttack playerAttack;

    public float moveSpeed = 3f; 
    private float jumpForce = 6f; 
    public bool grounded;

    public int facingDirection { get; private set; } = 1; 

    private bool canRoll = true;
    public bool isRolling;
    private float rollForce = 4f;
    private float rollingCooldown = 0.4f;

    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private float knockbackDuration = 0.3f;
    private Vector2 knockbackVelocity;

    private PlayerStamina playerStamina;
    private float staminaCostPerRoll = 30f;

    private float jumpCutMultiplier = 0.5f;
    private bool isJumping = false;



    private void Awake()
    {
        if (isRolling)
        {
            return;
        }

        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
        playerStamina = GetComponent<PlayerStamina>();


    }

    private void FixedUpdate()
    {

        if (isKnockedBack)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
            else
            {
                body.velocity = knockbackVelocity;
                return;
            }
        }

        if (isRolling)
        {
            return;
        }

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (playerAttack.isAttacking)
        {
            body.velocity = new Vector2(horizontalInput * moveSpeed * 0.8f, body.velocity.y);
        }
        else
        {
            body.velocity = new Vector2(horizontalInput * moveSpeed, body.velocity.y);
        }

        if (!playerAttack.isAttacking)
        {
            if (horizontalInput > 0.01f)
            {
                transform.localScale = Vector3.one;
                facingDirection = 1;
            }
            else if (horizontalInput < -0.01f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                facingDirection = -1;
            }
        }

        if (Input.GetKey(KeyCode.Space) && grounded)
        {
            Jump();
        }

        if (Input.GetKey(KeyCode.LeftShift) && canRoll && grounded)
        {
            StartCoroutine(Roll());
        }

        anim.SetBool("run", horizontalInput != 0 && !isRolling);
        anim.SetBool("grounded", grounded);
        
        // short hops if tapped
        if (isJumping && !Input.GetKey(KeyCode.Space) && body.velocity.y > 0f)
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y * jumpCutMultiplier);
            isJumping = false;
        }
    }

    private IEnumerator Roll()
    {
        if (!canRoll || playerAttack.isAttacking || playerStamina == null || !playerStamina.UseStamina(staminaCostPerRoll)) yield break;
        {

            canRoll = false;
            isRolling = true;
            playerStamina.SetDraining(true);

            anim.SetTrigger("roll");
            anim.SetBool("rolling", true);

            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

            float originalGravity = body.gravityScale;
            body.gravityScale = 0f;
            body.velocity = new Vector2(transform.localScale.x * rollForce, 0f);

            yield return null;
            anim.ResetTrigger("roll");

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

            isRolling = false;
            anim.SetBool("rolling", false);

            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);

            body.gravityScale = originalGravity;

            playerStamina.SetDraining(false);

            yield return new WaitForSeconds(rollingCooldown);
            canRoll = true;
        }
     
    }

    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpForce);
        anim.SetTrigger("jump");
        grounded = false;
        isJumping = true;
    }

    public void ApplyKnockback(Vector2 velocity)
    {
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        knockbackVelocity = velocity;

        if (knockbackVelocity.y > 0.1f)
        {
            grounded = false;
            isJumping = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
            isJumping = false;
        }
    }
}
