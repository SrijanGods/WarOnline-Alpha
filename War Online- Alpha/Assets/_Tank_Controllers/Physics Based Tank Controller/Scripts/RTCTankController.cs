#pragma warning disable 0414

#if UNITY_5 && (!UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && ! UNITY_5_3) || UNITY_2017
#define UNITY_MIN_5_4
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]

public class RTCTankController : MonoBehaviourPun
{

    [Header("edits")]
    public float speedmultiplier = 6f;
    public float fuelInputMultiplier = 6f;
    public float gasInputMultiplier = 3f;

    // Rigidbody
    public Rigidbody rigid;

    [Header("Enables/Disables controlling the vehicle")]
    //public bool canControl;               // Removed in favor of Photon-based controls
    public bool runEngineAtAwake = true;
    public bool engineRunning = false;
    private bool engineStarting = false;

    [Header("Reversing Bool")]
    private bool reversing = false;
    private bool autoReverse = false;
    private bool canGoReverseNow = false;
    private float reverseDelay = 0f;

    [Header("Wheel Transforms Of The Vehicle")]
    public Transform[] wheelTransform_L;
    public Transform[] wheelTransform_R;

    [Header("Wheel colliders of the vehicle")]
    public WheelCollider[] wheelColliders_L;
    public WheelCollider[] wheelColliders_R;

    [Header("All Wheel Colliders")]
    private List<WheelCollider> allWheelColliders = new List<WheelCollider>();

    [Header("Useless Gear Wheels")]
    public Transform[] uselessGearTransform_L;
    public Transform[] uselessGearTransform_R;

    [Header("Track Bones")]
    public Transform[] trackBoneTransform_L;
    public Transform[] trackBoneTransform_R;

    [Header("Track Customization")]
    public GameObject leftTrackMesh;
    public GameObject rightTrackMesh;
    public float trackOffset = 0.025f;
    public float trackScrollSpeedMultiplier = 1f;

    [Header("Wheels Rotation")]
    public float[] rotationValueL;
    public float[] rotationValueR;

    [Header("Center Of Mass")]
    public Transform COM;

    [Header("Mechanic")]
    public AnimationCurve engineTorqueCurve;
    public float engineTorque = 3500.0f;
    public float brakeTorque = 5000.0f;
    public float minEngineRPM = 1000.0f;
    public float maxEngineRPM = 5000.0f;
    public float maxSpeed = 60.0f;
    public float steerTorque = 5f;
    private float speed;
    private float defSteerAngle;
    private float acceleration = 0f;
    private float lastVelocity = 0f;
    private float engineRPM = 0.0f;

    [Header("Inputs")]
    public float gasInput = 0f;
    public float brakeInput = 0f;
    public float steerInput = 0f;
    public float fuelInput = 6f;

    [Header("Sound Effects")]

    [FMODUnity.EventRef]
    public string enginesfx = "event:/Engine";
    FMOD.Studio.EventInstance engineEv;

    [Header("Smokes")]
    public GameObject wheelSlip;
    private List<ParticleSystem> wheelParticles = new List<ParticleSystem>();



    [Header("Photon")]
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    #region Start
    void Start()
    {
        //FMOD sounds
        engineEv = FMODUnity.RuntimeManager.CreateInstance(enginesfx);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(engineEv, GetComponent<Transform>(), GetComponent<Rigidbody>());
        engineEv.start();

        WheelCollidersInit();
        if (wheelSlip)
            SmokeInit();

        //rigid = gameObject.GetComponent<Rigidbody>();

        rigid.maxAngularVelocity = 5f;
        rigid.centerOfMass = new Vector3((COM.localPosition.x) * transform.localScale.x, (COM.localPosition.y) * transform.localScale.y, (COM.localPosition.z) * transform.localScale.z);

        if (runEngineAtAwake)
            KillOrStartEngine();

#if UNITY_MIN_5_4
                // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
                {
                    this.CalledOnLevelWasLoaded(scene.buildIndex);
                };
#endif
    }
    #endregion Start

    #region WheelColliders

