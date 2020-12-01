using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
namespace _Scripts.Controls
{
    public class SimulatedInput : UserInputs.ITankActions
    {
        #region Variables & Init

        private static readonly Dictionary<InputCodes, float> AxesValues = new Dictionary<InputCodes, float>();

        private static readonly Dictionary<InputCodes, SimButtonControl> BtnValues =
            new Dictionary<InputCodes, SimButtonControl>();

        private static readonly Dictionary<InputCodes, bool> Simulated = new Dictionary<InputCodes, bool>();

        private static SimulatedInput _instance;

        private SimulatedInput()
        {
            var inputs = new UserInputs();

            inputs.Enable();

            inputs.Tank.Enable();
            inputs.Tank.SetCallbacks(this);
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            _instance = new SimulatedInput();
        }

        #endregion

        #region Set Values

        public static void SetAxis(InputCodes name, float value, bool simulated)
        {
           // Debug.Log("SetAxis name, value: " + name.ToString() + ", " + value ); 
            if (Simulated.ContainsKey(name))
            {
                if (Simulated[name] && !simulated) return;
                if (!Simulated[name] && simulated) return;
            }

            AxesValues[name] = value;
        }

        public static void SetButton(InputCodes name, SimButtonControl bc, bool simulated)
        {
            if (Simulated.ContainsKey(name))
            {
                if (Simulated[name] && !simulated) return;
                if (!Simulated[name] && simulated) return;
            }

            BtnValues[name] = bc;
        }

        #endregion

        #region API Calls

        public static float GetAxis(InputCodes name)
        {
            if (AxesValues.ContainsKey(name))
                return AxesValues[name];

            return 0;
        }

        public static bool GetButtonDown(InputCodes name)
        {
            //GameObject.Find("DBG").GetComponent<Text>().text = "BtnValues.ContainsKey(name): " + "\nBtnValues[name].down: "+ BtnValues[name].down;

            return BtnValues.ContainsKey(name) && BtnValues[name].down;
        }

        public static bool GetButton(InputCodes name)
        {
            return BtnValues.ContainsKey(name) && BtnValues[name].pressed;
        }

        public static bool GetButtonUp(InputCodes name)
        {
            return BtnValues.ContainsKey(name) && BtnValues[name].up;
        }

        public static void SimulateInput(InputCodes name, bool value)
        {
            Simulated[name] = value;
        }

        #endregion

        #region Tank Input Callbacks

        public void OnMove(InputAction.CallbackContext context)
        {
            var v = context.ReadValue<Vector2>();
            Debug.Log("OnMove v:" + v);

            SetAxis(InputCodes.TankMoveX, v.x, false);
            SetAxis(InputCodes.TankMoveY, v.y, false);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            var v = context.ReadValue<Vector2>();

            SetAxis(InputCodes.MouseLookX, v.x, false);
            SetAxis(InputCodes.MouseLookY, v.y, false);
        }

        public void OnRecenter(InputAction.CallbackContext context)
        {
            if (!(context.control is ButtonControl btn)) return;

            var s = new SimButtonControl
            {
                down = btn.wasPressedThisFrame, pressed = btn.isPressed, up = btn.wasReleasedThisFrame
            };

            SetButton(InputCodes.Recenter, s, false);
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (!(context.control is ButtonControl btn)) return;

            var s = new SimButtonControl
            {
                down = btn.wasPressedThisFrame, pressed = btn.isPressed, up = btn.wasReleasedThisFrame
            };

            SetButton(InputCodes.TankFire, s, false);
        }

        #endregion
    }

    [Serializable]
    public enum InputCodes
    {
        MouseLookX,
        MouseLookY,
        TankMoveX,
        TankMoveY,
        Recenter,
        TankFire
    }

    [Serializable]
    public struct SimButtonControl
    {
        public bool down, pressed, up;
    }
}