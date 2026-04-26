using UnityEngine;

public class SelectedObjectColorAnimator : MonoBehaviour
{
    [Header("Referencias")]
    public ObjectMaterialSelector selector;

    [Header("Animacion de color")]
    public Color startColor = Color.white;
    public Color endColor = Color.red;
    public float lerpDuration = 1.5f;
    public KeyCode toggleKey = KeyCode.Space;

    private float lerpTime = 0f;
    private bool colorAnimationStarted = false;

    public bool IsAnimationRunning => colorAnimationStarted;

    void Start()
    {
        if (selector == null)
        {
            selector = FindObjectOfType<ObjectMaterialSelector>();
        }
    }

    void Update()
    {
        if (selector == null) return;

        if (Input.GetKeyDown(toggleKey))
        {
            colorAnimationStarted = !colorAnimationStarted;
        }

        if (colorAnimationStarted)
        {
            AnimateSelectedObjectColor();
        }
    }

    private void AnimateSelectedObjectColor()
    {
        Renderer rend = selector.SelectedRenderer;
        if (rend == null) return;
        if (lerpDuration <= 0f) return;

        lerpTime += Time.deltaTime / lerpDuration;
        rend.material.color = Color.Lerp(startColor, endColor, lerpTime);

        if (lerpTime >= 1f)
        {
            lerpTime = 0f;

            // Swap colors for ping-pong effect
            (startColor, endColor) = (endColor, startColor);
        }
    }
}
