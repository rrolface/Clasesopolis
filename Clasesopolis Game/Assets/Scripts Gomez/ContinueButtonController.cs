using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ContinueButtonController : MonoBehaviour
{
    public Button button;
    public float delay = 3f;

    void Start()
    {
        if (button != null)
        {
            button.interactable = false;
            StartCoroutine(EnableButtonAfterDelay());
        }
    }

    IEnumerator EnableButtonAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        button.interactable = true;
    }
}