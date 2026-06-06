using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [Header("Settings")]
    public float holdDuration = 2f;
    public float overlayAlpha = 0.5f;

    bool  paused       = false;
    float holdProgress = 0f;

    Image      overlay;
    GameObject barPanel;
    Image      circleFill;

    void Start()
    {
        BuildUI();
        SetPaused(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !paused)
            SetPaused(true);

        if (!paused) return;

        if (Input.GetKey(KeyCode.Space))
        {
            holdProgress += Time.unscaledDeltaTime;
            circleFill.fillAmount = holdProgress / holdDuration;
            if (holdProgress >= holdDuration)
                SetPaused(false);
        }
        else
        {
            holdProgress          = 0f;
            circleFill.fillAmount = 0f;
        }
    }

    void SetPaused(bool p)
    {
        paused                = p;
        Time.timeScale        = p ? 0f : 1f;
        holdProgress          = 0f;
        circleFill.fillAmount = 0f;
        overlay.gameObject.SetActive(p);
        barPanel.SetActive(p);
    }

    void BuildUI()
    {
        GameObject cObj = new GameObject("PauseCanvas");
        Canvas canvas = cObj.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        cObj.AddComponent<CanvasScaler>();
        cObj.AddComponent<GraphicRaycaster>();

        // Dark overlay
        GameObject overlayObj = new GameObject("Overlay");
        overlayObj.transform.SetParent(canvas.transform, false);
        overlay       = overlayObj.AddComponent<Image>();
        overlay.color = new Color(0f, 0f, 0f, overlayAlpha);
        Stretch(overlayObj.GetComponent<RectTransform>());

        // Bottom panel
        GameObject panel = new GameObject("PauseBar");
        panel.transform.SetParent(canvas.transform, false);
        panel.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        RectTransform pr = panel.GetComponent<RectTransform>();
        pr.anchorMin        = new Vector2(0f, 0f);
        pr.anchorMax        = new Vector2(1f, 0f);
        pr.pivot            = new Vector2(0.5f, 0f);
        pr.sizeDelta        = new Vector2(0f, 80f);
        pr.anchoredPosition = Vector2.zero;
        barPanel = panel;

        // Grey ring (background)
        GameObject bgCircle = new GameObject("CircleBg");
        bgCircle.transform.SetParent(panel.transform, false);
        Image bgImg   = bgCircle.AddComponent<Image>();
        bgImg.sprite  = MakeRingSprite(128, 0.55f);
        bgImg.color   = new Color(0.35f, 0.35f, 0.35f, 1f);
        PlaceCircle(bgCircle.GetComponent<RectTransform>());

        // Orange fill ring (Radial360)
        GameObject fillCircle = new GameObject("CircleFill");
        fillCircle.transform.SetParent(panel.transform, false);
        circleFill               = fillCircle.AddComponent<Image>();
        circleFill.sprite        = MakeRingSprite(128, 0.55f);
        circleFill.color         = new Color(1f, 0.55f, 0f, 1f);
        circleFill.type          = Image.Type.Filled;
        circleFill.fillMethod    = Image.FillMethod.Radial360;
        circleFill.fillOrigin    = (int)Image.Origin360.Top;
        circleFill.fillClockwise = true;
        circleFill.fillAmount    = 0f;
        PlaceCircle(fillCircle.GetComponent<RectTransform>());

        // Text
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(panel.transform, false);
        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text      = "Hold Space to continue";
        label.alignment = TextAlignmentOptions.MidlineLeft;
        label.fontSize  = 16;
        label.color     = Color.white;
        RectTransform lr = labelObj.GetComponent<RectTransform>();
        lr.anchorMin = Vector2.zero;
        lr.anchorMax = Vector2.one;
        lr.offsetMin = new Vector2(90f, 0f); // leave room for the circle
        lr.offsetMax = Vector2.zero;
    }

    void PlaceCircle(RectTransform rt)
    {
        rt.anchorMin        = new Vector2(0f, 0.5f);
        rt.anchorMax        = new Vector2(0f, 0.5f);
        rt.pivot            = new Vector2(0f, 0.5f);
        rt.sizeDelta        = new Vector2(64f, 64f);
        rt.anchoredPosition = new Vector2(8f, 0f);
    }

    Sprite MakeRingSprite(int size, float innerFraction)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float center = size / 2f;
        float outer  = center;
        float inner  = outer * innerFraction;
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(center, center));
                tex.SetPixel(x, y, (dist >= inner && dist <= outer) ? Color.white : Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    void Stretch(RectTransform rt)
    {
        rt.anchorMin        = Vector2.zero;
        rt.anchorMax        = Vector2.one;
        rt.sizeDelta        = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }
}
