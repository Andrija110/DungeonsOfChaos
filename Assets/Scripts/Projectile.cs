using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour {

	public float damage;
    public float range;
    public GameObject shootEffect;
	public GameObject hitEffect;
	public GameObject firingObject;

	Vector2 startPosition;

    // Use this for initialization
    void Start () {
		if (shootEffect != null)
		{
			GameObject obj = (GameObject)Instantiate(shootEffect, transform.position + new Vector3(0, 0, 5), Quaternion.identity);
            obj.transform.parent = firingObject.transform;
        }
		
		Destroy(gameObject, 5f); //Bullet will despawn after 5 seconds

		startPosition = transform.position;

        Game game = Game.Instance;
        game.IgnoreAllTraps(GetComponent<Collider2D>());
    }
	
	// Update is called once per frame
	void Update () {
		if (Time.frameCount % 10 == 0)
		{
			if (Vector2.SqrMagnitude(startPosition - (Vector2)transform.position) > range * range)
			{
				Destroy(gameObject);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{

		var go = other.gameObject;
		var hit = false;

		if (go.CompareTag("Enemy"))
		{
			var enemy = go.GetComponent<Enemy>();
			enemy.TakeDamage(damage);
			hit = true;
		}
		else if (go == firingObject || go.CompareTag("PlayerBullet") || go.CompareTag("EnemyBullet"))
		{
			hit = false;
		}
		else hit = true;

		if (hit)
		{
			Destroy(gameObject);

            if (hitEffect != null)
            {
                GameObject obj = (GameObject)Instantiate(hitEffect, transform.position + new Vector3(0, 0, 5), Quaternion.identity);
                obj.transform.parent = firingObject.transform;
            }
        }


	}

}
