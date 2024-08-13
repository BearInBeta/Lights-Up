using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    GameObject[] lights, doors, grounds;
    [SerializeField] Color playerColor, backgroundColor, groundColor;
    [SerializeField] Color[] lightColors, doorColors;
    [SerializeField] GameObject player;
    [SerializeField] GameObject background;
    [SerializeField] Material groundMaterial;
    [SerializeField] float totalIntensity;
    // Start is called before the first frame update
    void Start()
    {
        int count = 0;
        lights = GameObject.FindGameObjectsWithTag("LightSource");
        foreach (var light in lights)
        {
            light.transform.GetComponent<Light2D>().intensity = totalIntensity / lights.Length;
            light.gameObject.transform.parent.gameObject.GetComponent<SpriteRenderer>().color = lightColors[count];
            count = (count + 1) % lightColors.Length;
        }

        doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (var door in doors)
        {
            door.gameObject.GetComponent<SpriteRenderer>().color = doorColors[count];
            count = (count + 1) % lightColors.Length;
        }

        grounds = GameObject.FindGameObjectsWithTag("Ground");
        foreach (var ground in grounds)
        {
            ground.gameObject.GetComponent<SpriteRenderer>().color = groundColor;
        }

        player.GetComponent<SpriteRenderer>().color = playerColor;
        background.GetComponent<SpriteRenderer>().color = backgroundColor;
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
}