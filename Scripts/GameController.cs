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
        
   
    }
}
