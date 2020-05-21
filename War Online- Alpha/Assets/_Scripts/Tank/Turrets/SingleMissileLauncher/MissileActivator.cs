using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileActivator : MonoBehaviour {


    public GameObject TrajectoryLine;


    public float Force;

    [HideInInspector]
    public static float ForcePressed;

    public GameObject Missile_Prefab;
    private GameObject Missile;

    [HideInInspector]
    public static float Rotation;

    float waitCountdown = 3;
    float forceCountdown = 0;

    private bool reverse = false;


    private void Start()
    {
        
        ForcePressed = Force / 2;

    }


    private void Update()
    {
        
        //Debug.Log(waitCountdown);

        if (Input.GetMouseButton(0))
        {

            if (waitCountdown >= 3)
            {

                if (reverse == false)
                {

                    TrajectoryLine.SetActive(true);

                    Debug.Log("GetMouseButtonDown Before " + ForcePressed);
                    ForcePressed += Force * Time.deltaTime;
                    Debug.Log("GetMouseButtonDown After " + ForcePressed);


                    Rotation = this.transform.rotation.x;

                    TrajectoryLine.GetComponent<LaunchArcRenderer>().RenderArc(ForcePressed);

                    forceCountdown += 0.5f * Time.deltaTime;

                }

                if (reverse == true)
                {

                    TrajectoryLine.SetActive(true);

                    Debug.Log("GetMouseButtonDown Before " + ForcePressed);
                    ForcePressed -= Force * Time.deltaTime;
                    Debug.Log("GetMouseButtonDown After " + ForcePressed);


                    Rotation = this.transform.rotation.x;

                    TrajectoryLine.GetComponent<LaunchArcRenderer>().RenderArc(ForcePressed);

                    forceCountdown += 0.5f * Time.deltaTime;

                }

                if (forceCountdown >= 2)
                {

                    reverse = !reverse;
                    forceCountdown = 0;

                    /*Missile = Instantiate(Missile_Prefab);
                    Missile.transform.SetParent(this.transform);
                    Missile.transform.localScale = new Vector3(1F, 1, 1);
                    Missile.transform.localPosition = new Vector3(0F, 0, 0);
                    Missile.transform.localRotation = new Quaternion(0F, 0, 0, 0);

                    Missile.GetComponent<MissileShoot>().MissileShootFunction(ForcePressed);

                    Missile.transform.parent = null;

                    ForcePressed = Force / 2;


                    waitCountdown = 0;
                    forceCountdown = 0;
                    reverse = false;

                    TrajectoryLine.SetActive(false);*/

                }

            }
            else
            {

                reverse = false;
                waitCountdown += 1 * Time.deltaTime;

            }

        }
        else
        {

            TrajectoryLine.SetActive(false);

            waitCountdown += 1 * Time.deltaTime;

        }

        if (Input.GetMouseButtonUp(0))
        {

            if (waitCountdown >= 3)
            {

                Missile = Instantiate(Missile_Prefab);
                Missile.transform.SetParent(this.transform);
                Missile.transform.localScale = new Vector3(1F, 1, 1);
                Missile.transform.localPosition = new Vector3(0F, 0, 0);
                Missile.transform.localRotation = new Quaternion(0F, 0, 0, 0);

                Missile.GetComponent<MissileShoot>().MissileShootFunction(ForcePressed);

                Missile.transform.parent = null;

                ForcePressed = Force / 2;


                waitCountdown = 0;
                forceCountdown = 0;
                reverse = false;

            }

        }
    }
	

}
