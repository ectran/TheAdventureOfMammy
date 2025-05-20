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

    private Animator anim;

    public int maxHoneyFlasks = 3;
    public int currentHoneyFlasks = 3;

    public float refillProgress = 0f;
    public float refillThreshold = 100f;
    public float healAmount = 70f;

    public Image refillBarFill; 
    public Image[] honeyIcons;



    void Start()
    {
        maxHealth = health;
        playerMovement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        
        UpdateHoneyUI();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1);

            if (Input.GetKeyDown(KeyCode.Q))
        {
            UseHoneyFlask();
        }
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

        if (anim != null)
        {
            anim.SetTrigger("hurt");
        }

        if (health <= 0f && anim != null)
        {
            anim.SetTrigger("die");
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            // Stop physics if needed
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Collider2D>().enabled = false;
        }

    }

    public void UseHoneyFlask()
    {
        if (currentHoneyFlasks <= 0) return;

        currentHoneyFlasks--;
        health = Mathf.Min(health + healAmount, maxHealth);

        UpdateHoneyUI();

        if (anim != null)
        {
            anim.SetTrigger("heal");
        }
    }

    void UpdateHoneyUI()
    {
        for (int i = 0; i < honeyIcons.Length; i++)
        {
            honeyIcons[i].enabled = i < currentHoneyFlasks;
        }

        if (refillBarFill != null)
            refillBarFill.fillAmount = refillProgress / refillThreshold;
    }

    public void GainSweetness(float amount)
    {

        refillProgress += amount;
        refillProgress = Mathf.Min(refillProgress, refillThreshold);

        if (refillBarFill != null)
            refillBarFill.fillAmount = refillProgress / refillThreshold;

        if (refillProgress >= refillThreshold)
        {
            if (currentHoneyFlasks < maxHoneyFlasks)
            {
                refillProgress = 0f;
                currentHoneyFlasks++;
                UpdateHoneyUI();
            }
            else
            {
                refillProgress = refillThreshold;
            }
        }
    }

    public bool IsSweetnessBuffActive()
    {
        return refillProgress >= refillThreshold && currentHoneyFlasks == maxHoneyFlasks;
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
