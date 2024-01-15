using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class ScrollScript : MonoBehaviour
{
    public GameObject Cog;

    [Header("Movement")]
    public float rotationSpeed;

    [SerializeField] private float _lookVec;

    public void OnLeftStick(InputAction.CallbackContext ctx) => _lookVec = ctx.ReadValue<float>();


    void Update()
    {

            Cog.transform.Rotate(new Vector3(0, 0, -rotationSpeed * _lookVec));
     
    }
}
  
   

