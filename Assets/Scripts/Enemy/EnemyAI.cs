using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public Transform player;
    public float moveSpeed = 2f;
    private Rigidbody2D rb;

    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    public float knockbackDuration = 0.3f;
    private Vector2 knockbackVelocity;

    private Animator anim;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
            else
            {
                rb.velocity = knockbackVelocity;
                if (anim != null)
                    anim.SetBool("run", false);
                return;
            }
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        

        if (anim != null)
            anim.SetBool("run", Mathf.Abs(direction.x) > 0.01f); 


        if (direction.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }
    
    public void ApplyKnockback(Vector2 velocity)
    {
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        knockbackVelocity = velocity;
    }


}
