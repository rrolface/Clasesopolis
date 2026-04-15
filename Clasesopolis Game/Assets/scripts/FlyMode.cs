using UnityEngine;

public class FlyMode : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadVuelo = 8f;
    public float velocidadRapida = 20f;

    [Header("Referencias")]
    public Movement scriptMovimiento;
    public Transform camara;

    private Rigidbody rb;
    private bool modoLibre = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Activar / desactivar con TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ActivarDesactivarVuelo();
        }

        if (modoLibre)
            ManejarVuelo();
    }

    void ActivarDesactivarVuelo()
    {
        modoLibre = !modoLibre;

        scriptMovimiento.enabled = !modoLibre;
        rb.useGravity = !modoLibre;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void ManejarVuelo()
    {
        float velocidad = Input.GetKey(KeyCode.LeftShift) ? velocidadRapida : velocidadVuelo;

        Vector3 movimiento = Vector3.zero;
        movimiento += camara.forward * Input.GetAxis("Vertical");
        movimiento += camara.right * Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.E)) movimiento += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) movimiento += Vector3.down;

        transform.position += movimiento * velocidad * Time.deltaTime;
    }
}

