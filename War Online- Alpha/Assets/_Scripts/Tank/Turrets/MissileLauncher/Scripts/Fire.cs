using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour {
	//InstantiateMissiles inst;
	private float downTime, upTime, pressTime =0;
	public float countDown = 2.0f;
	public Transform[] SpawnPoints = new Transform[4];
	public GameObject OBJ;
	public GameObject[] Missiles = new GameObject[4];
	public Transform Parent;
	public bool Ready = false;
	public float ReloadTime = 4f;
	public float DestroyAfter = 10f;
	Rigidbody[] rb = new Rigidbody[4];
	int c = 0;
	public bool RReload = false;
	public float forwardf;
	// Use this for initialization
	void Start () {
		for (int i = 0; i <= 3; i++) {
			Missiles[i] = (GameObject)Instantiate (OBJ, SpawnPoints [i].transform.position, SpawnPoints [i].transform.rotation);
			rb [i] = Missiles [i].GetComponent<Rigidbody> ();
			Missiles [i].transform.parent = transform;
		}
		//Ready = true;

	}
	
	// Update is called once per frame
	void Update () {
		
			//inst.Ready = false;
		if (Input.GetKeyDown (KeyCode.Space)&&Ready == false) {
			downTime = Time.time;
			pressTime = downTime + countDown;
			Ready = true;
			}
		if (Input.GetKeyUp (KeyCode.Space)) {
			Ready = false;
			rb [c].AddForce (transform.forward * forwardf);RReload = true;
			Missiles [c].transform.parent = null; 
			Destroy (Missiles [c], DestroyAfter);
			if (c >= 3) {
				StartCoroutine(Reload());
				c = 0;
			} else if(RReload==true) {
				c++;
				RReload = false;
			}
		}
		if( Time.time>= pressTime && Ready == true)
		{
			
			StartCoroutine (Shoot ());

			Ready = false;
			StartCoroutine (Reload ());
				
		}
	}
	IEnumerator Shoot()
	{

		for (int i = 0; i <= 3; i++) {

			rb [i].AddForce (transform.forward * forwardf);

			Missiles [i].transform.parent = null; 
			yield return new WaitForSeconds (0.5f);
			Destroy (Missiles [i], DestroyAfter);
		}

	}
	IEnumerator Reload()
	{
		yield return new WaitForSeconds (ReloadTime);
		Start ();
	}

}

