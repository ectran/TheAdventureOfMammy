using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator anim;
    public bool isAttacking { get; private set; } = false;

    public GameObject attackPoint;
    private PolygonCollider2D attackCollider;

    public LayerMask enemies;
    public float damage;

    private float attackDuration = 0.5f;
    private float attackTimer = 0f;

    public float knockbackForceToEnemy = 1f;
    public float knockbackForceToPlayer = 1.5f;


    void Start()
    {
        attackCollider = attackPoint.GetComponent<PolygonCollider2D>();
        attackCollider.enabled = false;
    }

    void Update()
    {
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();

        if (Input.GetMouseButtonDown(0) && !isAttacking && !playerMovement.isRolling)
        {
            isAttacking = true;
            anim.SetBool("attacking", true);


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
        anim.SetBool("attacking", false);

        attackCollider.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void attack()
    {
        attackCollider.enabled = true;
        StartCoroutine(EnableHitboxForDuration(0.1f));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemies) != 0)
        {
            //Debug.Log("Hit enemy with polygon collider");
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.health -= damage;
            }

            // Knockback enemy
            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                Vector2 direction = (other.transform.position - transform.position).normalized;
                enemyAI.ApplyKnockback(direction * knockbackForceToEnemy);
            }


            // Knockback player
            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Vector2 directionFromEnemy = (transform.position - other.transform.position).normalized;
                Vector2 knockbackVelocity = directionFromEnemy * knockbackForceToPlayer;
                playerMovement.ApplyKnockback(knockbackVelocity);
            }


        }
    }

    private IEnumerator EnableHitboxForDuration(float duration)
    {
        attackCollider.enabled = true;
        yield return new WaitForSeconds(duration);
        attackCollider.enabled = false;
    }
    
    public void CancelAttack()
    {
        isAttacking = false;
        anim.SetBool("attacking", false);
        attackCollider.enabled = false;
        StopAllCoroutines(); // stops the hitbox coroutine if running
    }



    
}
    