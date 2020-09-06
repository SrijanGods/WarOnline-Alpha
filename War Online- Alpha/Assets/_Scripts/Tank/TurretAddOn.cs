using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAddOn : MonoBehaviour
{
    private TankHealth _tankHealth;
    public GameObject turretBody, destroyedBody;

    private void Start()
    {
        _tankHealth = GetComponentInParent<TankHealth>();
        _tankHealth.actualTurret = turretBody;
        _tankHealth.destroyedTurret = destroyedBody;

        //GetComponentInChildren<Pro3DCamera.CameraControl>().target = transform;
    }
}
