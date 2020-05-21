using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoventShip : MonoBehaviour {

public float speed= 1.0f;
public float acceleration= 1.0f;
public float maxspeed= 2.0f;
public float minspeed= -0.25f;
public float heading= 0.0f;
public float rudder= 0.0f;
public float rudderDelta= 2.0f;
public float maxRudder= 180.0f;
public float bob= 0.1f;
public float bobFrequency= 0.2f;

private float elapsed= 0.0f;
private float seaLevel= 0.0f;
private GameObject rudderControl;
private float rudderAngle= 0.0f;

float  signedSqrt ( float x ){
	float r= Mathf.Sqrt(Mathf.Abs( x ));
	if( x < 0 ){
		return -r;
	} else {
		return r;
	}
}
		
void  LateUpdate (){


	Debug.Log("Sailing script activated");
	// Bobbing
	elapsed += Time.deltaTime;
	float tempY = seaLevel + bob * Mathf.Sin(elapsed * bobFrequency * (Mathf.PI * 2));
	transform.position = new Vector3(transform.position.x, tempY, transform.position.z);

	// Steering
	rudder += Input.GetAxis("Horizontal") * rudderDelta * Time.deltaTime;
	if( rudder > maxRudder ){
		rudder = -maxRudder;
	} else if ( rudder < -maxRudder ){
		rudder = maxRudder;
	}
	heading = (heading + rudder * Time.deltaTime * signedSqrt(speed)) % 360;
	// transform.Rotate(0, rudder * Time.deltaTime, 0);
	//transform.eulerAngles.y = heading;
	transform.eulerAngles = new Vector3(transform.eulerAngles.x, heading, transform.eulerAngles.z);
	//transform.eulerAngles.z = -rudder;
	transform.eulerAngles = new Vector3(transform.eulerAngles.x, -rudder, transform.eulerAngles.z);

	if( rudderControl != null){
		rudderAngle = ((-60 * rudder)/maxRudder + heading) % 360;
		//rudderControl.transform.localEulerAngles.y = (70 * rudderAngle) % 360;
		rudderControl.transform.eulerAngles = new Vector3(0, rudderAngle, 0);
	}

	// Sail
	speed += Input.GetAxis("Vertical") * acceleration * Time.deltaTime;

				
		if (speed > maxspeed) {
			speed = maxspeed;
		} else if (speed < minspeed) {
			speed = minspeed;
		}

	transform.Translate(0, 0, speed * Time.deltaTime);
}

	void OnCollisionEnter(Collision collision) 
	{
		{
			speed = 0f;
		}
	}

void  Awake (){
	seaLevel = transform.position.y;
	rudderControl = GameObject.Find("rudderControl");
}
}
