using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI.Table;

public class PlayerBullet : MonoBehaviour
{
    [Header("Damage")]
    public float damage;
    public float timeAlive;

    [Header("Score")]
    public Player player;

    [Header("Powerups")]
    public bool peirce;
    public bool ricochet;
    public float ricochetSearchRadius;
    public float angleOffset = 90;
    public float redirectForce;

    public Proj proj;

    public void SetValues(bool peirce, bool ricochet, Player player, float redirectForce)
    {
        this.peirce = peirce;
        this.ricochet = ricochet;
        this.player = player;
        this.redirectForce = redirectForce;
    }

    public void Start()
    {
        Invoke("customDestroy", timeAlive);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(damage);
            player.IncreaseMultiplier();
            player.IncreaseScore(enemy.Score);

            if (ricochet)
                Ricochet();
            else if (!peirce)
                customDestroy();
        }
        else if (collision.CompareTag("BulletDeath"))
        {
            customDestroy();
        }
    }


    public void Ricochet()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, ricochetSearchRadius);
        float closest = float.MaxValue;
        GameObject closestObject = null;
        for (int i = 0; i < enemies.Length; i++)
        {
            var dist = Vector2.Distance(transform.position, enemies[i].transform.position);
            if (dist < closest)
            {
                closest = dist;
                closestObject = enemies[i].gameObject;
            }
        }
        if (closestObject != null)
        {
            var dir2 = transform.position - closestObject.transform.position;
            var angle = Mathf.Atan2(dir2.y, dir2.x) * Mathf.Rad2Deg + angleOffset;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            var rb = GetComponent<Rigidbody2D>();
            rb.angularVelocity = 0;
            rb.velocity = Vector2.zero;
            rb.AddForce(transform.right * redirectForce);
        }
    }

    public void customDestroy()
    {
        proj.setFree();
    }
}