using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

[Serializable]
public struct PowerupInfo
{
    public string name;
    public bool enabled;
    public float powerupLength;
    [HideInInspector] public float timeToDisable;
}
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

    [Header("I Frames")]
    public bool iFrameActive;
    public float iFrameTime;
    private float _timeToDisableIFrame;

    [Header("Score")]
    public int score;
    public float multiplier;
    public float maxMultiplier;

    [Header("Powerups")]
    /*
     * 0 => Health
     * 1 => Implosion 
     * 2 => Triple Shot 
     * 3 => Circle Shot
     * 4 => Bounce Shot
     * 5 => Peirce Shot
     * 6 => Ricochet Shot
     * 7 => Super Fire
    */
    public PowerupInfo[] powerups;
    public float implosionDamage;
    public float implosionArea;
    public float healthIncreaseAmount;
    public float tripleShotAngle;
    public float circleBulletCount;
    public float superFireRate;

    [Header("UI")]
    public HealthGaugeFunctions healthGaugeFunctions;

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
        Physics2D.IgnoreLayerCollision(6, 8);
    }

    public void Start()
    {
        curHealth = maxHealth;
    }

    public void IncreaseMultiplier()
    {
        multiplier = multiplier + 0.1f >= maxMultiplier ? maxMultiplier : multiplier + 0.1f;
        healthGaugeFunctions.SetMultiplierBulbs(true);
        healthGaugeFunctions.SetMultiplierUI(multiplier);
    }

    public void IncreaseScore(int amount)
    {
        score += (int)(amount * multiplier);
        healthGaugeFunctions.SetScoreUI(score);
    }

    public void ResetMultiplier()
    {
        multiplier = 1;
        healthGaugeFunctions.SetMultiplierBulbs(false);
        healthGaugeFunctions.SetMultiplierUI(multiplier);
    }

    public void TakeDamage(float damage)
    {
        if (iFrameActive)
            return;

        if (!powerups[0].enabled)
        {
            ResetMultiplier();
            curHealth = curHealth - damage <= 0 ? 0 : curHealth - damage;
            healthGaugeFunctions.CheckHealth(curHealth, maxHealth);

            iFrameActive = true;
            _timeToDisableIFrame = Time.time + iFrameTime;


            if (curHealth == 0)
            {
                PlayerPrefs.SetInt("Score", score);
                SceneManager.LoadSceneAsync("EndScreen");
            }
        }
    }

    public void Update()
    {
        //// powerups
        //for (int i = 0; i < powerups.Length; i++)
        //{
        //    if (powerups[i].enabled)
        //    {
        //        if (Time.time > powerups[i].timeToDisable)
        //        {
        //            powerups[i].enabled = false;
        //        }
        //    }
        //}

        // I Frames
        if (Time.time > _timeToDisableIFrame && iFrameActive)
        {
            iFrameActive = false;
        }

        // movement
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
            gun.transform.Rotate(new Vector3(0, 0, gunRotationSpeed * val));
        }
        else
        {
            gun.transform.Rotate(new Vector3(0, 0, gunRotationSpeed * _aimVecOther));
        }


        if (_thrust)
            rb.AddForce(player.transform.up * thrustForce, ForceMode2D.Force);


        if (_fire && Time.time > _timeOfNextFire)
        {
            if (powerups[7].enabled) // Super Shot
                _timeOfNextFire = Time.time + superFireRate;
            else
                _timeOfNextFire = Time.time + fireRate;

            if (powerups[3].enabled) // circle
            {
                // spawn the bullets in a circle around
                for (int i = 0; i < circleBulletCount; i++)
                {
                    var addedRot = 360 / circleBulletCount * i;
                    SpawnBullet(Quaternion.Euler(firePoint.transform.rotation.eulerAngles.x, firePoint.transform.rotation.eulerAngles.y, firePoint.transform.rotation.eulerAngles.z + 90 + addedRot));
                }
            }
            else
            {
                SpawnBullet(Quaternion.Euler(firePoint.transform.rotation.eulerAngles.x, firePoint.transform.rotation.eulerAngles.y, firePoint.transform.rotation.eulerAngles.z + 90));
            }
        }
    }

    public void SpawnBullet(Quaternion rot)
    {
        if (powerups[2].enabled) // Triple Shot
        {
            for (int i = -1; i < 2; i++)
            {
                var newRot = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, rot.eulerAngles.z + i * tripleShotAngle);
                var tempBullet = Instantiate(bullet, firePoint.transform.position, newRot);
                tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.right * fireForce);
                tempBullet.GetComponent<PlayerBullet>().SetValues(powerups[4].enabled, powerups[5].enabled, powerups[6].enabled, this, fireForce);
            }
        }
        else
        {
            var tempBullet = Instantiate(bullet, firePoint.transform.position, rot);
            tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.right * fireForce);
            tempBullet.GetComponent<PlayerBullet>().SetValues(powerups[4].enabled, powerups[5].enabled, powerups[6].enabled, this, fireForce);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PowerUp"))
        {
            SetpowerUp(collision.GetComponent<Powerup>().powerupID);
            Destroy(collision.gameObject);
        }
    }

    public void SetpowerUp(int powerupID)
    {
        if (powerupID == 2) // implosion
        {
            // spawn implosion effect
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, implosionArea);
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].CompareTag("Enemy"))
                    enemies[i].GetComponent<Enemy>().TakeDamage(implosionDamage);
            }
            return;
        }
        else if (powerupID == 3)
        {
            curHealth = curHealth + healthIncreaseAmount > maxHealth ? maxHealth : curHealth + healthIncreaseAmount;
            healthGaugeFunctions.CheckHealth(curHealth, maxHealth);
        }
        powerups[powerupID].enabled = true;
        powerups[powerupID].timeToDisable = Time.time + powerups[powerupID].powerupLength;
    }
}
