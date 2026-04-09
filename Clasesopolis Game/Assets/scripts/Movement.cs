using UnityEngine;

public class Movement : MonoBehaviour
{
    public float velocidad = 5f;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movimiento = new Vector3(x, 0, z);
        transform.Translate(movimiento * velocidad * Time.deltaTime, Space.Self);
    }
}
    