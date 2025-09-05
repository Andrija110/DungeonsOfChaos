using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using UnityEngine;

public class EnemyStats
{
    public float Distance { get; set; }
    public float Health { get; set; }
    public Vector3 Position { get; set; }
}

public class WeaponControl : MonoBehaviour
{
    public GameObject ProjectilePrefab;

    private PlayerStats ps = PlayerStats.Instance;
    float timeLastShoot;
    private AudioSource Sound;

    // Start is called before the first frame update
    void Start()
    {
        var audio = GameObject.Find("Audio").GetComponent<Audio>();
        Sound = GetComponent<AudioSource>();
        Sound.clip = audio.PlayerShoot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    Vector2 UpdateAimDirection(Vector3 pos, Vector2 direction)
    {
        var g = Game.Instance;
        if (g.aimType == AimType.Manual) return direction;

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return direction;

        List<EnemyStats> enemiesInRange = new();
        foreach (var enemy in enemies)
        {
            var dist = Vector3.Distance(pos, enemy.transform.position);
            var enemyComponent = enemy.GetComponent<Enemy>();
            
            if (dist <= ps.ProjectileRange)
            {
                var enemyStats = new EnemyStats { Distance = dist, Health = enemyComponent.health, Position = enemy.transform.position };
                enemiesInRange.Add(enemyStats);
            }
        }
        if (enemiesInRange.Count == 0) return direction;

        EnemyStats target = null;
        switch (g.aimType)
        {            
            case AimType.AutomaticNearest:
                target = enemiesInRange.OrderBy(r => r.Distance).First();
                break;
            case AimType.AutomaticWeakest:
                target = enemiesInRange.OrderBy(r => r.Health).First();
                break;
            case AimType.AutomaticStrongest:
                target = enemiesInRange.OrderBy(r => r.Health).Last();
                break;
            default:
                return direction;
        }

        return (target.Position - pos).normalized;

    }

    public void Shoot(GameObject whoIsFiring, Vector2 direction) 
    {
        // Fire rate
        var timeDelta = Time.time - timeLastShoot;
        if (timeDelta < 1f / ps.FireRate) return;
        else timeLastShoot = Time.time;

        var pos = whoIsFiring.transform.position;

        direction = UpdateAimDirection(pos, direction);

        var rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg-90f);
        GameObject projectile = Instantiate(ProjectilePrefab, pos, rotation);
        Sound.Play();

        // ProjectileScale
        projectile.transform.localScale = Vector3.one * ps.ProjectileScale;

        // ProjectileSpeed
        projectile.GetComponent<Rigidbody2D>().AddForce(projectile.transform.up * ps.ProjectileSpeed);

        
        var projectileProperties = projectile.GetComponent<Projectile>();
        projectileProperties.firingObject = whoIsFiring;

        // ProjectileDamage
        projectileProperties.damage = ps.ProjectileDamage;

        // ProjectileRange
        projectileProperties.range = ps.ProjectileRange;

    }

}
