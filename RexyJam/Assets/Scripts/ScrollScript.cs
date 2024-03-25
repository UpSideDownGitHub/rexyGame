using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ScrollScript : MonoBehaviour
{
    public GameObject Cog;

    public GameObject loadingScreen;

    [Header("Movement")]
    public float rotationSpeed;

    public float zRotation;
    public float currentMin;
    public float currentMax;

    private float _lookVec;
    private float _lookVecRex;

    public void OnLeftStickRexy(InputAction.CallbackContext ctx) => _lookVecRex = ctx.ReadValue<float>();
    public void OnLeftStickOther(InputAction.CallbackContext ctx) => _lookVec = ctx.ReadValue<float>();
    public void OnSelect(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPerformedThisFrame())
            SelectPressed();
    }

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (_lookVecRex != 0)
        {
            var val = _lookVecRex;
            if (_lookVecRex < 0)
                val = -(1 - Mathf.Abs(_lookVecRex));
            else
                val = 1 - val;
            Cog.transform.Rotate(new Vector3(0, 0, -rotationSpeed * val));
        }
        else
        {
            Cog.transform.Rotate(new Vector3(0, 0, -rotationSpeed * _lookVec));
        }

        zRotation = Cog.transform.rotation.eulerAngles.z;
    }

    public void SelectPressed()
    {
        for (int i = 0; i < 5; i++)
        {
            currentMin = (360f / 5f * i) - 360f / 10f;
            currentMax = (360f / 5f * i) + 360f / 10f;
            if (Cog.transform.rotation.eulerAngles.z >= (360f / 5f * i) - 360f / 10f &&
                Cog.transform.rotation.eulerAngles.z <= (360f / 5f * i) + 360f / 10f)
            {
                currentMin = (360f / 5f * i) - 360f / 10f;
                currentMax = (360f / 5f * i) + 360f / 10f;
                ConfirmPressed(i);
                break;
            }

            if (currentMin == 252)
                ConfirmPressed(0);

        }
    }

    public void ConfirmPressed(int selected)
    {
        if (selected == 0) // play
        {
            loadingScreen.SetActive(true);
            SceneManager.LoadSceneAsync("Main");
        }
        else if (selected == 1) // Credits
        {
            loadingScreen.SetActive(true);
            SceneManager.LoadSceneAsync("Credits");
        }
        else if (selected == 2) // quit
        {
            loadingScreen.SetActive(true);
            Application.Quit();
        }
        else if (selected == 3) // controls
        {
            loadingScreen.SetActive(true);
            SceneManager.LoadSceneAsync("Controls");
        }
        else if (selected == 4) // leaderboard
        {
            loadingScreen.SetActive(true);
            SceneManager.LoadSceneAsync("Leaderboard");
        }
    }
}
  
   

