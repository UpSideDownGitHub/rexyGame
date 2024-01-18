using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
    public GameObject loadingScreenObject;
    public void OnButtonPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
        {
            loadingScreenObject.SetActive(true);
            SceneManager.LoadSceneAsync("MainMenu");
        }
    }
}
