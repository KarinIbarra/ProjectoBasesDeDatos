using UnityEngine;
using System.Collections;

public class SlidingDoor : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public Vector3 moveOffset = new Vector3(3f, 0f, 0f); // Cuánto se desplaza (ej: 3 metros en X)
    public float openSpeed = 2f; // Velocidad del movimiento

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpening = false;

    void Start()
    {
        // Guardamos la posición inicial como la "cerrada"
        closedPosition = transform.position;
        // Calculamos la posición final sumando el offset
        openPosition = closedPosition + moveOffset;
    }

    // Método que llamará el LevelManager
    public void Open()
    {
        if (!isOpening)
        {
            StopAllCoroutines();
            StartCoroutine(MoveDoor());
        }
    }

    IEnumerator MoveDoor()
    {
        isOpening = true;
        while (Vector3.Distance(transform.position, openPosition) > 0.01f)
        {
            // Movemos la puerta suavemente hacia la posición abierta
            transform.position = Vector3.MoveTowards(
                transform.position,
                openPosition,
                openSpeed * Time.deltaTime
            );
            yield return null;
        }
        transform.position = openPosition;
        Debug.Log("La puerta de salida está abierta.");
    }
}
