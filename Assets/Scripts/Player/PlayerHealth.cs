using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    public float health;
    public float maxHealth;
    public Image healthBar;

    private PlayerMovement playerMovement;
    public float knockbackForce = 2f;

    private bool isInvincible = false;
    public float invincibilityDuration = 1f;



    void Start()
    {
        maxHealth = health;
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1);
    }

    public void TakeDamage(float damageAmount, Transform damageSource)
    {
        if (isInvincible) return;

        health -= damageAmount;

        // Get knockback direction
        Vector2 direction = (transform.position - damageSource.position).normalized;
        Vector2 knockbackDir = new Vector2(direction.x, 1f).normalized;

        if (playerMovement != null)
        {
            playerMovement.ApplyKnockback(knockbackDir * knockbackForce);
        }

        // Optional: Clamp health to 0
        health = Mathf.Max(health, 0);

        StartCoroutine(InvincibilityCoroutine());

        // Cancel player's attack if they're hit
        PlayerAttack attackScript = GetComponent<PlayerAttack>();
        if (attackScript != null && attackScript.isAttacking)
        {
            attackScript.CancelAttack();
        }

    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        // Turn off collision between Player and Enemy layers
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        yield return new WaitForSeconds(invincibilityDuration);

        // Re-enable collision
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);

        isInvincible = false;
    }


}
