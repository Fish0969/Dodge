using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    [Header("Settings")]
    public float holdDuration   = 1.5f;
    public float quitWindowTime = 2f;

    float           holdProgress   = 0f;
    bool            escapePrimed   = false;
    float           escapeTimer    = 0f;
    float           resetProgress  = 0f;
    Image           circleFill;
    TextMeshProUGUI coinText;
    TextMeshProUGUI quitText;
    TextMeshProUGUI resetText;

    void Start()
    {
        BuildUI();
    }

    void Update()
    {
        coinText.text = "Coins: " + PlayerPrefs.GetInt("TotalCoins", 0);

        // Hold Space to play
        if (Input.GetKey(KeyCode.Space))
        {
            holdProgress           += Time.deltaTime;
            circleFill.fillAmount   = holdProgress / holdDuration;
            if (holdProgress >= holdDuration)
                SceneManager.LoadScene("Gameplay");
        }
        else
        {
            holdProgress          = 0f;
            circleFill.fillAmount = 0f;
        }

        // Double Escape to quit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escapePrimed)
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                escapePrimed  = true;
                escapeTimer   = quitWindowTime;
                quitText.text = "Press Esc again to quit";
            }
        }

        if (escapePrimed)
        {
            escapeTimer -= Time.deltaTime;
            if (escapeTimer <= 0f)
            {
                escapePrimed  = false;
                quitText.text = "";
            }
        }

        // Hold Delete to reset coins
        if (Input.GetKey(KeyCode.Delete))
        {
            resetProgress += Time.deltaTime;
            resetText.text = "Hold to reset coins... " + Mathf.CeilToInt(2f - resetProgress) + "s";
            if (resetProgress >= 2f)
            {
                PlayerPrefs.SetInt("TotalCoins", 0);
                PlayerPrefs.SetInt("BestScore",  0);
                PlayerPrefs.Save();
                resetProgress  = 0f;
                resetText.text = "Coins reset!";
            }
        }
        else
        {
            resetProgress = 0f;
            if (resetText.text != "Coins reset!") resetText.text = "";
        }
    }

    void BuildUI()
    {
        GameObject cObj = new GameObject("StartCanvas");
        Canvas canvas = cObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        cObj.AddComponent<CanvasScaler>();
        cObj.AddComponent<GraphicRaycaster>();

        // Coin counter — top right, white
        GameObject coinObj = new GameObject("CoinText");
        coinObj.transform.SetParent(canvas.transform, false);
        coinText           = coinObj.AddComponent<TextMeshProUGUI>();
        coinText.text      = "Coins: 0";
        coinText.fontSize  = 22;
        coinText.color     = Color.white;
        coinText.alignment = TextAlignmentOptions.TopRight;
        RectTransform crt  = coinObj.GetComponent<RectTransform>();
        crt.anchorMin        = Vector2.one;
        crt.anchorMax        = Vector2.one;
        crt.pivot            = Vector2.one;
        crt.sizeDelta        = new Vector2(200f, 50f);
        crt.anchoredPosition = new Vector2(-16f, -16f);

        // "Hold Space to Play" label — above the ring
        GameObject hintObj = new GameObject("HintText");
        hintObj.transform.SetParent(canvas.transform, false);
        TextMeshProUGUI hint = hintObj.AddComponent<TextMeshProUGUI>();
        hint.text      = "Hold Space to Play";
        hint.fontSize  = 18;
        hint.color     = new Color(1f, 1f, 1f, 0.8f);
        hint.alignment = TextAlignmentOptions.Center;
        RectTransform hrt = hintObj.GetComponent<RectTransform>();
        hrt.anchorMin        = new Vector2(0.5f, 0f);
        hrt.anchorMax        = new Vector2(0.5f, 0f);
        hrt.pivot            = new Vector2(0.5f, 0f);
        hrt.sizeDelta        = new Vector2(240f, 30f);
        hrt.anchoredPosition = new Vector2(0f, 155f);

        // Challenges hint — bottom left above quit
        GameObject chalHintObj = new GameObject("ChallengesHint");
        chalHintObj.transform.SetParent(canvas.transform, false);
        TextMeshProUGUI chalHint = chalHintObj.AddComponent<TextMeshProUGUI>();
        chalHint.text      = "E: Achievements";
        chalHint.fontSize  = 16;
        chalHint.color     = new Color(1f, 1f, 1f, 0.6f);
        chalHint.alignment = TextAlignmentOptions.BottomLeft;
        RectTransform chrt = chalHintObj.GetComponent<RectTransform>();
        chrt.anchorMin        = Vector2.zero;
        chrt.anchorMax        = Vector2.zero;
        chrt.pivot            = Vector2.zero;
        chrt.sizeDelta        = new Vector2(200f, 30f);
        chrt.anchoredPosition = new Vector2(16f, 56f);

        // Quit hint — bottom left
        GameObject quitObj = new GameObject("QuitText");
        quitObj.transform.SetParent(canvas.transform, false);
        quitText           = quitObj.AddComponent<TextMeshProUGUI>();
        quitText.text      = "";
        quitText.fontSize  = 16;
        quitText.color     = new Color(1f, 0.3f, 0.3f, 1f);
        quitText.alignment = TextAlignmentOptions.BottomLeft;
        RectTransform qrt  = quitObj.GetComponent<RectTransform>();
        qrt.anchorMin        = Vector2.zero;
        qrt.anchorMax        = Vector2.zero;
        qrt.pivot            = Vector2.zero;
        qrt.sizeDelta        = new Vector2(280f, 40f);
        qrt.anchoredPosition = new Vector2(16f, 16f);

        // Reset data hint — bottom left, above challenges hint
        GameObject resetObj = new GameObject("ResetText");
        resetObj.transform.SetParent(canvas.transform, false);
        resetText           = resetObj.AddComponent<TextMeshProUGUI>();
        resetText.text      = "";
        resetText.fontSize  = 14;
        resetText.color     = new Color(1f, 0.4f, 0.4f, 0.8f);
        resetText.alignment = TextAlignmentOptions.BottomLeft;
        RectTransform rrt   = resetObj.GetComponent<RectTransform>();
        rrt.anchorMin        = Vector2.zero;
        rrt.anchorMax        = Vector2.zero;
        rrt.pivot            = Vector2.zero;
        rrt.sizeDelta        = new Vector2(280f, 30f);
        rrt.anchoredPosition = new Vector2(16f, 86f);

        // Grey ring background
        GameObject bgCircle = new GameObject("CircleBg");
        bgCircle.transform.SetParent(canvas.transform, false);
        Image bgImg  = bgCircle.AddComponent<Image>();
        bgImg.sprite = MakeRingSprite(256, 0.6f);
        bgImg.color  = new Color(0.3f, 0.3f, 0.3f, 1f);
        RectTransform bgRt = bgCircle.GetComponent<RectTransform>();
        PlaceRing(bgRt);

        // Orange fill ring
        GameObject fillCircle = new GameObject("CircleFill");
        fillCircle.transform.SetParent(canvas.transform, false);
        circleFill               = fillCircle.AddComponent<Image>();
        circleFill.sprite        = MakeRingSprite(256, 0.6f);
        circleFill.color         = new Color(1f, 0.55f, 0f, 1f);
        circleFill.type          = Image.Type.Filled;
        circleFill.fillMethod    = Image.FillMethod.Radial360;
        circleFill.fillOrigin    = (int)Image.Origin360.Top;
        circleFill.fillClockwise = true;
        circleFill.fillAmount    = 0f;
        RectTransform fillRt = fillCircle.GetComponent<RectTransform>();
        PlaceRing(fillRt);
    }

    void PlaceRing(RectTransform rt)
    {
        rt.anchorMin        = new Vector2(0.5f, 0f);
        rt.anchorMax        = new Vector2(0.5f, 0f);
        rt.pivot            = new Vector2(0.5f, 0f);
        rt.sizeDelta        = new Vector2(120f, 120f);
        rt.anchoredPosition = new Vector2(0f, 20f);
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
}
