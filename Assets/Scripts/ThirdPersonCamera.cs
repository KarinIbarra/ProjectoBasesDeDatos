using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float mouseSensitivity = 3f;
    public float distance = 5f;
    public float minDistance = 1f; // distancia mínima cuando hay pared
    public Vector3 offset;
    public Vector2 pitchMinMax = new Vector2(-40, 85);

    private float yaw;
    private float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        transform.eulerAngles = new Vector3(pitch, yaw);

        Vector3 focusPosition = target.position + offset;

        // Dirección hacia atrás (donde debería estar la cámara)
        Vector3 desiredPosition = focusPosition - transform.forward * distance;

        RaycastHit hit;

        // Raycast desde el jugador hacia la cámara
        if (Physics.Raycast(focusPosition, -transform.forward, out hit, distance))
        {
            // Si golpea algo, acercamos la cámara
            float adjustedDistance = Mathf.Clamp(hit.distance, minDistance, distance);
            transform.position = focusPosition - transform.forward * adjustedDistance;
        }
        else
        {
            // Si no hay obstáculos, usamos la distancia normal
            transform.position = desiredPosition;
        }
    }
}
