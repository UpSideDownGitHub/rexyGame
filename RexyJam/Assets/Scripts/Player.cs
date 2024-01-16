using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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
    public float fireRate;
    private float _timeOfNextFire;

    [Header("Health")]
    public float maxHealth;
    public float curHealth;
    public Image healthImage;

    [Header("Score")]
    public TMP_Text scoreText;
    public TMP_Text multiplierText;
    public int score;
    public float multiplier;
    public float maxMultiplier;

    // INPUT
    private float _lookVecRex;
    private float _lookVecOther;
    private float _aimVecRex;
    private float _aimVecOther;
    private bool _thrust;
    private bool _fire;

    public void OnLeftStickRex(InputAction.CallbackContext ctx) => _lookVecRex = ctx.ReadValue<float>();
    public void OnLeftStickOther(InputAction.CallbackContext ctx) => _lookVecOther = ctx.ReadValue<float>();
    public void OnRightStickRex(InputAction.CallbackContext ctx) => _aimVecRex = ctx.ReadValue<float>();
    public void OnRightStickOther(InputAction.CallbackContext ctx) => _aimVecOther = ctx.ReadValue<float>();
    public void OnThrust(InputAction.CallbackContext ctx) => _thrust = ctx.action.WasPressedThisFrame();
    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
            _fire = true;
        else if (ctx.action.WasReleasedThisFrame())
            _fire = false;
    }

    public void Awake()
    {
        Physics2D.IgnoreLayerCollision(6, 7);
    }

    public void Start()
    {
        curHealth = maxHealth;
    }

    public void IncreaseMultiplier()
    {
        multiplier = multiplier + 0.1f >= maxMultiplier ? maxMultiplier : multiplier + 0.1f;
        multiplierText.text = "x " + multiplier;
    }

    public void IncreaseScore(int amount)
    {
        score += (int)(amount * multiplier);
        scoreText.text = score + " Pts";
    }

    public void ResetMultiplier()
    {
        multiplierText.text = "";
        multiplier = 1;
    }

    public void TakeDamage(float damage)
    {
        ResetMultiplier();
        curHealth = curHealth - damage <= 0 ? 0 : curHealth - damage;
        healthImage.fillAmount = curHealth / maxHealth;
        if (curHealth == 0)
        {
            PlayerPrefs.SetInt("Score", score);
        }
    }

    public void Update()
    {
        rb.angularVelocity = 0;

        if (_lookVecRex != 0)
        {
            var val = _lookVecRex;
            if (_lookVecRex < 0)
                val = -(1 - Mathf.Abs(_lookVecRex));
            else
                val = 1 - val;
            player.transform.Rotate(new Vector3(0, 0, -rotationSpeed * val));
        }
        else
        {
            player.transform.Rotate(new Vector3(0, 0, -rotationSpeed * _lookVecOther));
        }

        if (_aimVecRex != 0)
        {
            var val = _aimVecRex;
            if (_aimVecRex < 0)
                val = -(1 - Mathf.Abs(_aimVecRex));
            else
                val = 1 - val;
            gun.transform.Rotate(new Vector3(0, 0, -gunRotationSpeed * val));
        }
        else
        {
            gun.transform.Rotate(new Vector3(0, 0, -gunRotationSpeed * _aimVecOther));
        }

        if (_thrust)
            rb.AddForce(player.transform.up * thrustForce, ForceMode2D.Force);

        if (_fire && Time.time > _timeOfNextFire)
        {
            _timeOfNextFire = Time.time + fireRate;
            var rot = Quaternion.Euler(firePoint.transform.rotation.eulerAngles.x, firePoint.transform.rotation.eulerAngles.y, firePoint.transform.rotation.eulerAngles.z + 90);
            var tempBullet = Instantiate(bullet, firePoint.transform.position, rot);
            tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.right * fireForce);
            tempBullet.GetComponent<PlayerBullet>().player = this;
        }
    }
}
