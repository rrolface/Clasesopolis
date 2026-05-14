using UnityEngine;

public class OnEnableDetector : MonoBehaviour
{
    [SerializeField] private Animator byteAnimator;
    [SerializeField] private string triggerName = "MoveLeft";
    [SerializeField] private Animator cameraAnimator;
    [SerializeField] private string cameraTrigger = "MoveFases";

    void OnEnable()
    {
        byteAnimator.SetTrigger(triggerName);
        cameraAnimator.SetTrigger(cameraTrigger);
    }
}