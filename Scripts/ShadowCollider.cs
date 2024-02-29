using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ShadowCollider : MonoBehaviour
{
    public GameObject objectToCastFrom;
    public int numberOfRays = 36; // Number of rays to cast around the object
    public float raycastDistance = 5f; // Maximum distance to cast the rays
    public LayerMask groundLayer; // Layer mask for ground objects
    public EdgeCollider2D edgeCollider;
    Vector2 lastPosition = Vector2.zero;
    private void Update()
    {

        Debug.DrawRay(transform.parent.InverseTransformPoint(new Vector2(0,0)), transform.parent.InverseTransformPoint(new Vector2(1, 1)), Color.red);

        if(lastPosition != (Vector2) objectToCastFrom.transform.position)
        {
            lastPosition = (Vector2) objectToCastFrom.transform.position;
            CastRays();
        }
        
    }

    void CastRays()
    {
        Vector2[] points = new Vector2[4];
        // Calculate the angle between rays
        float angleStep = 360f / numberOfRays;

        bool hitBefore = false;
        Vector2 lastPoint1 = new Vector2();
        Vector2 lastPoint2 = new Vector2();
        List<Vector2> pointsList = new List<Vector2>();
        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = i * angleStep;
            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;

            RaycastHit2D hit = Physics2D.Raycast(objectToCastFrom.transform.position, direction, raycastDistance, groundLayer);

            bool hitFound = hit && hit.collider.gameObject == gameObject;
            // Draw the rays and color code them
            Color rayColor = hitFound ? Color.red : Color.green;

            // Draw the rays up to the hit point
            if (hitFound)
            {
                Debug.DrawRay(hit.point, direction * raycastDistance, Color.red);
                
                    pointsList.Add(edgeCollider.gameObject.transform.parent.InverseTransformPoint(hit.point));
                    pointsList.Add(edgeCollider.gameObject.transform.parent.InverseTransformPoint(direction * raycastDistance));
                    hitBefore = true;
                
                
            }
            else
            {
                Debug.DrawRay(objectToCastFrom.transform.position, direction * raycastDistance, Color.green);
               
            }

            lastPoint1 = edgeCollider.gameObject.transform.parent.InverseTransformPoint(hit.point);
            lastPoint2 = edgeCollider.gameObject.transform.parent.InverseTransformPoint(direction * raycastDistance);
        }
        points = pointsList.ToArray();
        edgeCollider.points = points;
    }


}
