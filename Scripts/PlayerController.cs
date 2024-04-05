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
    [SerializeField] float speed, jumpForce, jumpDetectRadius, slopeDetectRadius, jumpTimerMax, groundedTimerMax, checkDistanceRight, checkDistanceLeft, checkDistanceUp, lightUp, lightRight, pushSlower, slopeCheckOffset, slidingSpeed, slidingTimerMax, slideThetaMax, moveableDetectRadius;
    Transform isInLight;
    private float horizontal, jumpTimer, groundedTimer, slidingTimer;
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
        if (pulling)
        {
            activeSpeed = speed * pushSlower;
        }
        float vertical = Mathf.Clamp(rb.velocity.y, -100, jumpForce);
        float slope = slopeDetect();
        float slopeTheta = slopeThetaDetect(slope);
        bool isSliding = slopeTheta >= slideThetaMax * Mathf.Deg2Rad;

        if (isSliding)
        {
            if(slidingTimer > 0)
                slidingTimer -= Time.deltaTime;
            else
                slidingTimer = 0;
        }
        else
        {
            slidingTimer = slidingTimerMax;

        }
        if (slidingTimer != 0 || !IsGrounded())
        {
            rb.velocity = new Vector2(horizontal * activeSpeed, vertical);
            FlipPlayer();
        }
        else
        {

            rb.velocity = new Vector2(-Mathf.Abs(Mathf.Cos(Mathf.Atan(slope)))*slidingSpeed, -Mathf.Abs(Mathf.Sin(Mathf.Atan(slope))) * slidingSpeed);
            print(slopeTheta);
            FlipSlidingPlayer(slope);
        }

        animator.SetBool("Sliding", slidingTimer == 0);        
        animator.SetFloat("Horizontal", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("Vertical", rb.velocity.y);
  
        animator.SetBool("Holding", pulling);
        animator.SetBool("SignMatch", pullingObject && Mathf.Sign(pullingObject.collider.gameObject.transform.position.x - transform.position.x) == Mathf.Sign(rb.velocity.x));










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
        Vector2 ray2 = new Vector2(transform.position.x - slopeCheckOffset, transform.position.y + checkDistanceUp);

        RaycastHit2D hit1 = Physics2D.Raycast(ray1, -Vector2.up, slopeDetectRadius + transform.lossyScale.y / 2, groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(ray2, -Vector2.up, slopeDetectRadius + transform.lossyScale.y / 2, groundLayer);

        if (hit1)
            Debug.DrawLine(ray1, ray1 - Vector2.up, Color.green, 1);

        if (hit2)
            Debug.DrawLine(ray2, ray2 - Vector2.up, Color.green, 1);




        if (hit1 && hit2 && hit1.collider == hit2.collider )
        {

            return (hit2.point.y - hit1.point.y) / (hit2.point.x - hit1.point.x);
        }
        else
        {
            return 0;
        }

    }
    private RaycastHit2D CloseToMoevable()
    {

        Vector2 ray1 = new Vector2(transform.position.x, transform.position.y + checkDistanceUp);

        RaycastHit2D hit1 = Physics2D.Raycast(ray1, transform.right, moveableDetectRadius + transform.lossyScale.y / 2, moveableLayer);

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

    GameObject FindChildWithTag(GameObject parent, string tag)
    {
        GameObject child = null;

        foreach (Transform transform in parent.transform)
        {
            if (transform.CompareTag(tag))
            {
                child = transform.gameObject;
                break;
            }
        }

        return child;
    }

    public void Pull(InputAction.CallbackContext context)
    {
        
        
        if (context.started)
        {
            pullingObject = CloseToMoevable();
            if (pullingObject)
            {
                pullingObject.collider.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                pullingObject.collider.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                pullingObject.collider.gameObject.transform.parent = transform;
                FindChildWithTag(pullingObject.collider.gameObject, "BackupCollider").SetActive(true);
                FindChildWithTag(pullingObject.collider.gameObject, "BackupCollider").transform.parent = transform;
                
                pulling = true;
            }
            else
            {
                pulling = false;
            }
            
        }
        
        if (context.canceled)
        {
            if (pullingObject)
            {
                pullingObject.collider.gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
                pullingObject.collider.gameObject.transform.parent = null;
                FindChildWithTag(gameObject, "BackupCollider").SetActive(false);
                FindChildWithTag(gameObject, "BackupCollider").transform.parent = pullingObject.collider.gameObject.transform;
                pullingObject.collider.gameObject.GetComponent<BoxCollider2D>().enabled = true;
                pullingObject = new RaycastHit2D();
            }
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
