//----------------------------------------------
//            Realistic Tank Controller
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using _Scripts.Controls;
using UnityEngine.EventSystems;

[AddComponentMenu("BoneCracker Games/Realistic Tank Controller/UI/Drag")]
public class RTC_UIDragController : MonoBehaviour, IDragHandler
{
    // Getting an Instance of Main Shared RTC Settings.

    #region RTC Settings Instance

    private RTC_Settings RTCSettingsInstance;

    private RTC_Settings RTCSettings
    {
        get
        {
            if (RTCSettingsInstance == null)
            {
                RTCSettingsInstance = RTC_Settings.Instance;
            }

            return RTCSettingsInstance;
        }
    }

    #endregion

    internal float verticalInput = 0f;
    internal float horizontal = 0f;

    private float sensitivity
    {
        get { return RTCSettings.UIButtonSensitivity; }
    }

    private float gravity
    {
        get { return RTCSettings.UIButtonGravity; }
    }

    public bool pressing = false;
    public InputCodes horizontalAxis, verticalAxis;

    void Update()
    {
        if (!pressing)
        {
            verticalInput = Mathf.MoveTowards(verticalInput, 0f, Time.deltaTime * gravity);
            horizontal = Mathf.MoveTowards(horizontal, 0f, Time.deltaTime * gravity);
        }

        if (verticalInput < -1f)
            verticalInput = -1f;
        if (verticalInput > 1f)
            verticalInput = 1f;

        if (horizontal < -1f)
            horizontal = -1f;
        if (horizontal > 1f)
            horizontal = 1f;

        SimulatedInput.SetAxis(horizontalAxis, horizontal, true);
        SimulatedInput.SetAxis(verticalAxis, verticalInput, true);
    }

    public void OnDrag(PointerEventData data)
    {
        verticalInput = -data.delta.y * sensitivity * .02f;
        horizontal = -data.delta.x * sensitivity * .02f;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        pressing = true;

        SimulatedInput.SimulateInput(horizontalAxis, true);
        SimulatedInput.SimulateInput(verticalAxis, true);
    }


    public void OnEndDrag(PointerEventData data)
    {
        pressing = false;

        SimulatedInput.SimulateInput(horizontalAxis, false);
        SimulatedInput.SimulateInput(verticalAxis, false);
    }
}