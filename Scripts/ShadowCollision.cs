using UnityEngine;

public class ShadowCollision : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Shadow"))
        {
            // Get the contact point
            ContactPoint2D contact = collision.GetContact(0);

            // Get the direction between the center of the object and the contact point
            Vector2 direction = (contact.point - (Vector2)transform.position).normalized;

            // Give the object a velocity in the opposite direction
            rb.velocity = -direction * 5f; // You can adjust the velocity magnitude as needed
        }
    }
}