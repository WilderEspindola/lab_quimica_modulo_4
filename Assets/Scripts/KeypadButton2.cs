using UnityEngine;

public class KeypadButton2 : MonoBehaviour
{
    public KeypadLock2 keypadLock2; // Referencia al KeypadLock2 específico
    public string digitOrAction; // Ej: "1", "Clear", "Enter"

    public void PressButton()
    {
        if (digitOrAction == "Enter")
        {
            keypadLock2.SaveCode();
        }
        else if (digitOrAction == "Clear")
        {
            keypadLock2.ClearCode();
        }
        else
        {
            keypadLock2.AddDigit(digitOrAction);
        }
    }
}