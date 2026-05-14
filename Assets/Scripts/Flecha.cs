using UnityEngine;

public class GuideArrow : MonoBehaviour
{
    public Transform player;    // Referencia al jugador
    public Transform exitPoint; // Referencia a donde está la puerta
    public float heightOffset = 2.5f;

    void Update()
    {
        if (player == null || exitPoint == null) return;

        // Sigue al jugador suavemente
        transform.position = player.position + Vector3.up * heightOffset;

        // Mira hacia la salida (solo en el eje horizontal)
        Vector3 direction = exitPoint.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
