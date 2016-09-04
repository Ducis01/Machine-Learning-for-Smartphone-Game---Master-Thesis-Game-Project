using UnityEngine;
using System.Collections;

public class Boom : MonoBehaviour {

	public GameObject explosion;

	public Skeleton skeleton;

	void OnExplode(Vector3 pos) {
		// Create a quaternion with a random rotation in the z-axis.
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

		// Instantiate the explosion where the rocket is with the random rotation.
		Instantiate(explosion, pos, randomRotation);
	}

	void OnCollisionEnter2D(Collision2D collision) {

		OnExplode (collision.contacts [0].point);

		Destroy (gameObject);

	}
}
