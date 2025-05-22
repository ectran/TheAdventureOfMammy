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
    private float normalSpeed;
    public float healSlowSpeed = 1f;
    private bool isHealingActive = false;
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
    private Coroutine healingCoroutine;

    public Animator emmyAnimator;
    private bool lastSweetnessBuffState = false;

    public Image healthBarChip;
    private float chipSpeed = 0.5f;

    private float chipDelayTimerS = 0f;
    private float chipDelayDurationS = 0.5f;

    private float chipDelayTimerH = 0f;
    private float chipDelayDurationH = 0.5f;

    public Image sweetnessBarChip;


    void Start()
    {
        maxHealth = health;
        playerMovement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();

        if (playerMovement != null)
            normalSpeed = playerMovement.moveSpeed;

        UpdateHoneyUI();
        
        if (sweetnessBarChip != null)
        {
            sweetnessBarChip.fillAmount = 0f;
        }

    }

    void Update()
    {
        float targetFill = Mathf.Clamp(health / maxHealth, 0, 1);

        healthBar.fillAmount = targetFill;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseHoneyFlask();
        }

        if (healthBarChip.fillAmount > targetFill)
        {
            if (chipDelayTimerH < chipDelayDurationH)
            {
                chipDelayTimerH += Time.deltaTime;
            }
            else
            {
                healthBarChip.fillAmount = Mathf.MoveTowards(healthBarChip.fillAmount, targetFill, chipSpeed * Time.deltaTime);
            }
        }
        else if (healthBarChip.fillAmount < targetFill)
        {
            healthBarChip.fillAmount = targetFill;
            chipDelayTimerH = 0f;
        }
        else
        {
            chipDelayTimerH = 0f;
        }

        float refillNormalized = refillProgress / refillThreshold;

        if (sweetnessBarChip != null)
        {
            if (sweetnessBarChip.fillAmount > refillNormalized)
            {
                if (chipDelayTimerS < chipDelayDurationS)
                {
                    chipDelayTimerS += Time.deltaTime;
                }
                else
                {
                    sweetnessBarChip.fillAmount = Mathf.MoveTowards(sweetnessBarChip.fillAmount, refillNormalized, chipSpeed * Time.deltaTime);
                }
            }
            else
            {
                sweetnessBarChip.fillAmount = refillNormalized;
                chipDelayTimerS = 0f;
            }
        }

        bool isSweetnessBuffActive = IsSweetnessBuffActive();
        if (isSweetnessBuffActive != lastSweetnessBuffState)
        {
            lastSweetnessBuffState = isSweetnessBuffActive;

            if (emmyAnimator != null)
            {
                emmyAnimator.SetBool("sparkle", isSweetnessBuffActive);
            }
        }

    }

    public void TakeDamage(float damageAmount, Transform damageSource)
    {
        if (isInvincible) return;

        if (isHealingActive)
        {
            CancelHealing();
        }

        health -= damageAmount;

        // Get knockback direction
        Vector2 direction = (transform.position - damageSource.position).normalized;
        Vector2 knockbackDir = new Vector2(direction.x, 1f).normalized;

        if (playerMovement != null)
        {
            playerMovement.ApplyKnockback(knockbackDir * knockbackForce);
        }

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

            if (emmyAnimator != null)
            {
                emmyAnimator.SetTrigger("cry");
            }
        }

    }
    
    private void CancelHealing()
    {
        if (!isHealingActive) return;

        isHealingActive = false;

        if (healingCoroutine != null)
            StopCoroutine(healingCoroutine);

        anim.SetBool("heal", false);

        if (playerMovement != null)
            playerMovement.moveSpeed = normalSpeed;

    }

    public void UseHoneyFlask()
    {
        if (currentHoneyFlasks <= 0 || isHealingActive) return;

        currentHoneyFlasks--; 
        UpdateHoneyUI();

        TryRefillFromSweetness(); 

        if (anim != null && playerMovement != null)
        {
            isHealingActive = true;
            anim.SetBool("heal", true);
            playerMovement.moveSpeed = healSlowSpeed;

            healingCoroutine = StartCoroutine(HealingRoutine());
        }
    }


    private IEnumerator HealingRoutine()
    {
        float healDuration = .7f;
        float timer = 0f;

        while (timer < healDuration)
        {
            if (!isHealingActive) 
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        anim.SetBool("heal", false);
        playerMovement.moveSpeed = normalSpeed;
        isHealingActive = false;

    }

    void UpdateHoneyUI()
    {
        for (int i = 0; i < honeyIcons.Length; i++)
        {
            honeyIcons[i].enabled = i < currentHoneyFlasks;
        }

        if (refillBarFill != null)
            refillBarFill.fillAmount = refillProgress / refillThreshold;

        if (sweetnessBarChip != null)
    {
        float refillNormalized = refillProgress / refillThreshold;
        if (sweetnessBarChip.fillAmount < refillNormalized)
        {
            sweetnessBarChip.fillAmount = refillNormalized;
        }
    }
        }

    public void ApplyHealing()
    {
        health = Mathf.Min(health + healAmount, maxHealth);
        UpdateHoneyUI();
    }

    private void TryRefillFromSweetness()
    {
        if (refillProgress >= refillThreshold)
        {
            if (currentHoneyFlasks < maxHoneyFlasks)
            {
                if (emmyAnimator != null)
                {
                    emmyAnimator.SetTrigger("heal");
                }
            }
            else
            {
                refillProgress = refillThreshold; 
            }
        }
    }

    public void GainSweetness(float amount)
    {
        refillProgress += amount;
        refillProgress = Mathf.Min(refillProgress, refillThreshold);

        if (refillBarFill != null)
            refillBarFill.fillAmount = refillProgress / refillThreshold;

        TryRefillFromSweetness();
    }


    public void RefillHoneyFlask()
    {
        if (refillProgress >= refillThreshold && currentHoneyFlasks < maxHoneyFlasks)
        {
            refillProgress = 0f;
            currentHoneyFlasks++;
            UpdateHoneyUI();
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
