using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public enum EnemyType
{
    DRONE,
    SNIPER,
    BUG,
    KAMA,
    TANK
}

public class Enemy : MonoBehaviour
{
    //public NavMeshAgent agent;
    public GameObject target;
    public NavMeshAgent agent;
    public Rigidbody2D rb;

    public EnemyType type;

    //[Header("Rotation")]
    public float angleOffset;

    [Header("Health & Damage & Score")]
    public float maxHealth;
    public float curHealth;
    public float projectileDamage;
    public float touchDamage;
    public int Score = 100;
    public bool died = false;

    [Header("Pickups")]
    public GameObject[] pickups;
    [Range(0f, 1f)]
    public float powerupDropChance;

    [Header("Explosion Effect")]
    public GameObject explosionEffect;

    [Header("Shooting")]
    public GameObject firePoint;
    public GameObject bullet;
    public GameObject machineGunBullet;
    public float fireForce;
    public float attackTime;
    private float _timeOfNextAttack;
    public float attackTimeMachineGun;
    private float _timeOfNextAttackMachineGun;

    [Header("Rusher & Bug")]
    public float movmentSpeed;
    public float turnSpeed;

    [Header("Sniper & Tank")]
    public float stoppingDistance;
    public float minAttackDistance;
    public float maxAttackDistance;
    public float distanceToRetreat;
    public float retreatSpeed;
    public float suicideDistance;


    [Header("Kamakarzie")]
    public float rotationSpeed = 90f;
    public float lerpSpeed;
    public Vector3 offset;
    private Transform pivot;
    private Vector3 offsetDirection;
    public float distance;
    public float distanceDecreaseTime;
    public float distanceDecreaseAmount;
    private float _timeForNextDecrease;
    




    public void Awake()
    {
        if (type != EnemyType.BUG && type != EnemyType.KAMA && type != EnemyType.KAMA)
        {
            agent.updateUpAxis = false;
            agent.updateRotation = false;
        }
    }

    public void SetMultipliers(float health, float damage)
    {
        touchDamage *= damage;
        projectileDamage *= damage;
        maxHealth *= health;
        curHealth = maxHealth;
    }

    public void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");

