/*
using System.Collections;
using System.Collections.Generic;
using _Scripts.Tank;
using UnityEngine;

public class MissileDamage : MonoBehaviour {


    //private float hitBoxRange;
    private float rangeX;
    private float rangeY;
    private float rangeZ;
    private float range;

    private float hitBoxRadius;

    private float damagePerP;

    private float damageToDo;

    public float maxDamage;

    public GameObject missile;

    private Player other;

    private void Update()
    {

        

    }


    /*private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            missile.GetComponent<MissileDamageActivator>().enemyList.Add(other.gameObject.GetComponent<Player>());
            Debug.LogWarning("Player Object " + other.gameObject.name + " enters the hitbox. (enemyList count: " + missile.GetComponent<MissileDamageActivator>().enemyList.Count + ")");
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            missile.GetComponent<MissileDamageActivator>().enemyList.Remove(other.gameObject.GetComponent<Player>());
            Debug.LogWarning("Player Object " + other.gameObject.name + " exits the hitbox. (enemyList count: " + missile.GetComponent<MissileDamageActivator>().enemyList.Count + ")");
        }
    }#1#

    public void missileDamage(bool perfect)
    {

        Debug.LogError("missileDamage");

        other = MissileDamageActivator.allEnemiesSorted.Current;
        


        hitBoxRadius = this.transform.localScale.x * missile.transform.localScale.x * this.GetComponent<SphereCollider>().radius;
        
        range = Vector3.Distance(other.transform.position, this.transform.position);
        Debug.LogWarning("Radius: " + hitBoxRadius + " - Range: " + range);


        if (range <= hitBoxRadius)
        {

            if (perfect == true)
            {

                damageToDo = maxDamage;

            }
            else
            {
                damagePerP = maxDamage / hitBoxRadius;

                damageToDo = maxDamage - (damagePerP * range);

            }

            other.GetComponent<TankHealth>().TakeDamage(damageToDo);

            //Debug.LogWarning(damageToDo + " " + other.name);

            //Debug.LogWarning(MissileDamageActivator.allEnemiesSorted.Current);

            bool nextPlayerCheck = MissileDamageActivator.allEnemiesSorted.MoveNext();
            if (nextPlayerCheck == true)
            {
                missileDamage(false);
            }
            else
            {

                Destroy(missile);

            }

            other.GetComponent<TankHealth>().TakeDamage(damageToDo);

        }
        else
        {

            Destroy(missile);

        }


    }


}
*/
