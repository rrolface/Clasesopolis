using UnityEngine;
using UnityEngine.UI;

public class ComprobarRespuesta : MonoBehaviour
{
    public DropZone[] slots;
    public GameObject PanelCorrecto, PanelIncorrecto, PanelReto1;

    public DragItem[] todosLosItems;

    public void Comprobar()
    {
        bool todoCorrecto = true;

        foreach (DropZone slot in slots)
        {
            if (slot.itemActual == null)
            {
                Debug.Log("Falta un elemento ❗");
                todoCorrecto = false;
                continue;
            }

            if (slot.itemActual.nombre == slot.tipoCorrecto)
            {
                Debug.Log(slot.name + " Correcto ✅");
                slot.GetComponent<Image>().color = Color.green;
            }
            else
            {
                Debug.Log(slot.name + " Incorrecto ❌");
                slot.GetComponent<Image>().color = Color.red;
                todoCorrecto = false;
            }
        }

        if (todoCorrecto)
        {
            Debug.Log("TODO CORRECTO 🎉");
            PanelCorrecto.SetActive(true);
            PanelReto1.SetActive(false);
        }
        else
        {
            Debug.Log("HAY ERRORES ❌");
            PanelIncorrecto.SetActive(true);
            PanelReto1.SetActive(false);
        }
    }

    public void ResetearReto1()
    {
        foreach (DragItem item in todosLosItems)
        {
            item.ResetItem();
        }

        foreach (DropZone slot in slots)
        {
            slot.itemActual = null;
            slot.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }

        PanelCorrecto.SetActive(false);
        PanelIncorrecto.SetActive(false);
        PanelReto1.SetActive(true);
    }
}