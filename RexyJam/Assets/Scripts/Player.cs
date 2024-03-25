using JetBrains.Annotations;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
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
    private float _actualFireRate;
    private float _timeOfNextFire;

    [Header("Health")]
    public float maxHealth;
    public float curHealth;
    public GameObject deathObject;

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
     * 1 => Implosion - green
     * 2 => Triple Shot - pink
     * 3 => Circle Shot - blue
     * 4 => Peirce Shot - light blue
     * 5 => Ricochet Shot - yellow
     * 6 => Super Fire - orange
     * 7 => minion - red
    */
    public PowerupInfo[] powerups;
    public float implosionDamage;
    public float implosionArea;
    public float healthIncreaseAmount;
    public float tripleShotAngle;
    public float circleBulletCount;
    public float superFireRate;
    private float _actualSuperFireRate;
    public GameObject minion;
    public float angleOffset = -90;
    public float minionFireRate;
    private float _timeOfNextMinionShot;
    public GameObject explosionEffect;

    [Range(0f,1f)]
    public float lightingDecreaseAmount;

    [Header("UI")]
    public HealthGaugeFunctions healthGaugeFunctions;

    ProjectilePool projPool;

    [Header("Thruster Effect")]
    public GameObject thrusterOn;
    public GameObject thrusterOff;

    [Header("Sounds")]
    public AudioSource source;
    public float fireVolume;
    public AudioClip fire;
    public float powerupVoluem;
    public AudioClip powerup;
    public float implosionVolume;
    public AudioClip implosion;
    public float deathVolume;
    public AudioClip death;
    public float healthPickupVolume;
    public AudioClip healthPickup;

    [Header("Thruster")]
    public AudioSource thruster;
    public float thrusterOffVolume;
    public AudioClip thrusterOffSound;

    // INPUT
    private float _lookVecRex;
    private float _lookVecOther;
    private float _aimVecRex;
    private float _aimVecOther;
    private bool _thrust;
    private bool _fire;


    private bool _quit;
    public float quitHoldTime;
    private float _timeOfPress;
    private float _timeOfQuit;

    public GameObject quittingObject;
    public Slider quitSlider;

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

    public void OnQuit(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPressedThisFrame())
        {
            quittingObject.SetActive(true);
            _quit = true;
            _timeOfQuit = Time.time + quitHoldTime;
            _timeOfPress = Time.time;
        }
        else if (ctx.action.WasReleasedThisFrame())
        {
            quittingObject.SetActive(false);
            _quit = false;
        }
    }

    public void Awake()
    {
        Physics2D.IgnoreLayerCollision(6, 7);
        Physics2D.IgnoreLayerCollision(9, 7);
        Physics2D.IgnoreLayerCollision(6, 8);
        Physics2D.IgnoreLayerCollision(10, 10);
        Physics2D.IgnoreLayerCollision(9, 9);

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Start()
    {
        curHealth = maxHealth;
        projPool = ProjectilePool.instance;
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

        healthGaugeFunctions.Bobble();

        ResetMultiplier();
        curHealth = curHealth - damage <= 0 ? 0 : curHealth - damage;
        healthGaugeFunctions.CheckHealth(curHealth, maxHealth);

        iFrameActive = true;
        _timeToDisableIFrame = Time.time + iFrameTime;


        if (curHealth == 0)
        {
            PlayerPrefs.SetInt("Score", score);
            source.PlayOneShot(death, deathVolume);
            deathObject.SetActive(true);
        }
    }

    public void Update()
    {
        // quitting logic
        if (_quit)
        {
            // fill the slider over time
            quitSlider.value = (Time.time - _timeOfPress) / (_timeOfQuit - _timeOfPress);

            // if the final time is reached then quit the game
            if (Time.time > _timeOfQuit)
                SceneManager.LoadScene("MainMenu");
        }



        // powerups
        _actualSuperFireRate = superFireRate;
        _actualFireRate = fireRate;
        for (int i = 0; i < powerups.Length; i++)
        {
            if (powerups[i].enabled)
            {
                _actualSuperFireRate /= lightingDecreaseAmount;
                _actualFireRate /= lightingDecreaseAmount;

                if (Time.time > powerups[i].timeToDisable)
                {
                    powerups[i].enabled = false;
                    healthGaugeFunctions.SetPowerUpUI(i, false);
                }
            }
        }

        // I Frames
        if (Time.time > _timeToDisableIFrame && iFrameActive)
        {
            iFrameActive = false;
        }

        // Minion Powerup
        if (powerups[7].enabled)
        {
            if (Time.time > _timeOfNextMinionShot)
            {
                _timeOfNextMinionShot = Time.time + minionFireRate;
                FireTurrentBullet();
            }
        }
        else
        {
            minion.SetActive(false);
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
            player.transform.Rotate(new Vector3(0, 0, -rotationSpeed * val * Time.deltaTime));
        }
        else
        {
            player.transform.Rotate(new Vector3(0, 0, -rotationSpeed * _lookVecOther * Time.deltaTime));
        }

        if (_aimVecRex != 0)
        {
            var val = _aimVecRex;
            if (_aimVecRex < 0)
                val = -(1 - Mathf.Abs(_aimVecRex));
            else
                val = 1 - val;
            gun.transform.Rotate(new Vector3(0, 0, gunRotationSpeed * val * Time.deltaTime));
        }
        else
        {
            gun.transform.Rotate(new Vector3(0, 0, gunRotationSpeed * _aimVecOther * Time.deltaTime));
        }


        if (_thrust)
        {
            if (thrusterOff.activeInHierarchy)
                thruster.Play();
            thrusterOn.SetActive(true);
            thrusterOff.SetActive(false);
            rb.AddForce(player.transform.up * thrustForce * Time.deltaTime, ForceMode2D.Force);
        }
        else
        {
            if (thrusterOn.activeInHierarchy)
            {
                thruster.Stop();
                source.PlayOneShot(thrusterOffSound, thrusterOffVolume); 
            }
            thrusterOn.SetActive(false);
            thrusterOff.SetActive(true);
        }


        if (_fire && Time.time > _timeOfNextFire)
        {
            if (powerups[6].enabled) // Super Shot
                _timeOfNextFire = Time.time + _actualSuperFireRate;
            else
                _timeOfNextFire = Time.time + _actualFireRate;

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

    public void FireTurrentBullet()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length != 0)
        {
            int ran = Random.Range(0, enemies.Length);
            var dir2 = minion.transform.position - enemies[ran].transform.position;
            var angle = Mathf.Atan2(dir2.y, dir2.x) * Mathf.Rad2Deg + angleOffset;
            var rot = Quaternion.AngleAxis(angle, Vector3.forward);
            var tempBullet = projPool.SpawnProjectile(0, minion.transform.position, rot);
            if (tempBullet)
            {
                tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.right * fireForce);
                tempBullet.GetComponent<PlayerBullet>().SetValues(false, false, this, fireForce);
            }
            else
                Debug.LogError("Not Enough Bullets For Player To Spawn");
        }
    }

    public void SpawnBullet(Quaternion rot)
    {
        source.PlayOneShot(fire, fireVolume);

        if (powerups[2].enabled) // Triple Shot
        {
            for (int i = -1; i < 2; i++)
            {
                var newRot = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, rot.eulerAngles.z + i * tripleShotAngle);
                var tempBullet = projPool.SpawnProjectile(0, firePoint.transform.position, newRot);
                if (tempBullet)
                {
                    tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.right * fireForce);
                    tempBullet.GetComponent<PlayerBullet>().SetValues(powerups[4].enabled, powerups[5].enabled, this, fireForce);
                }
                else
                    Debug.LogError("Not Enough Bullets For Player To Spawn");
            }
        }
        else
        {
            var tempBullet = projPool.SpawnProjectile(0, firePoint.transform.position, rot);
            if (tempBullet)
            {
                tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.right * fireForce);
                tempBullet.GetComponent<PlayerBullet>().SetValues(powerups[4].enabled, powerups[5].enabled, this, fireForce);
            }
            else
                Debug.LogError("Not Enough Bullets For Player To Spawn");
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
        if (powerupID == 0)
        {
            source.PlayOneShot(healthPickup, healthPickupVolume);
            curHealth = curHealth + healthIncreaseAmount > maxHealth ? maxHealth : curHealth + healthIncreaseAmount;
            healthGaugeFunctions.CheckHealth(curHealth, maxHealth);
            return;
        }
        else if (powerupID == 1) // implosion
        {
            source.PlayOneShot(implosion, implosionVolume);
            // spawn implosion effect
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            healthGaugeFunctions.bobbleHeadAnimator.SetTrigger("Shake");
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, implosionArea);
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].CompareTag("Enemy"))
                    enemies[i].GetComponent<Enemy>().TakeDamage(999);
            }
        }
        else if (powerupID == 7)
        {
            minion.SetActive(true);
        }
        source.PlayOneShot(powerup, powerupVoluem);
        healthGaugeFunctions.SetPowerUpUI(powerupID, true);
        powerups[powerupID].enabled = true;
        powerups[powerupID].timeToDisable = Time.time + powerups[powerupID].powerupLength;
    }
}
