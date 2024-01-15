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
    RUSHER,
    KAMA,
    CIRCLE
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

    [Header("Projectiles")]
    public GameObject firePoint;
    public GameObject bullet;
    public float fireForce;

    [Header("Attacking")]
    public float attackTime;
    private float _timeOfNextAttack;

    [Header("Sniper")]
    public float stoppingDistance;
    public float minAttackDistance;
    public float maxAttackDistance;
    public float distanceToRetreat;
    public float retreatSpeed;
    public float meleeDistance;
    public float meleeTime;

    [Header("Rusher")]
    public float movmentSpeed;
    public float turnSpeed;

    [Header("Circle")]
    public float rotationSpeed = 90f;
    public float lerpSpeed;
    public Vector3 offset;
    private Transform pivot;
    private Vector3 offsetDirection;
    private float distance;
    public bool died = false;



    public void Awake()
    {
        if (type != EnemyType.RUSHER && type != EnemyType.KAMA && type != EnemyType.CIRCLE)
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

        if (type == EnemyType.RUSHER)
        {
            Rotate(transform.position);
        }
        else if (type == EnemyType.CIRCLE)
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
            GameObject.FindGameObjectWithTag("EnemySpawner").GetComponent<EnemyWaves>().EnemyDied();
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        switch (type)
        {
            case EnemyType.DRONE:
                Drone();
                break;
            case EnemyType.SNIPER:
                Sniper();
                break;
             case EnemyType.RUSHER:
                Rusher();
                break;
            case EnemyType.KAMA:
                Kama();
                break;
            case EnemyType.CIRCLE:
                Circle();
                break;
        }
        
    }

    public void Rotate(Vector3 pos)
    {
        var dir2 = pos - target.transform.position;
        var angle = Mathf.Atan2(dir2.y, dir2.x) * Mathf.Rad2Deg + angleOffset;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void Drone()
    {
        Rotate(transform.position);
        agent.SetDestination(target.transform.position);
    }

    public void Sniper()
    {
        var distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance < distanceToRetreat)
        {
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
            }
        }
    }

    public void Rusher()
    {
        Vector2 direction = target.transform.position - transform.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        rb.angularVelocity = -turnSpeed * rotateAmount;
        rb.velocity = transform.up * movmentSpeed;
    }

    public void Kama()
    {
        Vector2 direction = target.transform.position - transform.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        rb.angularVelocity = -turnSpeed * rotateAmount;
        rb.velocity = transform.up * movmentSpeed;

        if (Time.time > _timeOfNextAttack)
        {
            _timeOfNextAttack = Time.time + attackTime;
            // fire projectile at player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.Euler(0f, 0f, angle);
            var tempBullet = Instantiate(bullet, firePoint.transform.position, q);
            tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.right * fireForce);
        }
    }

    public void Circle()
    {
        Rotate(transform.position);

        Quaternion rotate = Quaternion.Euler(0, 0, rotationSpeed * Time.deltaTime);
        offsetDirection = (rotate * offsetDirection).normalized;
        var pos = Vector3.Lerp(transform.position, pivot.position + offsetDirection * distance, lerpSpeed * Time.deltaTime);
        transform.position = pos;

        if (Time.time > _timeOfNextAttack)
        {
            _timeOfNextAttack = Time.time + attackTime;
            // fire projectile at player
            var tempBullet = Instantiate(bullet, firePoint.transform.position, transform.rotation);
            tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.right * fireForce);
        }
    }
}
