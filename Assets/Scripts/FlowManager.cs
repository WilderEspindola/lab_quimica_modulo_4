using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FlowManager : MonoBehaviour
{
    // Variables para coeficientes (A-F)
    public int A, B, C, D, E, F;

    // Sistema de sockets y gestos
    public List<GameObject> Sockets = new List<GameObject>();
    public GameObject HandAnimation;
    public List<GameObject> Keypads = new List<GameObject>(); // Arrastra todos los keypads aquí en el Inspector
    public List<GameObject> PasscodeDisplays = new List<GameObject>(); // Arrastrar displays en Inspector
    public List<GameObject> Cubes = new List<GameObject>(); // Arrastrar los 6 cubos en el Inspector

    // Variables para ecuaciones químicas
    private ReactionManager.SerializableChemicalEquation currentEquation;
    private Dictionary<char, string> variableAssignments = new Dictionary<char, string>();
    private Dictionary<char, int> keypadValues = new Dictionary<char, int>();

    void Start()
    {
        InitializeKeypadValues();
        InitializeGame();
        KeypadLock.OnKeypadValueChanged += UpdateKeypadValue;    // A
        KeypadLock2.OnKeypadValueChanged += UpdateKeypadValue;   // B
        KeypadLock3.OnKeypadValueChanged += UpdateKeypadValue;   // C
        KeypadLock4.OnKeypadValueChanged += UpdateKeypadValue;   // D
        KeypadLock5.OnKeypadValueChanged += UpdateKeypadValue;   // E
        KeypadLock6.OnKeypadValueChanged += UpdateKeypadValue;   // F
    }

    private void InitializeKeypadValues()
    {
        keypadValues.Clear();
        keypadValues.Add('A', 1);
        keypadValues.Add('B', 1);
        keypadValues.Add('C', 1);
        keypadValues.Add('D', 1);
        keypadValues.Add('E', 1);
        keypadValues.Add('F', 1);

        // Actualizar variables de coeficientes
        A = B = C = D = E = F = 1;
    }

    private void InitializeGame()
    {
        // Mensajes y UI básica
        GameManager.Instance.UI_Messages.text = "Usa ✋ Thumbs Up para practicar o 🤙 Shaka para comenzar";
        GameManager.Instance.Timer.enabled = false;
        GameManager.Instance.MathematicsValues.gameObject.SetActive(false);

        // Gestos
        GameManager.Instance.RightThumbsUp.gameObject.SetActive(true);
        GameManager.Instance.RightShaka.gameObject.SetActive(true);
        GameManager.Instance.LeftThumbsUp.gameObject.SetActive(false);

        // Ocultación completa de elementos interactivos
        DisableSockets();
        HandAnimation.SetActive(false);

        // Ocultar TODOS los keypads y sus displays
        foreach (var keypad in Keypads)
        {
            if (keypad != null) keypad.SetActive(false);
        }

        foreach (var display in PasscodeDisplays)
        {
            if (display != null) display.SetActive(false);
        }
        // Ocultar cubos al inicio
        foreach (var cube in Cubes)
        {
            if (cube != null) cube.SetActive(false);
        }
    }

    // --- Gestos ---
    public void RightHandThumpsUpPerformed()
    {

        GameManager.Instance.UI_Messages.text = "Modo Práctica: Usa los keypads. ✋ Thumbs Up izquierdo para salir.";
        GameManager.Instance.RightThumbsUp.gameObject.SetActive(false);
        GameManager.Instance.RightShaka.gameObject.SetActive(false);
        GameManager.Instance.LeftThumbsUp.gameObject.SetActive(true);
        GameManager.Instance.MathematicsValues.gameObject.SetActive(true);
        EnableSockets();
        HandAnimation.SetActive(true);
        // Mostrar TODOS los keypads y displays en modo práctica
        foreach (var keypad in Keypads)
        {
            if (keypad != null) keypad.SetActive(true);
        }

        foreach (var display in PasscodeDisplays)
        {
            if (display != null) display.SetActive(true);
        }
        // Mostrar cubos SOLO en práctica
        foreach (var cube in Cubes)
        {
            if (cube != null) cube.SetActive(true);
        }

    }

    public void RightShakaPerformed()
    {

        GameManager.Instance.UI_Messages.text = "¡Balancea la ecuación!";
        GameManager.Instance.RightThumbsUp.gameObject.SetActive(false);
        GameManager.Instance.RightShaka.gameObject.SetActive(false);
        GameManager.Instance.MathematicsValues.gameObject.SetActive(true);
        GameManager.Instance.LeftThumbsUp.gameObject.SetActive(false); // Ocultar hasta terminar
        StartNewChemicalEquation();
        StartCountdown();
        // Ocultar cubos en modo juego
        foreach (var cube in Cubes)
        {
            if (cube != null) cube.SetActive(false);
        }
    }

    public void LeftHandThumpsUpPerformed()
    {
        RestartScene();
    }

    // --- Temporizador ---
    public void StartCountdown()
    {
        GameManager.Instance.Timer.enabled = true;
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        int countdownTime = 20; // para cambiar el tiempo de la prueba
        while (countdownTime >= 0)
        {
            GameManager.Instance.Timer.GetComponent<TextMeshProUGUI>().text = countdownTime.ToString();
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }
        OnCountdownFinished();
    }

    private void OnCountdownFinished()
    {
        GameManager.Instance.Timer.enabled = false;
        CheckChemicalBalance();
        // Activar gesto de salida siempre al terminar el tiempo
        GameManager.Instance.LeftThumbsUp.gameObject.SetActive(true);


    }

    // --- Sistema Químico ---
    private void StartNewChemicalEquation()
    {
        currentEquation = ReactionManager.Instance.GetRandomEquation();
        if (currentEquation == null)
        {
            Debug.LogError("No se pudo obtener ecuación aleatoria");
            return;
        }

        AssignVariablesToCompounds();
        UpdateChemicalEquationUI();
        ResetCoefficients();
        UpdateVariableDisplays();
    }

    private void AssignVariablesToCompounds()
    {
        variableAssignments.Clear();

        // Reactivos (A, B, C)
        for (int i = 0; i < currentEquation.reactants.Count; i++)
        {
            char variable = (char)('A' + i);
            variableAssignments[variable] = currentEquation.reactants[i].formula;
        }

        // Productos (D, E, F)
        for (int i = 0; i < currentEquation.products.Count; i++)
        {
            char variable = (char)('D' + i);
            variableAssignments[variable] = currentEquation.products[i].formula;
        }
    }

    private void UpdateChemicalEquationUI()
    {
        // 1. Obtener el tipo de reacción en mayúsculas
        string reactionType = currentEquation.reactionType.ToUpper();

        // 2. Construir la ecuación sin placeholders
        string equationText = "";

        // Reactivos
        for (int i = 0; i < currentEquation.reactants.Count; i++)
        {
            equationText += currentEquation.reactants[i].formula + " + ";
        }
        equationText = equationText.TrimEnd(' ', '+') + " → ";

        // Productos
        for (int i = 0; i < currentEquation.products.Count; i++)
        {
            equationText += currentEquation.products[i].formula + " + ";
        }
        equationText = equationText.TrimEnd(' ', '+');

        // 3. Formato final centrado con jerarquía visual
        GameManager.Instance.UI_Messages.text =
            $"<align=\"center\">" +
            $"<color=#000000>Tipo de ecuación</color>\n" +
            $"<b>{reactionType}</b>\n" +
            $"<color=#000000>Ecuación</color>\n" +
            $"{equationText}" +
            $"</align>";
    }

    private void UpdateVariableDisplays()
    {
        Transform valuesParent = GameManager.Instance.MathematicsValues.transform;
        int activeReactants = currentEquation.reactants.Count;
        int emptySlots = 3 - activeReactants; // 0, 1 o 2 espacios vacíos

        // 1. Resetear todos los elementos
        for (int i = 0; i < valuesParent.childCount; i++)
            valuesParent.GetChild(i).gameObject.SetActive(false);

        foreach (var keypad in Keypads) keypad?.SetActive(false);
        foreach (var display in PasscodeDisplays) display?.SetActive(false);

        // 2. Procesar REACTIVOS (A, B, C)
        for (int slot = 0; slot < 3; slot++)
        {
            bool isActive = slot < activeReactants;
            int uiIndex = slot * 2; // Índice para fórmula/signo

            if (isActive)
            {
                // --- FÓRMULA QUÍMICA ---
                TextMeshPro formulaText = valuesParent.GetChild(uiIndex).GetComponent<TextMeshPro>();
                formulaText.text = currentEquation.reactants[slot].formula;
                formulaText.gameObject.SetActive(true);
                formulaText.transform.localPosition += new Vector3(0.5236f * emptySlots, 0, 0);

                // --- DISPLAY NUMÉRICO (POSICIONES CORREGIDAS) ---
                if (slot < PasscodeDisplays.Count)
                {
                    PasscodeDisplays[slot].SetActive(true);
                    Vector3 displayPos = PasscodeDisplays[slot].transform.localPosition;

                    if (emptySlots == 1) // 1 espacio vacío (ej: A + B → ...)
                    {
                        displayPos.x = -0.021f;
                        displayPos.z = -0.877f;
                    }
                    else if (emptySlots == 2) // 2 espacios vacíos (ej: A → ...)
                    {
                        displayPos.x = -0.922f;
                        displayPos.z = -0.9077f;
                    }

                    PasscodeDisplays[slot].transform.localPosition = displayPos;
                }

                // --- KEYPAD ---
                if (slot < Keypads.Count)
                {
                    Keypads[slot].SetActive(true);
                    Vector3 keypadPos = Keypads[slot].transform.localPosition;

                    if (emptySlots == 1) // 1 espacio vacío
                    {
                        keypadPos.x = -0.247f;
                        keypadPos.z = -0.921f;
                    }
                    else if (emptySlots == 2) // 2 espacios vacíos
                    {
                        keypadPos.x = -1.163f;
                        keypadPos.z = -0.939f;
                    }

                    Keypads[slot].transform.localPosition = keypadPos;
                }

                // --- SIGNO "+" ---
                if (slot < activeReactants - 1)
                {
                    GameObject plusSign = valuesParent.GetChild(uiIndex + 1).gameObject;
                    plusSign.SetActive(true);
                    plusSign.transform.localPosition += new Vector3(0.5236f * emptySlots, 0, 0);
                }
            }
        }

        // 3. FLECHA (→) - POSICIÓN FIJA (índice 5)
        valuesParent.GetChild(5).gameObject.SetActive(true);

        // 4. PRODUCTOS (D, E, F) - POSICIONES FIJAS
        for (int slot = 0; slot < currentEquation.products.Count; slot++)
        {
            int uiIndex = 6 + slot * 2;
            TextMeshPro formulaText = valuesParent.GetChild(uiIndex).GetComponent<TextMeshPro>();
            formulaText.text = currentEquation.products[slot].formula;
            formulaText.gameObject.SetActive(true);

            int keypadIndex = 3 + slot;
            if (keypadIndex < Keypads.Count) Keypads[keypadIndex].SetActive(true);
            if (keypadIndex < PasscodeDisplays.Count) PasscodeDisplays[keypadIndex].SetActive(true);

            if (slot < currentEquation.products.Count - 1)
            {
                valuesParent.GetChild(uiIndex + 1).gameObject.SetActive(true);
            }
        }
    }

    private void ResetCoefficients()
    {
        InitializeKeypadValues(); // Reutilizamos esta función para resetear todo

        Debug.Log("Coeficientes reseteados: " +
                 $"A={A}, B={B}, C={C}, D={D}, E={E}, F={F}");
    }

    // --- Keypads y validación ---
    private void UpdateKeypadValue(char variable, int value)
    {
        if (!keypadValues.ContainsKey(variable))
        {
            GameManager.Instance.UI_Messages.text += $"\nERROR: Variable {variable} no existe";
            return;
        }

        keypadValues[variable] = value;
        UpdateCoefficientVariable(variable, value);

        // Actualizar la interfaz de usuario
        UpdateKeypadsStatusMessage(); // ¡Esta línea faltaba!

        // Verificar balance siempre (ya no hay distinción por modo)
        CheckChemicalBalance();
    }

    private void UpdateKeypadsStatusMessage()
    {
        // 1. Crear el mensaje de estado limpio
        string statusMessage = "[Coeficientes Actuales]\n";
        statusMessage += $"Reactivos: A={keypadValues['A']}  B={keypadValues['B']}  C={keypadValues['C']}\n";
        statusMessage += $"Productos: D={keypadValues['D']}  E={keypadValues['E']}  F={keypadValues['F']}";

        // 2. Obtener el texto actual sin el estado previo
        string currentText = GameManager.Instance.UI_Messages.text;
        int lastStatusIndex = currentText.IndexOf("[Coeficientes Actuales]");

        string newText = lastStatusIndex >= 0
            ? currentText.Substring(0, lastStatusIndex)
            : currentText;

        // 3. Limitar el historial de mensajes
        string[] messageLines = newText.Split('\n');
        if (messageLines.Length > 8) // Mantener solo las últimas 8 líneas
        {
            newText = string.Join("\n", messageLines.Skip(messageLines.Length - 8));
        }

        // 4. Actualizar UI con el nuevo estado
        GameManager.Instance.UI_Messages.text = $"{newText.Trim()}\n\n{statusMessage}";
    }

    private void UpdateCoefficientVariable(char variable, int value)
    {
        switch (variable)
        {
            case 'A': A = value; break;
            case 'B': B = value; break;
            case 'C': C = value; break;
            case 'D': D = value; break;
            case 'E': E = value; break;
            case 'F': F = value; break;
            default:
                Debug.LogError($"Variable no reconocida en UpdateCoefficientVariable: {variable}");
                break;
        }
    }

    private void CheckChemicalBalance()
    {
        // 1. Inicializar mensaje de diagnóstico
        string debugMessage = "=== Validación ===\n";
        debugMessage += $"Usando coeficientes: A={keypadValues['A']}, B={keypadValues['B']}, " +
                      $"C={keypadValues['C']}, D={keypadValues['D']}, " +
                      $"E={keypadValues['E']}, F={keypadValues['F']}\n";

        // 2. Verificar ecuación existente
        if (currentEquation == null)
        {
            //GameManager.Instance.UI_Messages.text = "Error: No hay ecuación activa";
            return;
        }

        // 3. Inicializar diccionarios
        Dictionary<string, int> reactantsAtoms = new Dictionary<string, int>();
        Dictionary<string, int> productsAtoms = new Dictionary<string, int>();

        // 4. Procesar reactivos (A, B, C)
        debugMessage += "Reactivos:\n";
        for (int i = 0; i < currentEquation.reactants.Count; i++)
        {
            char variable = (char)('A' + i);
            int coefficient = keypadValues.ContainsKey(variable) ? keypadValues[variable] : 1;

            debugMessage += $"{variable}={coefficient}*{currentEquation.reactants[i].formula}\n";
            AddCompoundAtoms(currentEquation.reactants[i], coefficient, reactantsAtoms);
        }

        // 5. Procesar productos (D, E, F)
        debugMessage += "Productos:\n";
        for (int i = 0; i < currentEquation.products.Count; i++)
        {
            char variable = (char)('D' + i);
            int coefficient = keypadValues.ContainsKey(variable) ? keypadValues[variable] : 1;

            debugMessage += $"{variable}={coefficient}*{currentEquation.products[i].formula}\n";
            AddCompoundAtoms(currentEquation.products[i], coefficient, productsAtoms);
        }

        // 6. Mostrar conteo de átomos
        debugMessage += "Átomos en reactivos:\n";
        foreach (var atom in reactantsAtoms)
            debugMessage += $"{atom.Key}:{atom.Value}\n";

        debugMessage += "Átomos en productos:\n";
        foreach (var atom in productsAtoms)
            debugMessage += $"{atom.Key}:{atom.Value}\n";

        // 7. Comparar
        bool isBalanced = true;
        foreach (var atom in reactantsAtoms)
        {
            if (!productsAtoms.ContainsKey(atom.Key) || productsAtoms[atom.Key] != atom.Value)
            {
                isBalanced = false;
                break;
            }
        }

        // Verificar también átomos que solo estén en productos
        foreach (var atom in productsAtoms)
        {
            if (!reactantsAtoms.ContainsKey(atom.Key))
            {
                isBalanced = false;
                break;
            }
        }

        // 8. Resultado final
        debugMessage += isBalanced ? "✅ BALANCEADA" : "❌ DESBALANCEADA";
        GameManager.Instance.UI_Messages.text = debugMessage;

        // Activar gesto si está balanceada
        if (isBalanced)
        {
            GameManager.Instance.LeftThumbsUp.gameObject.SetActive(true);
            GameManager.Instance.Timer.enabled = false; // Detener temporizador si se balanceó antes
        }
    }

    private void AddCompoundAtoms(ReactionManager.SerializableCompound compound, int coefficient, Dictionary<string, int> targetDict)
    {
        if (compound.composition == null)
        {
            GameManager.Instance.UI_Messages.text += "\nERROR: Composición nula";
            return;
        }

        foreach (var atom in compound.composition)
        {
            if (targetDict.ContainsKey(atom.atom))
                targetDict[atom.atom] += coefficient * atom.count;
            else
                targetDict[atom.atom] = coefficient * atom.count;
        }
    }

    // --- Métodos auxiliares ---
    private void DisableSockets()
    {
        foreach (var socket in Sockets)
        {
            if (socket != null)
            {
                socket.SetActive(false);
                socket.GetComponent<XRSocketInteractor>().enabled = false;
            }
        }
    }

    private void EnableSockets()
    {
        foreach (var socket in Sockets)
        {
            if (socket != null)
            {
                socket.SetActive(true);
                socket.GetComponent<XRSocketInteractor>().enabled = true;
            }
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        KeypadLock.OnKeypadValueChanged -= UpdateKeypadValue;    // A
        KeypadLock2.OnKeypadValueChanged -= UpdateKeypadValue;   // B
        KeypadLock3.OnKeypadValueChanged -= UpdateKeypadValue;   // C
        KeypadLock4.OnKeypadValueChanged -= UpdateKeypadValue;   // D
        KeypadLock5.OnKeypadValueChanged -= UpdateKeypadValue;   // E
        KeypadLock6.OnKeypadValueChanged -= UpdateKeypadValue;   // F
    }

}