using UnityEngine;
using UnityEngine.SceneManagement;

public class ComprobarMultiple : MonoBehaviour
{
    public DropZoneMultiple zona;

    public GameObject panelCorrecto;
    public GameObject panelIncorrecto;
    public GameObject PanelReto2;

    public int cantidadCorrectosNecesarios = 4;

    public DragItem[] todosLosItems;

    public void Comprobar()
    {
        bool hayError = false;
        int correctosSeleccionados = 0;

        foreach (TipoDato item in zona.items)
        {
            if (item.esCorrecto)
            {
                correctosSeleccionados++;
            }
            else
            {
                hayError = true; // metió uno incorrecto
            }
        }

        // ❗ Validación final
        if (!hayError && correctosSeleccionados == cantidadCorrectosNecesarios)
        {
            panelCorrecto.SetActive(true);
            PanelReto2.SetActive(false);
        }
        else
        {
            panelIncorrecto.SetActive(true);
            PanelReto2.SetActive(false);
        }
    }

    public void EscenaFase3()
    {
        SceneManager.LoadScene("Fase 3");
    }

    public void ResetearReto2()
    {
        // 🔹 devolver objetos
        foreach (DragItem item in todosLosItems)
        {
            item.ResetItem();
        }

        // 🔹 limpiar lista
        zona.items.Clear();

        // 🔹 ocultar paneles
        panelCorrecto.SetActive(false);
        panelIncorrecto.SetActive(false);
        PanelReto2.SetActive(true);
    }
}