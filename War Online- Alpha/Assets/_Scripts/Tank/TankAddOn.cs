using _Scripts.Controls;
using UnityEngine;

namespace _Scripts.Tank
{
    [RequireComponent(typeof(RTC_TankController))]
    public class TankAddOn : MonoBehaviour
    {
        #region Variables & Init

        public RTC_TankController tankController;

        #endregion

        #region Updating

        private void FixedUpdate()
        {
            tankController.gasInput = SimulatedInput.GetAxis(InputCodes.TankMoveY);
            tankController.steerInput = SimulatedInput.GetAxis(InputCodes.TankMoveX);
            tankController.brakeInput = -Mathf.Clamp(SimulatedInput.GetAxis(InputCodes.TankMoveY), -1f, 0f);
        }

        #endregion
    }
}