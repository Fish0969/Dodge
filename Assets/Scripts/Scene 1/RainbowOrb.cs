using UnityEngine;

public class RainbowOrb : MonoBehaviour
{
    float          hue = 0f;
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        hue = (hue + Time.unscaledDeltaTime * 2.5f) % 1f;
        if (sr != null) sr.color = Color.HSVToRGB(hue, 1f, 1f);
    }
}
