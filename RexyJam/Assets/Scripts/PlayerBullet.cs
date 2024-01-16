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

    [Header("Score")]
    public Player player;

    public void Start()
    {
        Destroy(gameObject, timeAlive);
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
