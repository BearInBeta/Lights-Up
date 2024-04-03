using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    [SerializeField] LayerMask groundLayer, moveableLayer;
    [SerializeField] float speed, jumpForce, jumpDetectRadius, slopeDetectRadius, jumpTimerMax, groundedTimerMax, checkDistanceRight, checkDistanceLeft, checkDistanceUp, lightUp, lightRight, pushSlower, slopeCheckOffset;
    Transform isInLight;
    private float horizontal, jumpTimer, groundedTimer;
    private RaycastHit2D pullingObject;
    private bool pulling;
    // Start is called before the first frame update
    void Start()
    {
        Physics2D.queriesHitTriggers = false;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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

        float activeSpeed = speed;
        if(IsPushing())
        {
            activeSpeed = speed * pushSlower;
        }
        float vertical = Mathf.Clamp(rb.velocity.y, -100, jumpForce);
        float slope = slopeDetect();
        float slopeTheta = slopeThetaDetect(slope);
        bool isSliding = slopeTheta >= Mathf.PI / 4;
        if (!isSliding || !IsGrounded())
        {
            rb.velocity = new Vector2(horizontal * activeSpeed, vertical);
            FlipPlayer();
        }
        else
        {
            if(rb.velocity.y > 0)
            {
                rb.velocity = Vector2.zero;
            }
            FlipSlidingPlayer(slope);
        }

        animator.SetBool("Sliding", isSliding);        
        animator.SetFloat("Horizontal", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("Vertical", rb.velocity.y);
        animator.SetBool("Pushing", IsPushing());
        animator.SetBool("Pulling", pulling && Mathf.Sign(pullingObject.collider.gameObject.transform.position.x - transform.position.x) == -Mathf.Sign(rb.velocity.x));


        GetComponent<BoxCollider2D>().isTrigger = !(IsPushing() && Mathf.Abs(horizontal) > 0);
        
        





    }
    private void Update()
    {
    }

    private void FlipPlayer()
    {
        if (!pulling)
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
    }

    private void FlipSlidingPlayer(float slope)
    {
        if (slope > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (slope < 0)
        {
            transform.rotation = Quaternion.identity;
        }
    }
    private bool IsGrounded()
    {

        Vector2 ray1 = new Vector2(transform.position.x + checkDistanceRight, transform.position.y + checkDistanceUp);
        Vector2 ray2 = new Vector2(transform.position.x - checkDistanceLeft, transform.position.y + checkDistanceUp);

        RaycastHit2D hit1 = Physics2D.Raycast(ray1, -Vector2.up, jumpDetectRadius + transform.lossyScale.y / 2, groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(ray2, -Vector2.up, jumpDetectRadius + transform.lossyScale.y / 2, groundLayer);

        if (hit1)
            Debug.DrawLine(ray1, ray1 - Vector2.up, Color.red, 1);

        if (hit2)
            Debug.DrawLine(ray2, ray2 - Vector2.up, Color.red, 1);


        return hit1 || hit2;
    }

    private float slopeThetaDetect(float slope)
    {
        if (slope != 0)
        {
            return Mathf.Atan(Mathf.Abs(slope));
        }
        else
        {
            return 0;
        }

    }
    private float slopeDetect()
    {
        Vector2 ray1 = new Vector2(transform.position.x + slopeCheckOffset, transform.position.y + checkDistanceUp);
        Vector2 ray2 = new Vector2(transform.position.x + slopeCheckOffset + 0.01f, transform.position.y + checkDistanceUp);

        RaycastHit2D hit1 = Physics2D.Raycast(ray1, -Vector2.up, slopeDetectRadius + transform.lossyScale.y / 2, groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(ray2, -Vector2.up, slopeDetectRadius + transform.lossyScale.y / 2, groundLayer);

        if (hit1)
            Debug.DrawLine(ray1, ray1 - Vector2.up, Color.red, 1);

        if (hit2)
            Debug.DrawLine(ray2, ray2 - Vector2.up, Color.red, 1);


        Vector2 ray3 = new Vector2(transform.position.x - slopeCheckOffset, transform.position.y + checkDistanceUp);
        Vector2 ray4 = new Vector2(transform.position.x - slopeCheckOffset - 0.01f, transform.position.y + checkDistanceUp);

        RaycastHit2D hit3 = Physics2D.Raycast(ray1, -Vector2.up, slopeDetectRadius + transform.lossyScale.y / 2, groundLayer);
        RaycastHit2D hit4 = Physics2D.Raycast(ray2, -Vector2.up, slopeDetectRadius + transform.lossyScale.y / 2, groundLayer);

        if (hit3)
            Debug.DrawLine(ray3, ray3 - Vector2.up, Color.red, 1);

        if (hit4)
            Debug.DrawLine(ray4, ray4 - Vector2.up, Color.red, 1);

        if (hit1 && hit2 && hit1.collider == hit2.collider || hit3 && hit4 && hit3.collider == hit4.collider)
        {

            float slope12 = (hit2.point.y - hit1.point.y) / (hit2.point.x - hit1.point.x);
            float slope34 = (hit4.point.y - hit3.point.y) / (hit4.point.x - hit3.point.x);

            if (Mathf.Abs(slope12) > Mathf.Abs(slope34))
            { 
                return slope12; 
            }
            else
            {
                return slope34;
            }
        }
        else
        {
            return 0;
        }

    }
    private RaycastHit2D IsPushing()
    {

        Vector2 ray1 = new Vector2(transform.position.x, transform.position.y + checkDistanceUp);

        RaycastHit2D hit1 = Physics2D.Raycast(ray1, transform.right, jumpDetectRadius + transform.lossyScale.y / 2, moveableLayer);

        if(hit1)
        Debug.DrawLine(ray1, ray1 + (Vector2)transform.right, Color.red, 1);

        return hit1;
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
    public void Pull(InputAction.CallbackContext context)
    {
        
        
        if (context.performed)
        {
            pullingObject = IsPushing();
            if (pullingObject)
            {
                pullingObject.collider.gameObject.transform.parent = transform;
                pullingObject.collider.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                pulling = true;
            }
            
        }

        if (context.canceled && pullingObject)
        {
            pullingObject.collider.gameObject.transform.parent = null;
            pullingObject.collider.gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
            pulling =  false;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && !pulling)
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
