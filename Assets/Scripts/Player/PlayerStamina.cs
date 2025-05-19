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

        staminaBar.fillAmount = Mathf.Clamp(stamina / maxStamina, 0, 1);
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
