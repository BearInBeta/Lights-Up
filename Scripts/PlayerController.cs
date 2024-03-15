using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float speed, jumpForce, jumpDetectRadius, jumpTimerMax, groundedTimerMax, checkDistanceRight, checkDistanceLeft, lightUp, lightRight;
    Transform isInLight;
    private float horizontal, jumpTimer, groundedTimer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //rb.angularVelocity = 0;

        if (IsGrounded())
        {
            animator.SetBool("Grounded", true);
            groundedTimer = groundedTimerMax;
        }
        else
        {
            animator.SetBool("Grounded", false);

        }
        if (groundedTimer > 0 && jumpTimer > 0)
        {
            groundedTimer = 0;
            jumpTimer = 0;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (jumpTimer > 0)
            jumpTimer -= Time.deltaTime;
        else
            jumpTimer = 0;

        if (groundedTimer > 0)
            groundedTimer -= Time.deltaTime;
        else
            groundedTimer = 0;

        float vertical = Mathf.Clamp(rb.velocity.y, -100, jumpForce);
        rb.velocity = new Vector2(horizontal * speed, vertical);
        animator.SetFloat("Horizontal", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("Vertical", rb.velocity.y);

        FlipPlayer();





    }
    private void Update()
    {
    }

    private void FlipPlayer()
    {
        if (rb.velocity.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (rb.velocity.x > 0)
        {
            transform.rotation = Quaternion.identity;
        }
    }
    private bool IsGrounded()
    {
        Physics2D.queriesHitTriggers = false;

        Vector2 ray1 = new Vector2(transform.position.x + checkDistanceRight, transform.position.y);
        Vector2 ray2 = new Vector2(transform.position.x - checkDistanceLeft, transform.position.y);

        RaycastHit2D hit1 = Physics2D.Raycast(ray1, -Vector2.up, jumpDetectRadius + transform.lossyScale.y / 2, groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(ray2, -Vector2.up, jumpDetectRadius + transform.lossyScale.y / 2, groundLayer);

        if (hit1)
            Debug.DrawLine(ray1, ray1 - Vector2.up, Color.red, 1);

        if (hit2)
            Debug.DrawLine(ray2, ray2 - Vector2.up, Color.red, 1);

        return hit1 || hit2;
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
                isInLight.transform.localPosition = new Vector3(lightRight, lightUp, isInLight.transform.localPosition.z);

            }
        }
        if (isInLight != null)
        {
            isInLight.rotation = Quaternion.identity;
        }


    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        if (context.canceled && groundedTimer > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
       
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            jumpTimer = jumpTimerMax;
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
