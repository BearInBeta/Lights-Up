using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float speed, jumpForce, jumpDetectRadius, jumpTimerMax, groundedTimerMax;
    Transform isInCarry;
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
            groundedTimer = groundedTimerMax;
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

        
    }
    private void Update()
    {
    }


    private bool IsGrounded()
    {
        Physics2D.queriesHitTriggers = false;

        Vector2 ray1 = new Vector2(transform.position.x + (transform.lossyScale.x / 2 - 0.01f), transform.position.y);
        Vector2 ray2 = new Vector2(transform.position.x - (transform.lossyScale.x / 2 - 0.01f), transform.position.y);

        RaycastHit2D hit1 = Physics2D.Raycast(ray1, -Vector2.up, jumpDetectRadius + transform.lossyScale.y / 2, groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(ray2, -Vector2.up, jumpDetectRadius + transform.lossyScale.y / 2, groundLayer);

        if (hit1)
            Debug.DrawLine(ray1, hit1.point, Color.red, 1);

        if (hit2)
            Debug.DrawLine(ray2, hit2.point, Color.red, 1);

        return hit1 || hit2;
    }

    public void carryItem(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.tag.Equals("Carry"))
                {
                    transform.GetChild(i).parent = null;
                    return;
                }
            }

            if (isInCarry != null && isInCarry.parent == null)
            {
               

                isInCarry.parent = transform;
                isInCarry.transform.localPosition = new Vector3(0, 0, isInCarry.transform.localPosition.z);

            }
        }
        if (isInCarry != null)
        {
            isInCarry.rotation = Quaternion.identity;
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
        if (collision.tag.Equals("Carry"))
        {
            isInCarry = collision.gameObject.transform;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Carry"))
        {
            isInCarry = null;
        }
    }
}