        if (type == EnemyType.BUG)
        {
            Rotate(transform.position);
        }
        else if (type == EnemyType.KAMA)
        {
            pivot = target.transform;
            offsetDirection = transform.position - pivot.position;
            distance = offset.magnitude;
        }
    }

    public void TakeDamage(float damage)
    {
        curHealth = curHealth - damage <= 0 ? 0 : curHealth - damage;
        
        if (curHealth == 0 && !died)
        {
            died = true;
            try
            {
                GameObject.FindGameObjectWithTag("EnemySpawner").GetComponent<EnemyWaves>().EnemyDied();
            }
            catch { /*NOTHING*/ }

            if (damage != 9999)
            {
                Instantiate(pickups[0], transform.position, Quaternion.identity);
                if (Random.value > powerupDropChance)
                    Instantiate(pickups[Random.Range(1, pickups.Length)], transform.position, Quaternion.identity);
            }
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        switch (type)
        {
            case EnemyType.BUG:
                Bug();
                break;
            case EnemyType.DRONE:
                Drone();
                break;
            case EnemyType.KAMA:
                Kama();
                break;
            case EnemyType.SNIPER:
                Sniper();
                break;
            case EnemyType.TANK:
                Tank();
                break;
        }
    }

    public void Rotate(Vector3 pos)
    {
        var dir2 = pos - target.transform.position;
        var angle = Mathf.Atan2(dir2.y, dir2.x) * Mathf.Rad2Deg + angleOffset;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (type)
            {
                case EnemyType.BUG:
                    collision.gameObject.GetComponent<Player>().TakeDamage(touchDamage);
                    // PLAY BUG DEATH SOUND HERE
                    // SPAWN BUG DEATH PARTICLE HERE
                    TakeDamage(9999);
                    break;
                case EnemyType.DRONE:
                    // DO NOTHING
                    break;
                case EnemyType.KAMA:
                    collision.gameObject.GetComponent<Player>().TakeDamage(touchDamage);
                    // PLAY BUG DEATH SOUND HERE
                    // SPAWN BUG DEATH PARTICLE HERE
                    TakeDamage(9999);
                    break;
                case EnemyType.SNIPER:
                    // DO NOTHING
                    break;
                case EnemyType.TANK:
                    // DO NOTHING
                    break;
            }
        }
        else if (collision.gameObject.CompareTag("EnemyDeath"))
            TakeDamage(9999);
    }

    public void Bug()
    {
        Vector2 direction = target.transform.position - transform.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        rb.angularVelocity = -turnSpeed * rotateAmount;
        rb.velocity = transform.up * movmentSpeed;
    }

    public void Drone()
    {
        Rotate(transform.position);
        agent.SetDestination(target.transform.position);

        if (Time.time > _timeOfNextAttack)
        {
            _timeOfNextAttack = Time.time + attackTime;
            var rot = Quaternion.Euler(firePoint.transform.rotation.eulerAngles.x, firePoint.transform.rotation.eulerAngles.y, firePoint.transform.rotation.eulerAngles.z + 90);
            var tempBullet = Instantiate(bullet, firePoint.transform.position, rot);
            tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.right * fireForce);
            tempBullet.GetComponent<EnemyBullet>().damage = projectileDamage;
        }
    }

    public void Kama()
    {
        Rotate(transform.position);

        Quaternion rotate = Quaternion.Euler(0, 0, rotationSpeed * Time.deltaTime);
        offsetDirection = (rotate * offsetDirection).normalized;
        var pos = Vector3.Lerp(transform.position, pivot.position + offsetDirection * distance, lerpSpeed * Time.deltaTime);
        transform.position = pos;

        if (Time.time > _timeForNextDecrease)
        {
            _timeForNextDecrease = Time.time + distanceDecreaseTime;
            distance = distance - distanceDecreaseAmount <= 0 ? 0 : distance - distanceDecreaseAmount;
        }
    }

    public void Sniper()
    {
        var distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance < suicideDistance)
        {
            // SPAWN EXPLOSION PARTICLE
            target.GetComponent<Player>().TakeDamage(touchDamage);
            TakeDamage(9999);
        }
        else if (distance < distanceToRetreat)
        {
            // SPAWN WARNING EFFECT
            agent.stoppingDistance = 0;
            Vector3 dirToPlayer = transform.position - target.transform.position;
            Vector3 newPos = transform.position + dirToPlayer;
            Rotate(newPos);
            agent.SetDestination(newPos);
        }
        else
        {
            Rotate(transform.position);
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(target.transform.position);
            // check for stopping distance

            if (distance < maxAttackDistance && distance > minAttackDistance && Time.time > _timeOfNextAttack)
            {
                _timeOfNextAttack = Time.time + attackTime;
                // fire projectile at player
                var tempBullet = Instantiate(bullet, firePoint.transform.position, firePoint.transform.rotation);
                tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.right * fireForce);
                tempBullet.GetComponent<EnemyBullet>().damage = projectileDamage;
            }
        }
    }

    public void Tank()
    {
        var distance = Vector3.Distance(transform.position, target.transform.position);
        
        Rotate(transform.position);
        agent.SetDestination(target.transform.position);

        if (distance < distanceToRetreat)
        {
            if (Time.time > _timeOfNextAttackMachineGun)
            {
                _timeOfNextAttackMachineGun = Time.time + attackTimeMachineGun;
                // fire projectile at player
                var tempBullet = Instantiate(machineGunBullet, firePoint.transform.position, firePoint.transform.rotation);
                tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.right * fireForce);
                tempBullet.GetComponent<EnemyBullet>().damage = touchDamage;
            }
        }
        else
        {
            if (Time.time > _timeOfNextAttack)
            {
                _timeOfNextAttack = Time.time + attackTime;
                // fire projectile at player
                var tempBullet = Instantiate(bullet, firePoint.transform.position, firePoint.transform.rotation);
                tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.right * fireForce);
                tempBullet.GetComponent<EnemyBullet>().damage = projectileDamage;
            }
        }
    }


}
