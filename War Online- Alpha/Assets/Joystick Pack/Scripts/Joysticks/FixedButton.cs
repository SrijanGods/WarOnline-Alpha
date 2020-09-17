using System.Collections;
using _Scripts.Controls;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [HideInInspector] public bool down, pressed;
    public InputCodes button;

    private void Update()
    {
        SimulatedInput.SetButton(button, new SimButtonControl {down = down, pressed = pressed}, true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;

        SimulatedInput.SimulateInput(button, true);
        down = true;
        StartCoroutine(ResetStateDown());
        SimulatedInput.SetButton(button, new SimButtonControl {down = down, pressed = true}, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;

        SimulatedInput.SetButton(button, new SimButtonControl {down = false, pressed = false, up = true}, true);
        SimulatedInput.SimulateInput(button, false);
    }

    private IEnumerator ResetStateDown()
    {
        yield return new WaitForSeconds(.05f);

        down = false;
    }
}