using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    private float health = 100;
    private float maxHealth;
    public Image healthBar;

    private PlayerMovement playerMovement;
    private float normalSpeed;
    private float healSlowSpeed = 1f;
    private bool isHealingActive = false;
   
    private float knockbackForce = 3f;
    private bool isInvincible = false;
    private float invincibilityDuration = 1.5f;

    private Animator anim;
    public Animator emmyAnimator;

    private int maxHoneyFlasks = 3;
    private int currentHoneyFlasks = 3;

    private float refillProgress = 0f;
    private float refillThreshold = 100f;
    private float healAmount = 70f;

    public Image refillBarFill; 
    public Image[] honeyIcons;
    private Coroutine healingCoroutine;
    private bool lastSweetnessBuffState = false;

    public Image healthBarChip;
    public Image sweetnessBarChip;
    private float chipSpeed = 0.5f;
    private float chipDelayTimerS = 0f;
    private float chipDelayDurationS = 0.5f;
    private float chipDelayTimerH = 0f;
    private float chipDelayDurationH = 0.5f;

    public DialogueManager dialogueManager;

    string[] deathLines = {
        //"M- Mammy? Please wake up… ;^;",
        //"I’m sorry…",
        //"Nooooo!",
        //"Don’t leave me here… ):",
        //"Everything feels so cold without you…",
        //"Stay with me please…"
        "died!"

    };

    string[] buffLines = {
        //"You're glowing, honey!",
        //"Don't let it get to your head, love!",
        //"Go get 'em! <3",
        //"You're so full of sweetness!",
        //"That buff looks good on you! C;",
        //"Go Mammy go! Go Mammy go!! Go Mammy goooo!!!"
        "giving buff!"
    };

    string[] refillLines = {
        //"Let me help you out there, honey!",
        //"You're welcome! Mwah!!",
        //"All set, Mammy! Don't waste it!",
        //"I'm right here! Forever and always, hehe.",
        //"I got you, don't worry!",
        //"More honey for my love!"
        "refilling potions!"
    };


    void Start()
    {
        maxHealth = health;
        playerMovement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();

        if (playerMovement != null)
        {
            normalSpeed = playerMovement.moveSpeed;
        }

        UpdateHoneyUI();
        
        if (sweetnessBarChip != null)
        {
            sweetnessBarChip.fillAmount = 0f;
        }

    }

    void Update()
    {
        // updates health bar and chip effect
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

        // updates sweetness refill bar chip
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

        // sweetness buff state
        bool isSweetnessBuffActive = IsSweetnessBuffActive();
        if (isSweetnessBuffActive != lastSweetnessBuffState)
        {
            lastSweetnessBuffState = isSweetnessBuffActive;

            if (emmyAnimator != null)
            {
                emmyAnimator.SetBool("sparkle", isSweetnessBuffActive);
            }

            if (isSweetnessBuffActive)
            {
                string chosenBuffLine = buffLines[Random.Range(0, buffLines.Length)];
                dialogueManager?.ShowDialogue(chosenBuffLine);
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

        // applys player knockback
        Vector2 direction = (transform.position - damageSource.position).normalized;
        Vector2 knockbackDir = new Vector2(direction.x, 1f).normalized;

        if (playerMovement != null)
        {
            playerMovement.ApplyKnockback(knockbackDir * knockbackForce);
        }

        health = Mathf.Max(health, 0);

        StartCoroutine(InvincibilityCoroutine());

        // cancels players attack if hit mid attack
        PlayerAttack attackScript = GetComponent<PlayerAttack>();
        if (attackScript != null && attackScript.isAttacking)
        {
            attackScript.CancelAttack();
        }

        if (anim != null)
        {
            anim.SetTrigger("hurt");
        }

        // player death
        if (health <= 0f && anim != null)
        {
            string chosenDeathLine = deathLines[Random.Range(0, deathLines.Length)];
            dialogueManager?.ShowDialogue(chosenDeathLine);

            anim.SetTrigger("die");
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

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
        {
            StopCoroutine(healingCoroutine);
        }

        anim.SetBool("heal", false);

        if (playerMovement != null)
        {
            playerMovement.moveSpeed = normalSpeed;
        }

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
            {
                yield break;
            }

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

                string chosenRefillLine = refillLines[Random.Range(0, refillLines.Length)];
                dialogueManager?.ShowDialogue(chosenRefillLine);
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
        {
            refillBarFill.fillAmount = refillProgress / refillThreshold;
        }

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

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        yield return new WaitForSeconds(invincibilityDuration);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);

        isInvincible = false;
    }


}
