using _Scripts.Tank;
using _Scripts.Tank.TankHealth;
using UnityEngine;

public class OceanDestroy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<TankHealth>() != null)
        {
            collision.gameObject.GetComponentInParent<TankHealth>().currentHealth = 0f;
        }
    }
}
