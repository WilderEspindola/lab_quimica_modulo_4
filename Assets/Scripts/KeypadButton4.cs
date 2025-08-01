using UnityEngine;

public class KeypadButton4 : MonoBehaviour
{
    public KeypadLock4 keypadLock4;
    public string digitOrAction;

    public void PressButton()
    {
        if (digitOrAction == "Enter")
        {
            keypadLock4.SaveCode();
        }
        else if (digitOrAction == "Clear")
        {
            keypadLock4.ClearCode();
        }
        else
        {
            keypadLock4.AddDigit(digitOrAction);
        }
    }
}