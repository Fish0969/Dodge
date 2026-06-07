using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinPickup : MonoBehaviour
{
    public float pickupRadius = 0.3f;
    public int score = 0;

    public TextMeshProUGUI scoreText;

    // Slow orb
    public float slowDuration  = 2f;
    const float SLOW_TIMESCALE = 0.3f;

    Movement        movement;
    float           originalMoveSpeed;
    float           slowTimeLeft = 0f;
    Image           slowOverlay;
    TextMeshPro     slowTimerTmp;

    // Rainbow orb
    public static bool IsRainbow { get; private set; }
    public float rainbowDuration = 5f;

    float          rainbowTimeLeft = 0f;
    float          rainbowHue      = 0f;
    SpriteRenderer playerSr;
    Color          originalPlayerColor;
    TextMeshPro    rainbowCountdown;

    void Start()
    {
        movement          = GetComponent<Movement>();
        originalMoveSpeed = movement.moveSpeed;
        playerSr          = GetComponentInChildren<SpriteRenderer>();
        if (playerSr != null) originalPlayerColor = playerSr.color;
        BuildRainbowCountdown();
        BuildSlowCountdown();
        BuildSlowUI();
    }

    void Update()
    {
        scoreText.text = "Score: " + score;

        // Coin pickup — skip special orbs
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject coin in coins)
        {
            if (coin.GetComponent<BlueOrb>()    != null) continue;
            if (coin.GetComponent<RainbowOrb>() != null) continue;
            if (Vector2.Distance(transform.position, coin.transform.position) < pickupRadius)
            {
                score++;
                PlayerPrefs.SetInt("TotalCoins", PlayerPrefs.GetInt("TotalCoins", 0) + 1);
                PlayerPrefs.Save();
                Destroy(coin);
            }
        }

        // Blue orb pickup
        BlueOrb[] blueOrbs = FindObjectsByType<BlueOrb>(FindObjectsSortMode.None);
        foreach (BlueOrb orb in blueOrbs)
        {
            if (Vector2.Distance(transform.position, orb.transform.position) < pickupRadius)
            {
                slowTimeLeft          = slowDuration;
                Time.timeScale        = SLOW_TIMESCALE;
                movement.moveSpeed    = originalMoveSpeed / SLOW_TIMESCALE;
                slowOverlay.gameObject.SetActive(true);
                slowTimerTmp.gameObject.SetActive(true);
                Destroy(orb.gameObject);
            }
        }

        // Rainbow orb pickup
        RainbowOrb[] rainbowOrbs = FindObjectsByType<RainbowOrb>(FindObjectsSortMode.None);
        foreach (RainbowOrb orb in rainbowOrbs)
        {
            if (Vector2.Distance(transform.position, orb.transform.position) < pickupRadius)
            {
                rainbowTimeLeft = rainbowDuration;
                IsRainbow       = true;
                rainbowCountdown.gameObject.SetActive(true);
                Destroy(orb.gameObject);
            }
        }

        // Slow countdown
        if (slowTimeLeft > 0f)
        {
            slowTimeLeft -= Time.unscaledDeltaTime;
            if (slowTimeLeft <= 0f)
            {
                slowTimeLeft       = 0f;
                Time.timeScale     = 1f;
                movement.moveSpeed = originalMoveSpeed;
                slowOverlay.gameObject.SetActive(false);
                slowTimerTmp.gameObject.SetActive(false);
            }
            else
            {
                slowTimerTmp.text = slowTimeLeft.ToString("F1");
            }
        }

        // Rainbow countdown + player colour cycling
        if (rainbowTimeLeft > 0f)
        {
            rainbowTimeLeft -= Time.unscaledDeltaTime;
            rainbowHue       = (rainbowHue + Time.unscaledDeltaTime * 3f) % 1f;
            if (playerSr != null)
                playerSr.color = Color.HSVToRGB(rainbowHue, 1f, 1f);

            if (rainbowTimeLeft <= 0f)
            {
                rainbowTimeLeft = 0f;
                IsRainbow       = false;
                if (playerSr != null) playerSr.color = originalPlayerColor;
                rainbowCountdown.gameObject.SetActive(false);
            }
            else
            {
                rainbowCountdown.text  = rainbowTimeLeft.ToString("F1");
                rainbowCountdown.color = Color.HSVToRGB((rainbowHue + 0.5f) % 1f, 1f, 1f);
            }
        }
    }

    public void ResetSlow()
    {
        slowTimeLeft   = 0f;
        Time.timeScale = 1f;
        if (slowOverlay  != null) slowOverlay.gameObject.SetActive(false);
        if (slowTimerTmp != null) slowTimerTmp.gameObject.SetActive(false);
    }

    public void ResetRainbow()
    {
        rainbowTimeLeft = 0f;
        IsRainbow       = false;
        if (playerSr != null) playerSr.color = originalPlayerColor;
        if (rainbowCountdown != null) rainbowCountdown.gameObject.SetActive(false);
    }

    void BuildRainbowCountdown()
    {
        GameObject obj = new GameObject("RainbowCountdown");
        obj.transform.SetParent(transform, false);
        obj.transform.localPosition = new Vector3(0f, 0f, -0.5f);

        rainbowCountdown            = obj.AddComponent<TextMeshPro>();
        rainbowCountdown.fontSize   = 2.8f;
        rainbowCountdown.fontStyle  = FontStyles.Bold;
        rainbowCountdown.alignment  = TextAlignmentOptions.Center;
        rainbowCountdown.color      = Color.white;

        // Render on top of the player sprite
        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = 10;

        obj.SetActive(false);
    }

    void BuildSlowCountdown()
    {
        GameObject obj = new GameObject("SlowCountdown");
        obj.transform.SetParent(transform, false);
        obj.transform.localPosition = new Vector3(0f, 0.35f, -0.5f);

        slowTimerTmp           = obj.AddComponent<TextMeshPro>();
        slowTimerTmp.fontSize  = 2.8f;
        slowTimerTmp.fontStyle = FontStyles.Bold;
        slowTimerTmp.alignment = TextAlignmentOptions.Center;
        slowTimerTmp.color     = Color.cyan;

        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = 10;

        obj.SetActive(false);
    }

    void BuildSlowUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject cObj = new GameObject("SlowCanvas");
            canvas = cObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            cObj.AddComponent<CanvasScaler>();
            cObj.AddComponent<GraphicRaycaster>();
        }

        // Full-screen blue tint overlay
        GameObject overlayObj = new GameObject("SlowOverlay");
        overlayObj.transform.SetParent(canvas.transform, false);
        slowOverlay       = overlayObj.AddComponent<Image>();
        slowOverlay.color = new Color(0f, 0.3f, 1f, 0.18f);
        RectTransform ort = overlayObj.GetComponent<RectTransform>();
        ort.anchorMin = Vector2.zero;
        ort.anchorMax = Vector2.one;
        ort.offsetMin = Vector2.zero;
        ort.offsetMax = Vector2.zero;
        overlayObj.SetActive(false);
    }
}
