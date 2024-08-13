using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstaDeath : MonoBehaviour
{
    [SerializeField] GameObject deathPrefab;
    [SerializeField] bool exact = false;

    // Start is called before the first frame update
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(!exact)
                Instantiate(deathPrefab, new Vector3(collision.transform.position.x, transform.position.y + transform.lossyScale.y/2, 0.5f), Quaternion.identity);
            else
                Instantiate(deathPrefab, new Vector3(collision.transform.position.x, transform.position.y, 0.5f), Quaternion.identity);

            collision.gameObject.SetActive(false);
        }
    }
}
