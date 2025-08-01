using UnityEngine;

public class KeypadButton3 : MonoBehaviour
{
    public KeypadLock3 keypadLock3; // Referencia exclusiva al KeypadLock3
    public string digitOrAction; // Ej: "5", "Clear", "Enter"

    public void PressButton()
    {
        if (digitOrAction == "Enter")
        {
            keypadLock3.SaveCode();
        }
        else if (digitOrAction == "Clear")
        {
            keypadLock3.ClearCode();
        }
        else
        {
            keypadLock3.AddDigit(digitOrAction);
        }
    }
}