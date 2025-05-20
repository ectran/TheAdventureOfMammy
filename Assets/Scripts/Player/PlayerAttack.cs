using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator anim;
    public bool isAttacking { get; private set; } = false;

    public GameObject attackPoint;
    private PolygonCollider2D attackCollider;

    public GameObject upswingAttackPoint;
    private PolygonCollider2D upswingCollider;
    private bool isUpswing = false;

    public GameObject downswingAttackPoint;
    private PolygonCollider2D downswingCollider;
    private bool isDownswing = false;

    public LayerMask enemies;
    public float damage;

    private float attackDuration = 0.5f;
    private float attackTimer = 0f;

    public float knockbackForceToEnemy = 1f;
    public float knockbackForceToPlayer = 1.5f;

    private PlayerHealth playerHealth;



    void Start()
    {
        attackCollider = attackPoint.GetComponent<PolygonCollider2D>();
        upswingCollider = upswingAttackPoint.GetComponent<PolygonCollider2D>();
        downswingCollider = downswingAttackPoint.GetComponent<PolygonCollider2D>();

        attackCollider.enabled = false;
        upswingCollider.enabled = false;
        downswingCollider.enabled = false;

        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();

        if (Input.GetMouseButtonDown(0) && !isAttacking && !playerMovement.isRolling)
        {
            isAttacking = true;

            isUpswing = Input.GetKey(KeyCode.W);
            isDownswing = Input.GetKey(KeyCode.S) && !playerMovement.grounded;

            if (isUpswing)
                anim.SetBool("upswing", true);
            else if (isDownswing)
                anim.SetBool("downswing", true);
            else
                anim.SetBool("attack", true);


            attackTimer = attackDuration;
            startAttack();

        }

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                endAttack();
            }
        }
    }

    public void startAttack()
    {
        attackTimer = attackDuration;
    }

    public void endAttack()
    {
        isAttacking = false;
        isUpswing = false;
        isDownswing = false;

        anim.SetBool("attack", false);
        anim.SetBool("upswing", false);
        anim.SetBool("downswing", false);

        attackCollider.enabled = false;
        upswingCollider.enabled = false;
        downswingCollider.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void attack()
    {
        attackCollider.enabled = true;
        upswingCollider.enabled = false;
        downswingCollider.enabled = false;
        StartCoroutine(EnableHitboxForDuration(0.1f));
    }

    public void attackUpswing()
    {
        upswingCollider.enabled = true;
        attackCollider.enabled = false;
        downswingCollider.enabled = false;
        StartCoroutine(EnableUpswingHitbox(0.1f));
    }

    public void attackDownswing()
    {
        downswingCollider.enabled = true;
        attackCollider.enabled = false;
        upswingCollider.enabled = false;
        StartCoroutine(EnableDownswingHitbox(0.1f));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemies) != 0)
        {
            //Debug.Log("Hit enemy with polygon collider");
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                float finalDamage = damage;

                if (playerHealth != null && playerHealth.IsSweetnessBuffActive())
                {
                    finalDamage *= 1.5f;
                }

                enemyHealth.health -= finalDamage;
                Debug.Log($"Player dealt {finalDamage} damage.");

                if (playerHealth != null)
                {
                    playerHealth.GainSweetness(10f);
                }
            }

            // Knockback enemy
            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                Vector2 direction = (other.transform.position - transform.position).normalized;
                enemyAI.ApplyKnockback(direction * knockbackForceToEnemy);
            }


            // Knockback player
            if (!isUpswing && !isDownswing)
            {
                PlayerMovement playerMovement = GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    Vector2 directionFromEnemy = (transform.position - other.transform.position).normalized;
                    Vector2 knockbackVelocity = directionFromEnemy * knockbackForceToPlayer;
                    playerMovement.ApplyKnockback(knockbackVelocity);
                }
            }
            else if (isDownswing)
            {
                // Vertical knockback (upwards) for downswing
                PlayerMovement playerMovement = GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    Vector2 knockbackVelocity = new Vector2(0f, knockbackForceToPlayer * 1.5f);
                    playerMovement.ApplyKnockback(knockbackVelocity);
                }
            }

        }
    }

    private IEnumerator EnableHitboxForDuration(float duration)
    {
        attackCollider.enabled = true;
        yield return new WaitForSeconds(duration);
        attackCollider.enabled = false;
    }

    private IEnumerator EnableUpswingHitbox(float duration)
    {
        upswingCollider.enabled = true;
        yield return new WaitForSeconds(duration);
        upswingCollider.enabled = false;
    }

    private IEnumerator EnableDownswingHitbox(float duration)
    {
        downswingCollider.enabled = true;
        yield return new WaitForSeconds(duration);
        downswingCollider.enabled = false;
    }


    public void CancelAttack()
    {
        isAttacking = false;
        isUpswing = false;
        isDownswing = false;

        anim.SetBool("attack", false);
        anim.SetBool("upswing", false);
        anim.SetBool("downswing", false);

        attackCollider.enabled = false;
        upswingCollider.enabled = false;
        downswingCollider.enabled = false;
        StopAllCoroutines();
    }





}
    