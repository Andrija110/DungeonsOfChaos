using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public float health, maxHealth = 3f;

    public Game game { get; set; }

    private AudioSource DeathSound;

    private void Start()
    {
        health = maxHealth;
        game = Game.Instance;

        var audio = GameObject.Find("Audio").GetComponent<Audio>();
        DeathSound = gameObject.AddComponent<AudioSource>();
        DeathSound.clip = audio.EnemyDeath;
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;       
        if(health <= 0)
        {
            DeathSound.Play();
            Destroy(gameObject);
        }

    }

    private void OnDestroy()
    {
        if (game != null) 
            game.EnemyDestroyed();
    }

    
    /*
private Rigidbody2D rb;
private Player player;
private float moveSpeed;
private Vector3 directoinToPlayer;
private Vector3 localScale;
// Start is called before the first frame update
void Start()
{
   rb = GetComponent<Rigidbody2D>();
   player = FindObjectOfType(typeof(Player)) as Player;
   moveSpeed = 2f;
   localScale = transform.localScale;
}

private void FixedUpdate()
{
   MoveEnemy();
}

private void MoveEnemy()
{
   directoinToPlayer = (player.transform.position - transform.position).normalized;
   rb.velocity = new Vector2(directoinToPlayer.x, directoinToPlayer.y) * moveSpeed;
}

private void LateUpdate()
{
   if (rb.velocity.x > 0)
   {
       transform.localScale = new Vector3(localScale.x, localScale.y, localScale.z);
   }
   else if (rb.velocity.x < 0) 
   {
       transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
   }
}
*/
}
