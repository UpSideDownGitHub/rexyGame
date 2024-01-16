using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class PlayerBullet : MonoBehaviour
{
    [Header("Damage")]
    public float damage;
    public float timeAlive;

    [Header("Assist")]
    public float assist;
    public float assistArea = 20;
    public float viewAngle = 50;
    public GameObject closestEnemy;
    public Rigidbody2D rb;
    public float turnSpeed;
    public float movmentSpeed;

    [Header("Score")]
    public Player player;

    public void Start()
    {
        assist = PlayerPrefs.GetFloat("Assist", 1);
        if (assist != 0)
            closestEnemy = findClosestEnemy();
        Destroy(gameObject, timeAlive);
    }

    public void Update()
    {
        if (closestEnemy)
        {
            Vector2 direction = closestEnemy.transform.position - transform.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            rb.angularVelocity = -(turnSpeed * assist) * rotateAmount;
            rb.velocity = transform.up * movmentSpeed;
        }
    }

    public GameObject findClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, assistArea);
        float closest = float.MaxValue;
        GameObject closestObject = null;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].CompareTag("Enemy"))
            {
                Transform target = enemies[i].transform;
                Vector2 dirToTarget = (target.position - transform.position).normalized;
                if (Vector2.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float dist = Vector2.Distance(transform.position, target.position);
                    if (dist < closest)
                    {
                        closest = dist;
                        closestObject = target.gameObject;
                    }
                }
            }
        }
        return closestObject;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(damage);
            player.IncreaseMultiplier();
            player.IncreaseScore(enemy.Score);
            Destroy(gameObject);
        }
    }
}
