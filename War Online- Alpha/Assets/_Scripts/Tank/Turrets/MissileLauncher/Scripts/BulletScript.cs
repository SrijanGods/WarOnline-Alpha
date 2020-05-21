using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {
	EnemyScript range2;
	public GameObject Enemy;
	// Use this for initialization
	void Start () {
		range2 = GetComponent<EnemyScript> ();
		Enemy = GameObject.FindGameObjectWithTag("Enemy");
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (Enemy.transform.position, this.transform.position) <15f) {
			Vector3 direction = Enemy.transform.position - this.transform.position;
			Debug.DrawRay (this.transform.position, direction);
			this.transform.rotation = Quaternion.Slerp(this.transform.rotation,Quaternion.LookRotation(direction),0.1f);

			if (direction.magnitude >5f) {
                //this.transform.Translate (0.2f, 0.2f, 0);

                direction.Normalize();
                var rotateAmount = Vector3.Cross( direction, transform.up);
                GetComponent<Rigidbody>().angularVelocity = -rotateAmount * 200f;
                GetComponent<Rigidbody>().velocity = transform.up * 5f;
            }
		}
	}
	void OnCollisionEnter(Collision col)
	{
		Destroy (gameObject);
	}
}
