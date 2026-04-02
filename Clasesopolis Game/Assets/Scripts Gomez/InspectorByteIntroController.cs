using System.Collections;
using UnityEngine;

public class InspectorByteIntroController : MonoBehaviour
{
    public Animator animator;
    public float introDuration = 1f;

    void Start()
    {
        StartCoroutine(DisableAnimatorAfterIntro());
    }

    IEnumerator DisableAnimatorAfterIntro()
    {
        yield return new WaitForSeconds(introDuration);

        if (animator != null)
        {
            animator.enabled = false;
        }
    }
}