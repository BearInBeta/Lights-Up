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
    Transform isInLight;
    private float horizontal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

    }


    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundcheck.position, jumpDetectRadius, groundLayer);
    }

    public void lighControl(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.tag.Equals("LightSource"))
                {
                    transform.GetChild(i).parent = null;
                    return;
                }
            }

            if (isInLight != null && isInLight.parent == null)
            {
               

                isInLight.parent = transform;
                isInLight.transform.localPosition = new Vector3(0, 0, isInLight.transform.localPosition.z);

            }
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
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.Equals("LightSource"))
        {
            isInLight = collision.gameObject.transform;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("LightSource"))
        {
            isInLight = null;
        }
    }
}
