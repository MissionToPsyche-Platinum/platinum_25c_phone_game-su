using UnityEngine;

public class AlphaFlashing : MonoBehaviour
{
    [Range(0f, 1f)] public float alphaMin = 0f;
    [Range(0f, 1f)] public float alphaMax = 1f;
    public float fadeDuration = 1f;
    [Range(0.1f, 5f)] public float dimBias = 3f;

    private float time = 0f;
    private bool isFadingIn = true;

    void Update()
    {
        time += (isFadingIn ? 1f : -1f) * (Time.deltaTime / fadeDuration);

        if (time >= 1f)
        {
            time = 1f;
            isFadingIn = false;
        }
        else if (time <= 0f)
        {
            time = 0f;
            isFadingIn = true;
        }

        float curvedTime = Mathf.Pow(time, dimBias);
        float alpha = Mathf.Lerp(alphaMin, alphaMax, curvedTime);
        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            Color color = sprite.color;
            color.a = alpha;
            sprite.color = color;
        }
    }
}