using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public enum EnemyType
{
    DRONE,
    SNIPER,
    RUSHER,
    KAMA
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
    

    public void Awake()
    {
        if (type != EnemyType.RUSHER && type != EnemyType.KAMA)
        {
            agent.updateUpAxis = false;
            agent.updateRotation = false;
        }
    }

    public void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");

        if (type == EnemyType.RUSHER)
        {
            Rotate(transform.position);
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
                tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.up * fireForce);
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
            Vector3 targetDir = target.transform.position - transform.position;
            Quaternion lookDir = Quaternion.LookRotation(targetDir);
            var tempBullet = Instantiate(bullet, firePoint.transform.position, lookDir);
            tempBullet.GetComponent<Rigidbody2D>().AddForce(tempBullet.transform.up * fireForce);
        }
    }
}
