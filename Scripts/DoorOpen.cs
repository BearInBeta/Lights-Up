using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    [SerializeField] GameObject connectedKey;
    [SerializeField] float gateSpeed;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            GameObject player = collision.gameObject;
            for (int i = 0; i < player.transform.childCount; i++)
            {
                if (player.transform.GetChild(i).gameObject.Equals(connectedKey))
                {
                    Destroy(player.transform.GetChild(i).gameObject);
                    StartCoroutine(RaiseGate());
                }
            }
        }
    }

    IEnumerator RaiseGate()
    {
        for(int i = 0;i < 2/gateSpeed;i++)
        {
            transform.position = transform.position + new Vector3(0, gateSpeed);
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
}
