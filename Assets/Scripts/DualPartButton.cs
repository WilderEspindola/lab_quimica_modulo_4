using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSimpleInteractable))]
public class DualPartButton : MonoBehaviour
{
    [Header("Configuraci�n del Bot�n")]
    public Transform[] movingParts;       // Asignar pCylinder96 y pCylinder97
    public float pressedZPosition = -0.00012f;
    public float returnSpeed = 25f;

    [Header("Feedback H�ptico")]
    public bool hapticFeedback = true;
    [Range(0, 1)] public float hapticIntensity = 0.3f;
    public float hapticDuration = 0.1f;

    [Header("Control del Pist�n")]
    public Transform piston;
    public float pistonSpeed = 0.5f;
    public float minPistonHeight = -22.21f;
    public float maxPistonHeight = 0.05f;
    public bool isOnButton = true;  // True para buttonon (bajar pist�n), False para buttonoff (subir pist�n)

    private Vector3[] originalPositions;
    private bool isPressed = false;

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
    }

    void Update()
    {
        // Movimiento del bot�n
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

        // Control del pist�n
        if (isPressed && piston != null)
        {
            ControlPistonMovement();
        }
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        isPressed = true;

        // Posici�n exacta inmediata
        for (int i = 0; i < movingParts.Length; i++)
        {
            Vector3 newPos = movingParts[i].localPosition;
            newPos.z = pressedZPosition;
            movingParts[i].localPosition = newPos;
        }

        // Vibraci�n
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
        float direction = isOnButton ? -1f : 1f; // Direcci�n basada en isOnButton

        Vector3 newPos = piston.localPosition;
        newPos.y = Mathf.Clamp(
            newPos.y + (direction * pistonSpeed * Time.deltaTime),
            minPistonHeight,
            maxPistonHeight
        );
        piston.localPosition = newPos;
    }
}