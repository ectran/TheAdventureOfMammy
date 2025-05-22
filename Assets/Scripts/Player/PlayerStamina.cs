using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    public float stamina;
    public float maxStamina;
    public Image staminaBar;

    public float regenRate = 20f;
    private bool isDraining = false;
    private float regenDelay = 0.5f;
    private float regenTimer = 0f;

    public Image staminaBarChip;
    public float chipSpeed = 0.5f;

    private float chipDelayTimerS = 0f;
    private float chipDelayDurationS = 0.5f;

    void Start()
    {
        maxStamina = stamina;
    }

    void Update()
    {
        if (stamina < maxStamina)
        {
            if (!isDraining)
            {
                if (regenTimer >= regenDelay)
                {
                    stamina += regenRate * Time.deltaTime;
                    stamina = Mathf.Clamp(stamina, 0, maxStamina);
                }
                else
                {
                    regenTimer += Time.deltaTime;
                }
            }
        }
        else
        {
            regenTimer = 0f;
        }

        float normalizedStamina = Mathf.Clamp(stamina / maxStamina, 0f, 1f);

        staminaBar.fillAmount = normalizedStamina;

        if (staminaBarChip != null)
        {
            if (staminaBarChip.fillAmount > normalizedStamina)
            {
                if (chipDelayTimerS < chipDelayDurationS)
                {
                    chipDelayTimerS += Time.deltaTime;
                }
                else
                {
                    staminaBarChip.fillAmount = Mathf.MoveTowards(staminaBarChip.fillAmount, normalizedStamina, chipSpeed * Time.deltaTime);
                }
            }
            else
            {
                staminaBarChip.fillAmount = normalizedStamina;
                chipDelayTimerS = 0f;
            }
        }
    }
    

    public bool UseStamina(float amount)
    {
        if (stamina >= amount)
        {
            stamina -= amount;
            regenTimer = 0f;
            return true;
        }
        return false;
    }

    public void SetDraining(bool state)
    {
        isDraining = state;
    }
}
