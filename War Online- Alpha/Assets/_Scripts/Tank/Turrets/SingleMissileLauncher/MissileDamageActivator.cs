/*
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissileDamageActivator : MonoBehaviour {

    public static Player closestEnemy;

    public static Player[] allEnemies;
    public static IEnumerator<Player> allEnemiesSorted;

    [HideInInspector] public List<Player> enemyList;

    private static GameObject MDA;
    private static GameObject HitBoxChild;

    private static float distanceToEnemy;

    public GameObject thisScript;
    public GameObject hitBoxChild;

    Rigidbody rig;

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
    }

   /* private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Player"))
        {
            foreach (Player damageable in enemyList)
            {
                Debug.LogWarning("Damage inflicted to \"" + damageable.gameObject.name + "\" (Distance from missile: " + (this.transform.position - damageable.transform.position).sqrMagnitude + " units, Damage: unknown)");
            }
            Destroy(this.gameObject);
        }
    }#1#

    void OnTriggerEnter(Collider other)
    {



        MDA = thisScript;
        HitBoxChild = hitBoxChild;

        if (other.CompareTag("Player")) FindClosestEnemy(true);
        else FindClosestEnemy(false);
    }

    public static void FindClosestEnemy(bool perfectHit)
    {

        Debug.Log("FindClosestEnemy");

        allEnemies = GameObject.FindObjectsOfType<Player>();
        allEnemiesSorted = allEnemies.OrderBy(e => Vector3.Distance(e.transform.position, MDA.transform.position)).GetEnumerator(); //(MDA.transform.position - e.transform.position).sqrMagnitude);

        Debug.LogError(allEnemies[0]);

        allEnemiesSorted.MoveNext();
        HitBoxChild.GetComponent<MissileDamage>().missileDamage(perfectHit);

    }

}
*/
