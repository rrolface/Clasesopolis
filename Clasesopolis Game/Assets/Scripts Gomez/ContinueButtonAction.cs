using UnityEngine;

public class ContinueButtonAction : MonoBehaviour
{
    public GameObject targetToHide;
    public GameObject targetToShow;

    public void OnContinue()
    {
        if (targetToHide != null)
            targetToHide.SetActive(false);

        if (targetToShow != null)
            targetToShow.SetActive(true);
    }
}