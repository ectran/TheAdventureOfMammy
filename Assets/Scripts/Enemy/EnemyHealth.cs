using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health;
    public float currentHealth;
    public float damage;

    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;

    private bool isDead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = health;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (health < currentHealth)
        {
            currentHealth = health;
            anim.SetTrigger("attacked");
        }
        
        // enemy death
        if (health <= 0 && !isDead)
        {
            isDead = true;
            anim.SetBool("dead", true);

            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            col.enabled = false;

            StartCoroutine(DestroyAfterDeath());
        }
    }

    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }

    // deals damage to player on collision
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, transform);

            }

            Debug.Log("Player collided with enemy");
        }
    }


}

