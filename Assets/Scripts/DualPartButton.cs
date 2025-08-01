using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using TMPro;

[RequireComponent(typeof(XRSimpleInteractable))]
public class DualPartButton : MonoBehaviour
{
    [Header("Configuración del Botón")]
    public Transform[] movingParts;
    public float pressedZPosition = -0.00012f;
    public float returnSpeed = 25f;

    [Header("Feedback Háptico")]
    public bool hapticFeedback = true;
    [Range(0, 1)] public float hapticIntensity = 0.3f;
    public float hapticDuration = 0.1f;

    [Header("Control del Pistón")]
    public Transform piston;
    public float pistonSpeed = 0.5f;
    public float minPistonHeight = -22.21f;
    public float maxPistonHeight = 0.05f;
    public bool isOnButton = true;

    [Header("Medidor de Volumen")]
    public TextMeshProUGUI volumeText;
    public RectTransform volumeUI; // El RectTransform del botón UI
    public float maxVolume = 5f;
    public float minVolume = 0f;
    public Vector3 uiOffset = new Vector3(0, 0.1f, 0); // Ajuste de posición relativa al pistón

    private Vector3[] originalPositions;
    private bool isPressed = false;
    private Vector3 initialUIPosition; // Para guardar la posición XZ original

    void Start()
    {
        originalPositions = new Vector3[movingParts.Length];
        for (int i = 0; i < movingParts.Length; i++)
        {
            originalPositions[i] = movingParts[i].localPosition;
        }

        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnButtonPressed);
        interactable.selectExited.AddListener(OnButtonReleased);

        // Guardar la posición inicial del UI (en espacio de mundo)
        if (volumeUI != null)
        {
            initialUIPosition = volumeUI.position;
        }

        UpdateVolumeUI();
    }

    void Update()
    {
        // Movimiento del botón físico
        for (int i = 0; i < movingParts.Length; i++)
        {
            Vector3 targetPos = isPressed ?
                new Vector3(originalPositions[i].x, originalPositions[i].y, pressedZPosition) :
                originalPositions[i];

            movingParts[i].localPosition = Vector3.Lerp(
                movingParts[i].localPosition,
                targetPos,
                returnSpeed * Time.deltaTime
            );
        }

        // Control del pistón
        if (isPressed && piston != null)
        {
            ControlPistonMovement();
            UpdateVolumeUI(); // Actualizar posición y texto
        }
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        isPressed = true;

        for (int i = 0; i < movingParts.Length; i++)
        {
            Vector3 newPos = movingParts[i].localPosition;
            newPos.z = pressedZPosition;
            movingParts[i].localPosition = newPos;
        }

        if (hapticFeedback && args.interactorObject is XRBaseInputInteractor inputInteractor)
        {
            if (inputInteractor.TryGetComponent(out XRController controller))
            {
                controller.SendHapticImpulse(hapticIntensity, hapticDuration);
            }
        }
    }

    private void OnButtonReleased(SelectExitEventArgs args)
    {
        isPressed = false;
    }

    private void ControlPistonMovement()
    {
        float direction = isOnButton ? -1f : 1f;

        Vector3 newPos = piston.localPosition;
        newPos.y = Mathf.Clamp(
            newPos.y + (direction * pistonSpeed * Time.deltaTime),
            minPistonHeight,
            maxPistonHeight
        );
        piston.localPosition = newPos;
    }

    private void UpdateVolumeUI()
    {
        if (volumeText == null || volumeUI == null || piston == null) return;

        // 1. Calcular el volumen
        float normalizedPosition = Mathf.InverseLerp(minPistonHeight, maxPistonHeight, piston.localPosition.y);
        float currentVolume = Mathf.Lerp(minVolume, maxVolume, normalizedPosition);

        // 2. Actualizar el texto
        volumeText.text = $"--- {currentVolume.ToString("0.000")} m³ ---";

        // 3. Mover el UI junto con el pistón (manteniendo XZ original)
        Vector3 newUIPosition = new Vector3(
            initialUIPosition.x,
            piston.position.y + uiOffset.y, // Sigue la Y del pistón con offset
            initialUIPosition.z
        );

        volumeUI.position = newUIPosition;
    }
}