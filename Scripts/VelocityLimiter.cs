using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityLimiter : MonoBehaviour
{
    public float maxHorizontalVelocity = 5f;
    public float maxVerticalVelocity = 5f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Limit horizontal velocity
        if (Mathf.Abs(rb.velocity.x) > maxHorizontalVelocity)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxHorizontalVelocity, rb.velocity.y);
        }

        // Limit vertical velocity
        if (Mathf.Abs(rb.velocity.y) > maxVerticalVelocity)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Sign(rb.velocity.y) * maxVerticalVelocity);
        }
        
    }
}

