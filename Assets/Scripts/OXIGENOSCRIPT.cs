using UnityEngine;

public class OXIGENOSCRIPT : MonoBehaviour
{
    public GameObject infoButton; // Arrastra el Botón 2 (información) aquí en el Inspector

    private bool isInfoVisible = false;

    public void ToggleInfo()
    {
        isInfoVisible = !isInfoVisible; // Alternar estado
        infoButton.SetActive(isInfoVisible); // Mostrar/ocultar
    }
}
