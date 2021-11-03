using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	[Header("Move speed")]
	[Range(0, 100)]
	[SerializeField] private float moveSpeed;

	[Header("Bullet speed")] [Range(0, 100)] [SerializeField]
	private float bulletSpeed = 20;

	[Header("Atack time")] [Range(0.2f, 5)] 
	[SerializeField] private float attackTime;

	[SerializeField] private GameObject bulletPref;
	[SerializeField] private GameObject particlePref;
	[SerializeField] private Transform target;
	[SerializeField] private Transform spawnTransform;
	
	private ParticleSystem _particle;
	private void Start()
	{
		_particle = Instantiate(particlePref).GetComponent<ParticleSystem>();
		_particle.Stop();
		StartCoroutine(Shot());
	}

	private IEnumerator Shot()
	{
		while (true)
		{
			yield return new WaitForSeconds(attackTime);
			
			Ray ray = new Ray(transform.position, Vector3.forward);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.transform.CompareTag("Obstacle"))
					yield return new WaitForSeconds(attackTime);
			}
			
			var bullet = Instantiate(bulletPref, spawnTransform.position + Vector3.back, Quaternion.identity);
			bullet.transform.rotation = transform.rotation;
			bullet.tag = "BulletEnemy";
			bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;
			Destroy(bullet, 3f);
		}
	}
	

	int sign = 1;
	private void FixedUpdate()
	{
		transform.LookAt(target);
		if (transform.position.x <= -3f || transform.position.x >= 3f)
			sign *= -1;
		transform.position += sign * Vector3.right * moveSpeed * Time.fixedDeltaTime;
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Bullet"))
		{
			_particle.transform.position = other.transform.position;
			moveSpeed = 0;
			StopAllCoroutines();
			_particle.Play();
			Destroy(gameObject, 1f);
		}
	}

}
