using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameController : MonoBehaviour
{
    GameObject[] lights;
    [SerializeField] Color playerColor, backgroundColor, groundColor;
    [SerializeField] Color[] lightColors;
    [SerializeField] GameObject player;
    [SerializeField] GameObject background;
    [SerializeField] Material groundMaterial;

    // Start is called before the first frame update
    void Start()
    {
        int count = 0;
        lights = GameObject.FindGameObjectsWithTag("LightSource");
        foreach (var light in lights)
        {
            light.transform.GetComponentInChildren<Light2D>().intensity = 1.5f/lights.Length;
            light.gameObject.GetComponent<SpriteRenderer>().color = lightColors[count];
            count = (count + 1) % lightColors.Length;
        }
        player.GetComponent<SpriteRenderer>().color = playerColor;
        background.GetComponent<SpriteRenderer>().color = backgroundColor;
        groundMaterial.color = groundColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