    public void CreateWheelColliders()
    {

        List<Transform> allWheelTransformsL = new List<Transform>();
        List<Transform> allWheelTransformsR = new List<Transform>();

        foreach (Transform wheel in wheelTransform_L)
        {
            allWheelTransformsL.Add(wheel);
        }
        foreach (Transform wheel in wheelTransform_R)
        {
            allWheelTransformsR.Add(wheel);
        }

        if (allWheelTransformsR[0] == null || allWheelTransformsL[0] == null)
        {
            Debug.LogError("You haven't choose your Wheel Transforms. Please select all of your Wheel Transforms before creating Wheel Colliders. Script needs to know their positions, aye?");
            return;
        }

        transform.rotation = Quaternion.identity;

        GameObject _WheelColliders_L = new GameObject("WheelColliders_L");
        _WheelColliders_L.transform.parent = transform;
        _WheelColliders_L.transform.rotation = transform.rotation;
        _WheelColliders_L.transform.localPosition = Vector3.zero;
        _WheelColliders_L.transform.localScale = Vector3.one;

        GameObject _WheelColliders_R = new GameObject("WheelColliders_R");
        _WheelColliders_R.transform.parent = transform;
        _WheelColliders_R.transform.rotation = transform.rotation;
        _WheelColliders_R.transform.localPosition = Vector3.zero;
        _WheelColliders_R.transform.localScale = Vector3.one;

        #region Wheel Collider Properties
        foreach (Transform wheel in allWheelTransformsL)
        {

            GameObject wheelcolliderL = new GameObject(wheel.transform.name);

            wheelcolliderL.transform.position = wheel.transform.position;
            wheelcolliderL.transform.rotation = transform.rotation;
            wheelcolliderL.transform.name = wheel.transform.name;
            wheelcolliderL.transform.parent = _WheelColliders_L.transform;
            wheelcolliderL.transform.localScale = Vector3.one;
            wheelcolliderL.AddComponent<WheelCollider>();
            wheelcolliderL.GetComponent<WheelCollider>().radius = (wheel.GetComponent<MeshRenderer>().bounds.size.y / 2f) / transform.localScale.y;

            JointSpring spring = wheelcolliderL.GetComponent<WheelCollider>().suspensionSpring;

            spring.spring = 50000f;
            spring.damper = 5000f;
            spring.targetPosition = .5f;

            wheelcolliderL.GetComponent<WheelCollider>().mass = 200f;
            wheelcolliderL.GetComponent<WheelCollider>().wheelDampingRate = 10f;
            wheelcolliderL.GetComponent<WheelCollider>().suspensionDistance = .3f;
            wheelcolliderL.GetComponent<WheelCollider>().forceAppPointDistance = .25f;
            wheelcolliderL.GetComponent<WheelCollider>().suspensionSpring = spring;

            wheelcolliderL.transform.localPosition = new Vector3(wheelcolliderL.transform.localPosition.x, wheelcolliderL.transform.localPosition.y + (wheelcolliderL.GetComponent<WheelCollider>().suspensionDistance / 2f), wheelcolliderL.transform.localPosition.z);

            WheelFrictionCurve sidewaysFriction = wheelcolliderL.GetComponent<WheelCollider>().sidewaysFriction;
            WheelFrictionCurve forwardFriction = wheelcolliderL.GetComponent<WheelCollider>().forwardFriction;

            forwardFriction.extremumSlip = .4f;
            forwardFriction.extremumValue = 1;
            forwardFriction.asymptoteSlip = .8f;
            forwardFriction.asymptoteValue = .75f;
            forwardFriction.stiffness = 1.75f;

            sidewaysFriction.extremumSlip = .25f;
            sidewaysFriction.extremumValue = 1;
            sidewaysFriction.asymptoteSlip = .5f;
            sidewaysFriction.asymptoteValue = .75f;
            sidewaysFriction.stiffness = 2f;

            wheelcolliderL.GetComponent<WheelCollider>().sidewaysFriction = sidewaysFriction;
            wheelcolliderL.GetComponent<WheelCollider>().forwardFriction = forwardFriction;

        }

        foreach (Transform wheel in allWheelTransformsR)
        {

            GameObject wheelcolliderR = new GameObject(wheel.transform.name);

            wheelcolliderR.transform.position = wheel.transform.position;
            wheelcolliderR.transform.rotation = transform.rotation;
            wheelcolliderR.transform.name = wheel.transform.name;
            wheelcolliderR.transform.parent = _WheelColliders_R.transform;
            wheelcolliderR.transform.localScale = Vector3.one;
            wheelcolliderR.AddComponent<WheelCollider>();
            wheelcolliderR.GetComponent<WheelCollider>().radius = (wheel.GetComponent<MeshRenderer>().bounds.size.y / 2f) / transform.localScale.y;

            JointSpring spring = wheelcolliderR.GetComponent<WheelCollider>().suspensionSpring;

            spring.spring = 50000f;
            spring.damper = 5000f;
            spring.targetPosition = .5f;

            wheelcolliderR.GetComponent<WheelCollider>().mass = 200f;
            wheelcolliderR.GetComponent<WheelCollider>().wheelDampingRate = 10f;
            wheelcolliderR.GetComponent<WheelCollider>().suspensionDistance = .3f;
            wheelcolliderR.GetComponent<WheelCollider>().forceAppPointDistance = .25f;
            wheelcolliderR.GetComponent<WheelCollider>().suspensionSpring = spring;

            wheelcolliderR.transform.localPosition = new Vector3(wheelcolliderR.transform.localPosition.x, wheelcolliderR.transform.localPosition.y + (wheelcolliderR.GetComponent<WheelCollider>().suspensionDistance / 2f), wheelcolliderR.transform.localPosition.z);

            WheelFrictionCurve sidewaysFriction = wheelcolliderR.GetComponent<WheelCollider>().sidewaysFriction;
            WheelFrictionCurve forwardFriction = wheelcolliderR.GetComponent<WheelCollider>().forwardFriction;

            forwardFriction.extremumSlip = .4f;
            forwardFriction.extremumValue = 1;
            forwardFriction.asymptoteSlip = .8f;
            forwardFriction.asymptoteValue = .75f;
            forwardFriction.stiffness = 1.75f;

            sidewaysFriction.extremumSlip = .25f;
            sidewaysFriction.extremumValue = 1;
            sidewaysFriction.asymptoteSlip = .5f;
            sidewaysFriction.asymptoteValue = .75f;
            sidewaysFriction.stiffness = 2f;

            wheelcolliderR.GetComponent<WheelCollider>().sidewaysFriction = sidewaysFriction;
            wheelcolliderR.GetComponent<WheelCollider>().forwardFriction = forwardFriction;

        }
        #endregion

        wheelColliders_L = _WheelColliders_L.GetComponentsInChildren<WheelCollider>();
        wheelColliders_R = _WheelColliders_R.GetComponentsInChildren<WheelCollider>();

    }

