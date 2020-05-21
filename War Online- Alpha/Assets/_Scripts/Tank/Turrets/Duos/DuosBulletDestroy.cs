using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DuosBulletDestroy : MonoBehaviour
{
    public float destroyTime;
    public ParticleSystem particle;
    public ParticleSystem destroyedParticle;
    public float damage;

    [FMODUnity.EventRef]
    public string duosImpactSfx = "event:/TurretDuosImpact";
    FMOD.Studio.EventInstance duosImpactEv;
    private TankHealth enemy;

    private void Start()
    {
        particle.Play();
        Destroy();
        duosImpactEv = FMODUnity.RuntimeManager.CreateInstance(duosImpactSfx);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(duosImpactEv, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }

    private void Update()
    {
        if (particle.isStopped == true)
        {
            particle.Play();
            duosImpactEv.start();
        }
        
    }

    private void Destroy()
    {
        Destroy(gameObject, destroyTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionPoint = collision.GetContact(0).point;
        GameObject destroyedpart = Instantiate(destroyedParticle, collisionPoint, Quaternion.identity).gameObject;
        Destroy(destroyedpart, 0.5f);
        Destroy(gameObject);

        if (collision.gameObject.GetComponentInParent<TankHealth>() != null)
        {
            FactionID fID = collision.gameObject.GetComponentInParent<FactionID>();
            FactionID myID = gameObject.GetComponentInParent<FactionID>();

            if (fID == null || fID._teamID == 1 || myID._teamID == null || myID._teamID == 1 || fID._teamID != myID._teamID)
            {
                if (fID.myAccID != myID.myAccID)
                {
                    Damage();
                    enemy = collision.gameObject.GetComponentInParent<TankHealth>();
                }
            }
        }
    }

    
    void Damage()
    {
        if(enemy != null)
        enemy.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", enemy.GetComponent<PhotonView>().Owner, damage);
    }
}

