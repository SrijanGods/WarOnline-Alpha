using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]


public class LaunchArcRenderer : MonoBehaviour {

    LineRenderer lr;

    [HideInInspector]
    public float velocity;
    [HideInInspector]
    public float angle;

    public int resolution = 10;

    float gravity;
    float radianAngle;

    float Rotation;
    


    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        gravity = Mathf.Abs(Physics2D.gravity.y);
    }

    void Start () {

        //RenderArc();

	}

     


    private void Update()
    {

        //velocity = MissileActivator.ForcePressed / 59;
    }


    //populating the LineRender with the appropriate settings
    public void RenderArc(float ForcePressed)
    {

        Rotation = MissileActivator.Rotation * (118310068447 / 1000000000);

        this.transform.localRotation = new Quaternion(0F, 0, 0, 0);


        if (this.transform.localRotation.z > 0)
        {

            this.transform.Rotate(0, 270, (Rotation - Rotation - Rotation));
            angle = Rotation;

        }
        else
        {

            this.transform.Rotate(0, 270, Rotation);

            angle = (Rotation - Rotation - Rotation);

        }


        velocity = (ForcePressed / 595) * 10;


        lr.SetVertexCount(resolution + 1);
        lr.SetPositions(CalculateArcArray());

    }

    private Vector3[] CalculateArcArray()
    {

        Vector3[] arcArray = new Vector3[resolution + 1];

        radianAngle = Mathf.Deg2Rad * angle;
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / gravity;

        for (int i = 0; i <= resolution; i++)
        {

            float t = (float)i / (float)resolution;
            arcArray[i] = CalculateArcPoint(t, maxDistance);

        }

        return arcArray;

    }

    Vector3 CalculateArcPoint(float t, float maxDistance)
    {

        float x = t * maxDistance;
        float y = x * Mathf.Tan(radianAngle) - ((gravity * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
        return new Vector3(x, y);

    }

}