    void WheelCollidersInit()
    {

        WheelCollider[] wheelcolliders = GetComponentsInChildren<WheelCollider>();

        foreach (WheelCollider wc in wheelcolliders)
        {
            allWheelColliders.Add(wc);
        }

    }

    #endregion WheelColliders

    #region Sounds

    public void Sounds()
    {
        //Engine Audio Volume.
        if (speed == 0)
        {
            engineEv.setParameterValue("EngineStone", 0f);
            engineEv.setParameterValue("EngineTrack", 0f);
        }
        else
        {
            engineEv.setParameterValue("EngineStone", 1f);
            engineEv.setParameterValue("EngineTrack", 1f);
        }

        /*
        if (engineEv)
        {
            
            if (!reversing)
            {
                engineEv.setParameterValue("EngineStone", 1f);
                engineEv.setParameterValue("EngineTrack", 1f);
            }

            else
            {
                engineEv.setParameterValue("EngineStone", 1f);
                engineEv.setParameterValue("EngineTrack", 1f);
            }
               
            if (engineRunning)
            {
                engineEv.setParameterValue("EngineStone", 1f);
                engineEv.setParameterValue("EngineTrack", 1f);
            }
            else
            {
                engineEv.setParameterValue("EngineStone", 1f);
                engineEv.setParameterValue("EngineTrack", 1f);
            }
                

        }

        if (engineIdleAudioClip)
        {

            if (!reversing)
            {
                engineEv.setParameterValue("EngineStone", 0f);
                engineEv.setParameterValue("EngineTrack", 0f);
            }
            else
            {
                engineEv.setParameterValue("EngineStone", 0f);
                engineEv.setParameterValue("EngineTrack", 0f);
            }
               

            if (engineRunning)
            {
                engineEv.setParameterValue("EngineStone", 0f);
                engineEv.setParameterValue("EngineTrack", 0f);
            }
            else
            {
                engineEv.setParameterValue("EngineStone", 0f);
                engineEv.setParameterValue("EngineTrack", 0f);
                engineEv.setParameterValue("EngineStart", 0f);
            }
               

        }
        */
            
    }

    #endregion Sounds

    #region EngineStuff

    public void KillOrStartEngine()
    {

        if (engineRunning && !engineStarting)
        {
            engineRunning = false;
        }
        else if (!engineStarting)
        {
            StartCoroutine("StartEngine");
        }

    }

