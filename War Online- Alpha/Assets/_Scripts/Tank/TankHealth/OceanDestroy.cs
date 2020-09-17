using _Scripts.Tank;
using UnityEngine;

public class OceanDestroy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<TankHealth>() != null)
        {
            collision.gameObject.GetComponentInParent<TankHealth>().m_CurrentHealth = 0f;
        }
    }
}
