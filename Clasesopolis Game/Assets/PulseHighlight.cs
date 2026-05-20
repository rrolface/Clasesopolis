using UnityEngine;
using UnityEngine.UI;

public class PulseHighlight : MonoBehaviour
{
    [SerializeField] private RawImage highlightImage;
    [SerializeField] private float pulseSpeed = 1.2f;
    [SerializeField] private float minAlpha = 0f;
    [SerializeField] private float maxAlpha = 0.6f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime * pulseSpeed;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(timer) + 1f) / 2f);
        Color c = highlightImage.color;
        c.a = alpha;
        highlightImage.color = c;
    }
}