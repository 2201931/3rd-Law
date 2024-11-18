using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
        // Destroy the bullet after 3 seconds
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Bullet collision with " + collision.gameObject.name);
        GameObject player = collision.gameObject;
        if (player == null) return;

        GameRules gameRules = Object.FindFirstObjectByType<GameRules>();
        if (gameRules != null)
        {
            gameRules.BulletHitPlayer(gameObject, player);
        }
    }
}
