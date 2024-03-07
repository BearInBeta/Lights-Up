using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    GameObject[] lights;
    [SerializeField] Color playerColor, backgroundColor, groundColor;
    [SerializeField] Color[] lightColors;
    [SerializeField] GameObject player;
    [SerializeField] GameObject background;
    [SerializeField] Material groundMaterial;
    [SerializeField] float totalIntensity;

    // Start is called before the first frame update
    void Start()
    {
        SetupStage();


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResetStage(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    void SetupStage()
    {
        int count = 0;
        lights = GameObject.FindGameObjectsWithTag("LightSource");
        Light2D globalLight = GameObject.FindGameObjectWithTag("GlobalLight").GetComponent<Light2D>();
        globalLight.intensity = totalIntensity / 10;
        foreach (var light in lights)
        {
            light.transform.GetComponentInChildren<Light2D>().intensity = (totalIntensity *9/10) / lights.Length;
            light.gameObject.GetComponent<SpriteRenderer>().color = lightColors[count];
            count = (count + 1) % lightColors.Length;
        }
        player.GetComponent<SpriteRenderer>().color = playerColor;
        background.GetComponent<SpriteRenderer>().color = backgroundColor;
        groundMaterial.color = groundColor;
    }
}
