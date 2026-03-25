using UnityEngine;

public class HelpPanelToggle : MonoBehaviour
{
    public GameObject helpPanel;

    public void ToggleHelpPanel()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(!helpPanel.activeSelf);
        }
    }
}