    IEnumerator StartEngine()
    {

        engineRunning = false;
        engineStarting = true;
        if (!engineRunning)
            engineEv.setParameterValue("EngineStart", 1f);
        yield return new WaitForSeconds(1f);
        engineRunning = true;
        yield return new WaitForSeconds(1f);
        engineStarting = false;

    }

    void SmokeInit()
    {

        for (int i = 0; i < allWheelColliders.Count; i++)
        {
            GameObject wp = (GameObject)Instantiate(wheelSlip, allWheelColliders[i].transform.position, transform.rotation) as GameObject;
            wheelParticles.Add(wp.GetComponent<ParticleSystem>());
        }

        for (int i = 0; i < allWheelColliders.Count; i++)
        {
            wheelParticles[i].transform.position = allWheelColliders[i].transform.position;
            wheelParticles[i].transform.parent = allWheelColliders[i].transform;
        }

    }

    #endregion EngineStuff

    #region Updates

    void Update()
    {

        WheelAlign();
        Sounds();

    }

    void FixedUpdate()
    {

        AnimateGears();
        Engine();
        Braking();
        Inputs();
        Smoke();

    }
    #endregion Updates

    #region EngineBraqueTorque
    void Engine()
    {

        //Reversing Bool.
        if (gasInput < 0 && transform.InverseTransformDirection(rigid.velocity).z < 1 && canGoReverseNow)
            reversing = true;
        else
            reversing = false;

        speed = rigid.velocity.magnitude * speedmultiplier;

        //Acceleration Calculation.
        acceleration = 0f;
        acceleration = (transform.InverseTransformDirection(rigid.velocity).z - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = transform.InverseTransformDirection(rigid.velocity).z;

        //Drag Limit.

        float rpm = 0f;
        float wheelRPM = ((Mathf.Abs((wheelColliders_L[2].rpm * wheelColliders_L[2].radius) + (wheelColliders_R[2].rpm * wheelColliders_R[2].radius)) / 2f) / 3.25f);

        if (!reversing)
            rpm = Mathf.Clamp((Mathf.Lerp(minEngineRPM, maxEngineRPM, wheelRPM / maxSpeed) + minEngineRPM) * (Mathf.Clamp01(gasInput + Mathf.Abs(steerInput))), minEngineRPM, maxEngineRPM);
        else
            rpm = Mathf.Clamp((Mathf.Lerp(minEngineRPM, maxEngineRPM, wheelRPM / maxSpeed) + minEngineRPM) * (Mathf.Clamp01(brakeInput + Mathf.Abs(steerInput))), minEngineRPM, maxEngineRPM);

        engineRPM = Mathf.Lerp(engineRPM, (rpm + UnityEngine.Random.Range(-50f, 50f)) * fuelInput, Time.deltaTime * 2f);

        if (!engineRunning)
            fuelInput = 0;
        else
            fuelInput = fuelInputMultiplier;

        //Auto Reverse Bool.
        if (autoReverse)
        {
            canGoReverseNow = true;
        }
        else
        {
            if (brakeInput > .1f && speed < 5)
                reverseDelay += Time.deltaTime;
            else if (brakeInput > 0 && transform.InverseTransformDirection(rigid.velocity).z > 1f)
                reverseDelay = 0f;
        }

        if (reverseDelay >= .75f)
            canGoReverseNow = true;
        else
            canGoReverseNow = false;


        for (int i = 0; i < wheelColliders_L.Length; i++)
        {

            ApplyMotorTorque(wheelColliders_L[i], engineTorque, true);
            wheelColliders_L[i].wheelDampingRate = Mathf.Lerp(100f, 0f, gasInput);

        }

        for (int i = 0; i < wheelColliders_R.Length; i++)
        {

            ApplyMotorTorque(wheelColliders_R[i], engineTorque, false);
            wheelColliders_R[i].wheelDampingRate = Mathf.Lerp(100f, 0f, gasInput);

        }

        //Steering.
        if (!reversing)
        {
            if (wheelColliders_L[2].isGrounded || wheelColliders_R[2].isGrounded)
            {
                if (Mathf.Abs(rigid.angularVelocity.y) < 1f)
                {
                    rigid.AddRelativeTorque((Vector3.up * steerInput) * steerTorque, ForceMode.Acceleration);
                }
            }
        }
        else
        {
            if (wheelColliders_L[2].isGrounded || wheelColliders_R[2].isGrounded)
            {
                if (Mathf.Abs(rigid.angularVelocity.y) < 1f)
                {
                    rigid.AddRelativeTorque((-Vector3.up * steerInput) * steerTorque, ForceMode.Acceleration);
                }
            }
        }

    }

    public void ApplyMotorTorque(WheelCollider wc, float torque, bool leftSide)
    {

        WheelHit hit;
        wc.GetGroundHit(out hit);

        if (Mathf.Abs(hit.forwardSlip) > 1f)
            torque = 0;

        if (speed > maxSpeed || Mathf.Abs(wc.rpm) > 1000 || Mathf.Abs(wc.rpm) > 1000 || !engineRunning)
            torque = 0;

        if (reversing && speed > 55)
            torque = 0;

        if (!engineRunning || !wc.isGrounded)
            torque = 0;

        if (!reversing)
        {
            if (leftSide)
                wc.motorTorque = torque * Mathf.Clamp((Mathf.Clamp(gasInput * fuelInput, 0f, 1f)) + Mathf.Clamp(steerInput, -1f, 1f), -1f, 1f) * engineTorqueCurve.Evaluate(speed);
            else
                wc.motorTorque = torque * Mathf.Clamp((Mathf.Clamp(gasInput * fuelInput, 0f, 1f)) + Mathf.Clamp(-steerInput, -1f, 1f), -1f, 1f) * engineTorqueCurve.Evaluate(speed);
        }
        else
        {
            wc.motorTorque = (-torque) * brakeInput;
        }

    }

    public void ApplyBrakeTorque(WheelCollider wc, float brake)
    {

        wc.brakeTorque = brake;

    }

    public void Braking()
    {

        for (int i = 0; i < allWheelColliders.Count; i++)
        {

            if (brakeInput > .1f && !reversing)
            {
                ApplyBrakeTorque(allWheelColliders[i], brakeTorque * (brakeInput));
            }
            else if (brakeInput > .1f && reversing)
            {
                ApplyBrakeTorque(allWheelColliders[i], 0f);
            }
            else if (gasInput < .1f && Mathf.Abs(steerInput) < .1f)
            {
                ApplyBrakeTorque(allWheelColliders[i], 10f);
            }
            else
            {
                ApplyBrakeTorque(allWheelColliders[i], 0f);
            }

        }

    }
    #endregion EngineBraqueTorque

    #region Inputs
    void Inputs()
    {
       // uselessGearTransform_L[0].transform.rotation;
        /*	if(!photonView.isMine){
                gasInput = 0;
                brakeInput = 0;
                steerInput = 0;
                return;
            } */

        //Motor Input.
        gasInput = Input.GetAxis("Vertical") * gasInputMultiplier;

        //Brake Input
        brakeInput = -Mathf.Clamp(gasInput, -1f, 0f);

        //Steering Input.
        steerInput = Input.GetAxis("Horizontal");

    }
    #endregion Inputs

    #region Gears&Wheels

    void AnimateGears()
    {
        for (int i = 0; i < uselessGearTransform_L.Length; i++)
        {
            uselessGearTransform_L[i].transform.rotation = wheelColliders_L[i].transform.rotation * Quaternion.Euler(rotationValueL[Mathf.CeilToInt((wheelColliders_L.Length) / 2)], wheelColliders_L[i].steerAngle, 0);
        }
        for (int i = 0; i < uselessGearTransform_R.Length; i++)
        {
            uselessGearTransform_R[i].transform.rotation = wheelColliders_R[i].transform.rotation * Quaternion.Euler(rotationValueR[Mathf.CeilToInt((wheelColliders_R.Length) / 2)], wheelColliders_R[i].steerAngle, 0);
        }

    }

    void WheelAlign()
    {

        RaycastHit hit;

        //Right Wheels Transform.
        for (int k = 0; k < wheelColliders_R.Length; k++)
        {

            Vector3 ColliderCenterPoint = wheelColliders_R[k].transform.TransformPoint(wheelColliders_R[k].center);

            if (Physics.Raycast(ColliderCenterPoint, -wheelColliders_R[k].transform.up, out hit, (wheelColliders_R[k].suspensionDistance + wheelColliders_R[k].radius) * transform.localScale.y) && hit.transform.root != transform)
            {
                wheelTransform_R[k].transform.position = hit.point + (wheelColliders_R[k].transform.up * wheelColliders_R[k].radius) * transform.localScale.y;
                if (trackBoneTransform_R.Length > 0)
                    trackBoneTransform_R[k].transform.position = hit.point + (wheelColliders_R[k].transform.up * trackOffset) * transform.localScale.y;
            }
            else
            {
                wheelTransform_R[k].transform.position = ColliderCenterPoint - (wheelColliders_R[k].transform.up * wheelColliders_R[k].suspensionDistance) * transform.localScale.y;
                if (trackBoneTransform_R.Length > 0)
                    trackBoneTransform_R[k].transform.position = ColliderCenterPoint - (wheelColliders_R[k].transform.up * (wheelColliders_R[k].suspensionDistance + wheelColliders_R[k].radius - trackOffset)) * transform.localScale.y;
            }

           
            rotationValueR[k] += wheelColliders_R[k].rpm * (6) * Time.deltaTime;

        }

        //Left Wheels Transform.
        for (int i = 0; i < wheelColliders_L.Length; i++)
        {

            Vector3 ColliderCenterPoint = wheelColliders_L[i].transform.TransformPoint(wheelColliders_L[i].center);

            if (Physics.Raycast(ColliderCenterPoint, -wheelColliders_L[i].transform.up, out hit, (wheelColliders_L[i].suspensionDistance + wheelColliders_L[i].radius) * transform.localScale.y) && hit.transform.root != transform)
            {
                wheelTransform_L[i].transform.position = hit.point + (wheelColliders_L[i].transform.up * wheelColliders_L[i].radius) * transform.localScale.y;
                if (trackBoneTransform_L.Length > 0)
                    trackBoneTransform_L[i].transform.position = hit.point + (wheelColliders_L[i].transform.up * trackOffset) * transform.localScale.y;
            }
            else
            {
                wheelTransform_L[i].transform.position = ColliderCenterPoint - (wheelColliders_L[i].transform.up * wheelColliders_L[i].suspensionDistance) * transform.localScale.y;
                if (trackBoneTransform_L.Length > 0)
                    trackBoneTransform_L[i].transform.position = ColliderCenterPoint - (wheelColliders_L[i].transform.up * (wheelColliders_L[i].suspensionDistance + wheelColliders_L[i].radius - trackOffset)) * transform.localScale.y;
            }

            wheelTransform_L[i].transform.rotation = wheelColliders_L[i].transform.rotation * Quaternion.Euler(rotationValueL[Mathf.CeilToInt((wheelColliders_L.Length) / 2)], 0, 0);
            rotationValueL[i] += wheelColliders_L[i].rpm * (6) * Time.deltaTime;

        }

        //Scrolling Track Texture Offset.
        if (leftTrackMesh && rightTrackMesh)
        {
            leftTrackMesh.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((rotationValueL[Mathf.CeilToInt((wheelColliders_L.Length) / 2)] / 1000) * trackScrollSpeedMultiplier, 0));
            rightTrackMesh.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((rotationValueR[Mathf.CeilToInt((wheelColliders_R.Length) / 2)] / 1000) * trackScrollSpeedMultiplier, 0));
            leftTrackMesh.GetComponent<Renderer>().material.SetTextureOffset("_BumpMap", new Vector2((rotationValueL[Mathf.CeilToInt((wheelColliders_L.Length) / 2)] / 1000) * trackScrollSpeedMultiplier, 0));
            rightTrackMesh.GetComponent<Renderer>().material.SetTextureOffset("_BumpMap", new Vector2((rotationValueR[Mathf.CeilToInt((wheelColliders_R.Length) / 2)] / 1000) * trackScrollSpeedMultiplier, 0));
        }

    }

    #endregion Gears&Wheels

    #region Smoke
    void Smoke()
    {

        if (wheelParticles.Count > 0)
        {

            for (int i = 0; i < allWheelColliders.Count; i++)
            {

                WheelHit CorrespondingGroundHit;
                allWheelColliders[i].GetGroundHit(out CorrespondingGroundHit);

                if (speed > 1f && allWheelColliders[i].isGrounded)
                {
                    wheelParticles[i].enableEmission = true;
                    wheelParticles[i].emissionRate = Mathf.Lerp(0f, 10f, speed / 25f);
                }
                else
                {
                    wheelParticles[i].enableEmission = false;
                }

            }

        }

    }
    #endregion Smoke

    #region IFSTATS
#if !UNITY_MIN_5_4
    /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
    void OnLevelWasLoaded(int level)
    {
        this.CalledOnLevelWasLoaded(level);
    }
#endif


    void CalledOnLevelWasLoaded(int level)
    {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }
    }
    #endregion IFSTATS
}