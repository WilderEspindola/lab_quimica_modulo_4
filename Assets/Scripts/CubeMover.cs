using UnityEngine;

public class CubeMover : MonoBehaviour
{
    [Tooltip("Cantidad fija que se mueve el cubo en Y (por clic).")]
    public float moveAmount = 0.1f; // Afecta tanto subida como bajada

    // Bajar el cubo (resta moveAmount en Y)
    public void MoveCubeDown()
    {
        transform.position -= new Vector3(0, moveAmount, 0);
    }

    // Subir el cubo (suma moveAmount en Y)
    public void MoveCubeUp()
    {
        transform.position += new Vector3(0, moveAmount, 0);
    }

    // (Opcional) Resetear a la posición inicial exacta
    public void ResetPosition()
    {
        transform.position = new Vector3(2.979f, 0.692f, 0.57f);
    }
}