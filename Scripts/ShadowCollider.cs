using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class ShadowCollider : MonoBehaviour
{
    public GameObject[] objectsToCastFrom;
    public LayerMask groundLayer; // Layer mask for ground objects
    public GameObject shadowCollider;
    Vector2[] lastPositions;
    Dictionary<int, EdgeCollider2D[]> shadowColliders;

    private void Start()
    {
        objectsToCastFrom = GameObject.FindGameObjectsWithTag("LightSource");

        shadowColliders = new Dictionary<int, EdgeCollider2D[]>();
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
        
        for (int i = 0; i < objectsToCastFrom.Length; i++)
        {
            GameObject obj = objectsToCastFrom[i];
            
            
            if (obj.activeInHierarchy)
            {
               
                ShadeCollider(obj);
                lastPositions[i] = (Vector2)obj.transform.position;
            }
            else
            {
                shadowColliders.Remove(obj.GetInstanceID());
            }
        }
    }
    void ShadeCollider(GameObject objectToCastFrom)
    {

        float radius = objectToCastFrom.transform.GetComponentInChildren<Light2D>().pointLightOuterRadius;



        Vector2[] corners = new Vector2[4];

        if (!shadowColliders.ContainsKey(objectToCastFrom.GetInstanceID()))
        {
            shadowColliders.Add(objectToCastFrom.GetInstanceID(), new EdgeCollider2D[corners.Length]);
        }

        corners[0] = new Vector2(transform.position.x + transform.lossyScale.x/2, transform.position.y + transform.lossyScale.y/2);
        corners[1] = new Vector2(transform.position.x - transform.lossyScale.x/2, transform.position.y + transform.lossyScale.y/2);
        corners[2] = new Vector2(transform.position.x + transform.lossyScale.x/2, transform.position.y - transform.lossyScale.y/2);
        corners[3] = new Vector2(transform.position.x - transform.lossyScale.x/2, transform.position.y - transform.lossyScale.y/2);

        int count = 0;
        foreach (var cor in corners)
        {
            List<Vector2> pointsList = new List<Vector2>();

          

            
            Vector2 origin = objectToCastFrom.transform.position;
            float distance;

            Vector2 direction = new Vector2(cor.x - origin.x, cor.y - origin.y);
            Vector2 unit = direction.normalized;

            RaycastHit2D hitDistance = Physics2D.Raycast(cor + unit, direction, radius, groundLayer);
            
            if (hitDistance)
            {
                
                distance = Mathf.Min(Mathf.Abs(Vector2.Distance(cor, hitDistance.point)), radius - Mathf.Abs(Vector2.Distance(origin, cor)));
            }
            else
            {
                distance = radius - Mathf.Abs(Vector2.Distance(origin, cor));
            }
            


            float newX = cor.x + distance * unit.x;
            float newY = cor.y + distance * unit.y;


          


            pointsList.Add(gameObject.transform.InverseTransformPoint(cor));
            Vector2 newPoint = gameObject.transform.InverseTransformPoint(new Vector2(newX, newY));
            RaycastHit2D hit = Physics2D.Raycast(origin, cor - origin, radius, groundLayer);
            RaycastHit2D hitSelf = Physics2D.Raycast(new Vector2(newX, newY), cor - new Vector2(newX, newY), distance, groundLayer);
            
            if (shadowColliders[objectToCastFrom.GetInstanceID()][count] == null)
            {
                shadowColliders[objectToCastFrom.GetInstanceID()][count] = shadowCollider.AddComponent<EdgeCollider2D>();

            }

            if (distance < 0 || (hit && Mathf.Abs(Vector2.Distance(origin, hit.point)) < Mathf.Abs(Vector2.Distance(origin, cor) - 0.01f)) || (hitSelf && hitSelf.collider.gameObject == gameObject && Mathf.Abs(Vector2.Distance(origin, hitSelf.point)) > Mathf.Abs(Vector2.Distance(origin, cor) + 0.01f)))
            {
                
                shadowColliders[objectToCastFrom.GetInstanceID()][count].isTrigger = true;

            }
            else
            {
                shadowColliders[objectToCastFrom.GetInstanceID()][count].isTrigger = false;

            }

            if(hitSelf && hitSelf.collider.gameObject != gameObject)
            {
                newPoint = gameObject.transform.InverseTransformPoint(hitSelf.point);
            }

            pointsList.Add(newPoint);


            shadowColliders[objectToCastFrom.GetInstanceID()][count].points = pointsList.ToArray();
            count++;

        }
        
    }
    






}
