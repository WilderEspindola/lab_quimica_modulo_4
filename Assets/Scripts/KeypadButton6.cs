using UnityEngine;

public class KeypadButton6 : MonoBehaviour
{
    public KeypadLock6 keypadLock6;
    public string digitOrAction;

    public void PressButton()
    {
        if (digitOrAction == "Enter") keypadLock6.SaveCode();
        else if (digitOrAction == "Clear") keypadLock6.ClearCode();
        else keypadLock6.AddDigit(digitOrAction);
    }
}