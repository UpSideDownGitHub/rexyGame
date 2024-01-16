using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class ScrollScript : MonoBehaviour
{
    public GameObject Cog;
    public SelectedButton buttonScript;

    [Header("Movement")]
    public float rotationSpeed;

    [SerializeField] private float _lookVec;
    [SerializeField] private float _lookVecRex;

    public void OnLeftStickRexy(InputAction.CallbackContext ctx) => _lookVecRex = ctx.ReadValue<float>();
    public void OnLeftStickOther(InputAction.CallbackContext ctx) => _lookVec = ctx.ReadValue<float>();


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
        
    }

    public void SelectPressed()
    {
        buttonScript.ConfirmPressed();
    }
}
  
   

