using UnityEngine;
using TMPro;
using System.Collections;

public class KeypadLock4 : MonoBehaviour
{
    [Header("Keypad 4 Config")]
    public TextMeshPro passCodeDisplay;
    public GameObject[] keyButtons;

    private string currentInput = "";  // Cambiado de currentCode a currentInput
    private int savedValue = 1;       // Cambiado de string savedCode="?" a int con valor inicial 1

    // Evento estático (modificado para enviar int en lugar de string)
    public static System.Action<char, int> OnKeypadValueChanged;
    public char associatedLetter = 'D';

    void Start()
    {
        passCodeDisplay.text = savedValue.ToString();  // Mostrar el valor numérico inicial
        SetKeypadVisible(false);
    }

    public void AddDigit(string digit)
    {
        if (currentInput.Length < 2)  // Limitar a 2 dígitos (para coeficientes 1-12)
            currentInput += digit;

        passCodeDisplay.text = currentInput;
    }

    public void SaveCode()
    {
        if (!string.IsNullOrEmpty(currentInput))
        {
            savedValue = int.Parse(currentInput);
            passCodeDisplay.text = savedValue.ToString();
            OnKeypadValueChanged?.Invoke(associatedLetter, savedValue);
        }
        currentInput = "";
        SetKeypadVisible(false);
    }


    // --- Métodos que NO cambiaron ---
    public void ToggleKeypad()
    {
        bool newState = !keyButtons[0].activeSelf;
        SetKeypadVisible(newState);
        if (newState) currentInput = "";
    }

    private void SetKeypadVisible(bool visible)
    {
        foreach (var button in keyButtons)
            button.SetActive(visible);

        passCodeDisplay.text = visible ? currentInput : savedValue.ToString();
    }

    public void ClearCode()
    {
        currentInput = "";
        passCodeDisplay.text = savedValue.ToString();
    }
}