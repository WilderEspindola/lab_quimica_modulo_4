using UnityEngine;

public class CubeRiser : MonoBehaviour
{
    [Tooltip("Cantidad que sube el cubo en Y (por clic).")]
    public float riseAmount = 0.1f; // Ajustable en el Inspector

    [Tooltip("Altura máxima permitida (Y no puede ser mayor a esto).")]
    public float maxYPosition = 0.6f; // Límite superior en Y

    private Vector3 originalPosition; // Guarda la posición inicial

    void Start()
    {
        originalPosition = transform.position; // Posición exacta inicial
    }

    // Método para subir el cubo (llamado por el botón UI)
    public void MoveCubeUp()
    {
        Vector3 currentPosition = transform.position;

        // Calcula la nueva posición Y (sumando riseAmount, pero no más de maxYPosition)
        float newY = Mathf.Min(currentPosition.y + riseAmount, maxYPosition);

        // Aplica el movimiento SOLO en Y (mantiene X y Z originales)
        transform.position = new Vector3(currentPosition.x, newY, currentPosition.z);
    }

    // (Opcional) Resetear a la posición inicial
    public void ResetPosition()
    {
        transform.position = originalPosition;
    }
}