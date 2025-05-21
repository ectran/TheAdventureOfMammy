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

    public Image staminaBarChip;
    public float chipSpeed = .5f;

    void Start()
    {
        maxStamina = stamina;
    }

    void Update()
    {
        if (!isDraining && stamina < maxStamina)
        {
            stamina += regenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }

        float normalizedStamina = Mathf.Clamp(stamina / maxStamina, 0f, 1f);

        staminaBar.fillAmount = normalizedStamina;

        if (staminaBarChip != null)
        {
            if (staminaBarChip.fillAmount > normalizedStamina)
            {
                staminaBarChip.fillAmount = Mathf.MoveTowards(staminaBarChip.fillAmount, normalizedStamina, chipSpeed * Time.deltaTime);
            }
            else
            {
                staminaBarChip.fillAmount = normalizedStamina;
            }
        }
    }
    

    public bool UseStamina(float amount)
    {
        if (stamina >= amount)
        {
            stamina -= amount;
            return true;
        }
        return false;
    }

    public void SetDraining(bool state)
    {
        isDraining = state;
    }
}
