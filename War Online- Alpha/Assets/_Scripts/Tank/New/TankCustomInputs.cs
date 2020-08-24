using _Scripts.Controls;
using UnityEngine;

namespace _Scripts.Tank.New
{
    [RequireComponent(typeof(RTC_TankController))]
    public class TankCustomInputs : MonoBehaviour
    {
        #region Variables & Init

        private RTC_TankController _tankController;

        private void Awake()
        {
            _tankController = GetComponent<RTC_TankController>();
        }

        #endregion

        #region Updating

        private void FixedUpdate()
        {
            _tankController.gasInput = SimulatedInput.GetAxis(InputCodes.TankMoveY);
            _tankController.steerInput = SimulatedInput.GetAxis(InputCodes.TankMoveX);
            _tankController.brakeInput = -Mathf.Clamp(SimulatedInput.GetAxis(InputCodes.TankMoveY), -1f, 0f);
        }

        #endregion
    }
}