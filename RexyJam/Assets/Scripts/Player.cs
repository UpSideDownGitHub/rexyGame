using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float movementSpeed;
    public bool thrust;
    public float thrustForce;

    private Vector2 _movementVec;

    public void OnMovement(InputAction.CallbackContext ctx) => _movementVec = ctx.ReadValue<Vector2>();
}
