using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform groundcheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float speed, jumpForce, jumpDetectRadius;
    [SerializeField] Transform lightSource;
    bool isInLight = false;
    private float horizontal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

    }


    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundcheck.position, jumpDetectRadius, groundLayer);
    }

    public void lighControl(InputAction.CallbackContext context)
    {
        if(context.performed && lightSource.parent != null && lightSource.parent.Equals(transform))
        {
            lightSource.parent = null;
        }else if(context.performed && isInLight && lightSource.parent == null)
        {
            lightSource.parent = transform;
            lightSource.transform.localPosition = new Vector3(0,0,lightSource.transform.localPosition.z);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("LightSource"))
        {
            isInLight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("LightSource"))
        {
            isInLight = false;
        }
    }
}
