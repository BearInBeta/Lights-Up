using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ShadowCollider : MonoBehaviour
{
    public GameObject[] objectsToCastFrom;
    float raycastDistance = 100f; // Maximum distance to cast the rays
    public LayerMask groundLayer; // Layer mask for ground objects
    public EdgeCollider2D edgeCollider;
    Vector2[] lastPositions;

    private void Start()
    {
        objectsToCastFrom = GameObject.FindGameObjectsWithTag("LightSource");
        lastPositions = new Vector2[objectsToCastFrom.Length];
        for(int i = 0; i < objectsToCastFrom.Length; i++)
        {
            lastPositions[i] = objectsToCastFrom[i].transform.position;
        }
        CheckLights();

    }
    private void FixedUpdate()
    {
    }

    private void Update()
    {
        CheckLights();
    }
    private void CheckLights()
    {
        Vector2[] points = new Vector2[0];
        for (int i = 0; i < objectsToCastFrom.Length; i++)
        {
            GameObject obj = objectsToCastFrom[i];
                        
            if (obj.activeInHierarchy)
            {
                points = points.Concat(ShadeCollider(obj)).ToArray();
                lastPositions[i] = (Vector2)obj.transform.position;

            }
        }

        edgeCollider.points = points;
    }
    Vector2[] ShadeCollider(GameObject objectToCastFrom)
    {
        

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
        return pointsList.ToArray();
        
    }
    






}
