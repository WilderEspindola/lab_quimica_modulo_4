using UnityEngine;

public class KeypadButton5 : MonoBehaviour
{
    public KeypadLock5 keypadLock5; // Referencia única al KeypadLock5
    public string digitOrAction; // Ej: "3", "Clear", "Enter"

    public void PressButton()
    {
        if (digitOrAction == "Enter")
        {
            keypadLock5.SaveCode();
        }
        else if (digitOrAction == "Clear")
        {
            keypadLock5.ClearCode();
        }
        else
        {
            keypadLock5.AddDigit(digitOrAction);
        }
    }
}