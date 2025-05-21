using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRefillHandler : MonoBehaviour
{
    public PlayerHealth playerHealth;

    public void RefillHoneyFlask()
    {
        if (playerHealth != null)
        {
            playerHealth.RefillHoneyFlask();
        }
    }
}
