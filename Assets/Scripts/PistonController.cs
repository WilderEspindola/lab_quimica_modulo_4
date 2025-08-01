using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PistonController : MonoBehaviour
{
    [Header("Configuración del Pistón")]
    public Transform piston; // Asigna el objeto del pistón
    public float moveSpeed = 1.0f; // Velocidad de movimiento

    [Header("Límites del Pistón")]
    public float minHeight = -22.21f; // Límite inferior (bajar)
    public float maxHeight = 0.05f; // Límite superior (subir)

    private bool isMovingDown = false;
    private bool isMovingUp = false;

    void Update()
    {
        // Mover el pistón hacia abajo
        if (isMovingDown && piston.localPosition.y > minHeight)
        {
            Vector3 newPosition = piston.localPosition;
            newPosition.y -= moveSpeed * Time.deltaTime;
            newPosition.y = Mathf.Max(newPosition.y, minHeight); // Aplicar límite inferior
            piston.localPosition = newPosition;
        }

        // Mover el pistón hacia arriba
        if (isMovingUp && piston.localPosition.y < maxHeight)
        {
            Vector3 newPosition = piston.localPosition;
            newPosition.y += moveSpeed * Time.deltaTime;
            newPosition.y = Mathf.Min(newPosition.y, maxHeight); // Aplicar límite superior
            piston.localPosition = newPosition;
        }
    }

    // Métodos públicos para control desde los botones
    public void StartMovingDown()
    {
        isMovingDown = true;
        isMovingUp = false;
    }

    public void StopMovingDown()
    {
        isMovingDown = false;
    }

    public void StartMovingUp()
    {
        isMovingUp = true;
        isMovingDown = false;
    }

    public void StopMovingUp()
    {
        isMovingUp = false;
    }
}