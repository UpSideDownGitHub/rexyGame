using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("General")]
    public Rigidbody2D rb;
    public GameObject player;
    public GameObject gun;
    public GameObject firePoint;

    [Header("Movement")]
    public float rotationSpeed;
    public float gunRotationSpeed;
    public float thrustForce;

    [Header("Shooting")]
    public GameObject bullet;
    public float fireForce;


    // INPUT
    [SerializeField]private float _lookVec;
    [SerializeField] private float _aimVec;
    [SerializeField] private bool _thrust;
    [SerializeField] private bool _fire;

    public void OnLeftStick(InputAction.CallbackContext ctx) => _lookVec = ctx.ReadValue<float>();
    public void OnRightStick(InputAction.CallbackContext ctx) => _aimVec = ctx.ReadValue<float>();
    public void OnThrust(InputAction.CallbackContext ctx) => _thrust = ctx.action.WasPressedThisFrame();
    public void OnFire(InputAction.CallbackContext ctx) => _fire = ctx.action.WasPressedThisFrame();

    public void Update()
    {
        rb.angularVelocity = 0;
        player.transform.Rotate(new Vector3(0,0, -rotationSpeed * _lookVec));
        gun.transform.Rotate(new Vector3(0, 0, -gunRotationSpeed * _aimVec));

        if (_thrust)
        {
            rb.AddForce(player.transform.up * thrustForce); 
        }

        if (_fire)
        {
            _fire = false;
            var tempBullet = Instantiate(bullet, firePoint.transform.position, firePoint.transform.rotation);
            tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.up * fireForce);
        }
    }
}
