using UnityEngine;

public class KeypadButton : MonoBehaviour
{
    public KeypadLock keypadLock;
    public string digitOrAction;

    public void PressButton()
    {
        if (digitOrAction == "Enter")
        {
            keypadLock.SaveCode(); // Este método ahora hace más cosas, pero la llamada es igual
        }
        else if (digitOrAction == "Clear")
        {
            keypadLock.ClearCode();
        }
        else
        {
            keypadLock.AddDigit(digitOrAction);
        }
    }
}