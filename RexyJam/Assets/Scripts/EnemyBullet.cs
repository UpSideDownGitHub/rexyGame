using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class EnemyBullet : MonoBehaviour
{
    [Header("Damage")]
    public float damage;
    public float timeAlive;

    public void Start()
    {
        Destroy(gameObject, timeAlive);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
