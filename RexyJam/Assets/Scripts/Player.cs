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
    private float _lookVec;
    private float _aimVec;
    private bool _thrust;
    private bool _fire;

    public void OnLeftStick(InputAction.CallbackContext ctx) => _lookVec = ctx.ReadValue<float>();
    public void OnRightStick(InputAction.CallbackContext ctx) => _aimVec = ctx.ReadValue<float>();
    public void OnThrust(InputAction.CallbackContext ctx) => _thrust = ctx.action.WasPressedThisFrame();
    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
            _fire = true;
        else if (ctx.action.WasReleasedThisFrame())
            _fire = false;
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

        if (_lookVec != 0)
        {
            var val = _lookVec;
            if (_lookVec < 0)
                val = -(1 - Mathf.Abs(_lookVec));
            else
                val = 1 - val;
            player.transform.Rotate(new Vector3(0, 0, -rotationSpeed * val));
        }

        if (_aimVec != 0)
        {
            var val = _aimVec;
            if (_aimVec < 0)
                val = -(1 - Mathf.Abs(_aimVec));
            else
                val = 1 - val;
            gun.transform.Rotate(new Vector3(0, 0, -gunRotationSpeed * val));
        }

        if (_thrust)
            rb.AddForce(player.transform.up * thrustForce, ForceMode2D.Force);
        else
            //rb.velocity = Vector2.zero;

        if (_fire && Time.time > _timeOfNextFire)
        {
            _timeOfNextFire = Time.time + fireRate;
            var tempBullet = Instantiate(bullet, firePoint.transform.position, firePoint.transform.rotation);
            tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.up * fireForce);
            tempBullet.GetComponent<PlayerBullet>().player = this;
        }
    }
}
