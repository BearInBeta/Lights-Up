using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ShadowCollider : MonoBehaviour
{
    public GameObject objectToCastFrom;
    int numberOfRays = 720;// Number of rays to cast around the object
    float raycastDistance = 100f; // Maximum distance to cast the rays
    public LayerMask groundLayer; // Layer mask for ground objects
    public EdgeCollider2D edgeCollider;
    Vector2 lastPosition = Vector2.zero;

    private void Start()
    {
        objectToCastFrom = GameObject.FindGameObjectWithTag("LightSource");
    }
    private void FixedUpdate()
    {
    }

    private void Update()
    {
        edgeCollider.isTrigger = objectToCastFrom.transform.parent != null;

        if (lastPosition != (Vector2)objectToCastFrom.transform.position)
        {
            //CastRays();
            ShadeCollider();
            lastPosition = (Vector2)objectToCastFrom.transform.position;

        }
    }
    void ShadeCollider()
    {
        Vector2[] points;

        List<Vector2> pointsList = new List<Vector2>();

        Vector2[] corners = new Vector2[4];

        corners[0] = new Vector2(transform.position.x + transform.lossyScale.x/2, transform.position.y + transform.lossyScale.y/2);
        corners[1] = new Vector2(transform.position.x - transform.lossyScale.x/2, transform.position.y + transform.lossyScale.y/2);
        corners[2] = new Vector2(transform.position.x + transform.lossyScale.x/2, transform.position.y - transform.lossyScale.y/2);
        corners[3] = new Vector2(transform.position.x - transform.lossyScale.x/2, transform.position.y - transform.lossyScale.y/2);


        foreach (var cor in corners)
        {
            Vector2 origin = objectToCastFrom.transform.position;
            Vector2 direction = new Vector2(cor.x - origin.x, cor.y - origin.y);
            Vector2 unit = direction.normalized;
            float newX = cor.x + raycastDistance * unit.x;
            float newY = cor.y + raycastDistance * unit.y;
            pointsList.Add(edgeCollider.gameObject.transform.parent.InverseTransformPoint(cor));
            pointsList.Add(edgeCollider.gameObject.transform.parent.InverseTransformPoint(new Vector2(newX  , newY)));

        }

        points = pointsList.ToArray();
        edgeCollider.points = points;
    }
    void CastRays()
    {
        Vector2[] points;
        // Calculate the angle between rays
        float angleStep = 360f / numberOfRays;
        List<Vector2> pointsList = new List<Vector2>();
        List<Vector2[]> allHits = new List<Vector2[]>();
        for (int i = 0; i < numberOfRays; i++)
        {
            
            float angle = i * angleStep;
            Vector3 direction = Quaternion.AngleAxis(angle, transform.forward) * transform.right;

            RaycastHit2D hit = Physics2D.Raycast(objectToCastFrom.transform.position, direction, raycastDistance, groundLayer);

            bool hitFound = hit && hit.collider.gameObject == gameObject;
            Color c = Color.red;
            if (hit)
            {
                c = Color.yellow;

            }
            // Draw the rays up to the hit point
            if (hitFound)
            {
                c = Color.green;
                
                allHits.Add(new Vector2[] { edgeCollider.gameObject.transform.parent.InverseTransformPoint(hit.point), direction * raycastDistance });
            }
            //Debug.DrawRay(objectToCastFrom.transform.position, direction * raycastDistance, c, 5);

        }

        foreach (var point in allHits)
        {
            pointsList.Add(point[0]);
            pointsList.Add(point[1]);
        }

        points = pointsList.ToArray();
        edgeCollider.points = points;
    }






}
