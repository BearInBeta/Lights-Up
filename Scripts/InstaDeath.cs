using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstaDeath : MonoBehaviour
{
    [SerializeField] GameObject deathPrefab;

    // Start is called before the first frame update
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Instantiate(deathPrefab, new Vector3(collision.transform.position.x, transform.position.y + transform.lossyScale.y/2, 0.5f), Quaternion.identity);
            collision.gameObject.SetActive(false);
        }
    }
}
