using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ShadowCollider : MonoBehaviour
{
    public GameObject objectToCastFrom;
    int numberOfRays = 360; // Number of rays to cast around the object
    public float raycastDistance = 5f; // Maximum distance to cast the rays
    public LayerMask groundLayer; // Layer mask for ground objects
    public EdgeCollider2D edgeCollider;
    Vector2 lastPosition = Vector2.zero;

    private void Start()
    {
        objectToCastFrom = GameObject.FindGameObjectWithTag("LightSource");
    }
    private void FixedUpdate()
    {
        if (lastPosition != (Vector2)objectToCastFrom.transform.position)
        {
            CastRays();
            lastPosition = (Vector2)objectToCastFrom.transform.position;
        }
    }

    private void Update()
    {
        edgeCollider.isTrigger = objectToCastFrom.transform.parent != null;
    }
    void CastRays()
    {
        Vector2[] points = new Vector2[4];
        // Calculate the angle between rays
        float angleStep = 360f / numberOfRays;
        bool firstHit = false;
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
                if (!firstHit)
                {
                    pointsList.Add(edgeCollider.gameObject.transform.parent.InverseTransformPoint(hit.point));
                    pointsList.Add(edgeCollider.gameObject.transform.parent.InverseTransformPoint(direction * raycastDistance));
                    firstHit = true;
                }
                
            }
            else
            {
                Debug.DrawRay(objectToCastFrom.transform.position, direction * raycastDistance, Color.green);
                if (firstHit)
                {
                    pointsList.Add(lastPoint2);
                    pointsList.Add(lastPoint1);
                    break;
                }

            }

            lastPoint1 = edgeCollider.gameObject.transform.parent.InverseTransformPoint(hit.point);
            lastPoint2 = edgeCollider.gameObject.transform.parent.InverseTransformPoint(direction * raycastDistance);
        }
        points = pointsList.ToArray();
        edgeCollider.points = points;
    }


}
