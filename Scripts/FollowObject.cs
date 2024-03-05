using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform targetObject; // The object to follow
    public float distance = 3f; // The desired distance between the objects
    public Vector2 direction = Vector2.right; // The direction in which the object should follow

    private void Update()
    {
        // Check if the target object is assigned
        if (targetObject != null)
        {
            // Calculate the desired position based on the target object's position, distance, and direction
            Vector3 desiredPosition = targetObject.position + (Vector3)direction * distance;

            // Move the current object to the desired position
            transform.position = desiredPosition;
        }
    }
}

