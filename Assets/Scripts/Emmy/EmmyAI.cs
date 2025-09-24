using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmyAI : MonoBehaviour
{

    public Transform player;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true; 
        }
    }
}
