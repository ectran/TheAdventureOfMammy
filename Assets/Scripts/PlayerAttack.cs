using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator anim;
    public bool isAttacking { get; private set; } = false;

    public GameObject attackPoint;
    public float radius;
    public LayerMask enemies;
    public float damage;

    private float attackDuration = 0.25f;
    private float attackTimer = 0f;

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
        attack();
        attackTimer = attackDuration;
    }

    public void endAttack()
    {
        isAttacking = false;
        anim.SetBool("attacking", false);

        GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
    }

    public void attack()
    {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);

        foreach (Collider2D enemyGameObject in enemy)
        {
            Debug.Log("Hit enemy");
            enemyGameObject.GetComponent<EnemyHealth>().health -= damage;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.transform.position, radius);

        if (attackPoint != null)
        {
            Gizmos.color = isAttacking ? Color.green : Color.red;
            Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
        }
    }
}
    