using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;

public class EnemyFolowPlayer : MonoBehaviour
{
    public float speed;
    public float lineOfSight;
    public float shootingRange;
    public float fireRate = 1f;
    private float nextFireTime;

    public GameObject bullet;

    private Transform bulletParent;
    private Transform player;

    private AudioSource ShootSound;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bulletParent = transform.Find("BulletParent");

        var audio = GameObject.Find("Audio").GetComponent<Audio>();
        ShootSound = gameObject.AddComponent<AudioSource>();
        ShootSound.clip = audio.EnemyShoot;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer < lineOfSight && distanceFromPlayer > shootingRange)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.position, speed * Time.deltaTime);
        }
        else if (distanceFromPlayer <= shootingRange && nextFireTime < Time.time) 
        {
            ShootSound.Play();
            Instantiate(bullet, bulletParent.position, Quaternion.identity);
            nextFireTime = Time.time + fireRate;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSight); 
        Gizmos.DrawWireSphere(transform.position, shootingRange);

    }
}
