using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensibilidad = 200f;
    public Transform jugador; // el cuerpo (cápsula)

    float rotacionX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // bloquea el mouse al centro
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidad * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidad * Time.deltaTime;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);

        // Rotación vertical (mirar arriba/abajo)
        transform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        // Rotación horizontal (girar jugador)
        jugador.Rotate(Vector3.up * mouseX);
    }
}