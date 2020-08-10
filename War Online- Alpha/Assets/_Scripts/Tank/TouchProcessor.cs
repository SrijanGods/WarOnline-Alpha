﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchProcessor : MonoBehaviour
{
    public FixedJoystick fixedJoystick;
    public FixedButton fixedButton;
    public FixedTouchField touchField;

    [HideInInspector]
    public Vector2 lookAxis;

    [HideInInspector]
    public bool fire;

    private RTCTankController tankController;
    // Start is called before the first frame update
    void Start()
    {
        tankController = gameObject.GetComponent<RTCTankController>();
    }

    // Update is called once per frame
    void Update()
    {
        tankController.moveAxis.x = fixedJoystick.Horizontal;
        tankController.moveAxis.y = fixedJoystick.Vertical;

        lookAxis = touchField.TouchDist;
        fire = fixedButton.Pressed;
    }
}
