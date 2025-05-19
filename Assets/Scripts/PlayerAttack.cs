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
        }
    }

    
}
    