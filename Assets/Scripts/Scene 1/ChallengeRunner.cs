using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeRunner : MonoBehaviour
{
    enum State { Grace, Running, Success, Failed }

    const float GRACE_DURATION = 3f;
    const float POPUP_DURATION = 2.5f;

    State  state;
    float  timer;
    float  popupTimer;
    int    coinsAtStart;

    CoinPickup coinPickup;
    ChallengeData.Challenge challenge;

    // UI
    Canvas          uiCanvas;
    GameObject      gracePanel;
    TextMeshProUGUI graceText;
    GameObject      bannerPanel;
    TextMeshProUGUI bannerText;
    Image           bannerBar;
    GameObject      popupPanel;
    TextMeshProUGUI popupText;

    void Start()
    {
        challenge  = ChallengeData.GetActive();
        coinPickup = FindFirstObjectByType<CoinPickup>();
        BuildUI();
        EnterGrace();
    }

    void EnterGrace()
    {
        state = State.Grace;
        timer = GRACE_DURATION;
        gracePanel.SetActive(true);
        bannerPanel.SetActive(false);
        popupPanel.SetActive(false);
    }

    void EnterRunning()
    {
        state        = State.Running;
        timer        = challenge.timeLimit;
        coinsAtStart = coinPickup != null ? coinPickup.score : 0;
        gracePanel.SetActive(false);
        bannerPanel.SetActive(true);
    }

    void Update()
    {
        switch (state)
        {
            case State.Grace:
                timer -= Time.deltaTime;
                graceText.text =
                    "<b>" + challenge.name.ToUpper() + "</b>\n" +
                    challenge.description + "\n" +
                    "<size=80%>Starts in " + Mathf.CeilToInt(timer) + "...</size>";
                if (timer <= 0f) EnterRunning();
                break;

            case State.Running:
                timer -= Time.deltaTime;
                int collected = Mathf.Max(0, coinPickup != null ? coinPickup.score - coinsAtStart : 0);

                bannerText.text =
                    challenge.name + "  |  " +
                    collected + " / " + challenge.coinsRequired + " coins  |  " +
                    Mathf.Max(0f, timer).ToString("F1") + "s";

                bannerBar.fillAmount = Mathf.Clamp01(1f - timer / challenge.timeLimit);
                bannerBar.color      = timer < 3f
                    ? Color.Lerp(Color.red, new Color(1f, 0.55f, 0f), timer / 3f)
                    : new Color(1f, 0.55f, 0f);

                if (collected >= challenge.coinsRequired)
                    OnSuccess();
                else if (timer <= 0f)
                    OnFailed();
                break;

            case State.Success:
            case State.Failed:
                popupTimer -= Time.unscaledDeltaTime;
                if (popupTimer <= 0f)
                {
                    popupPanel.SetActive(false);
                    enabled = false;
                }
                break;
        }
    }

    void OnSuccess()
    {
        state      = State.Success;
        popupTimer = POPUP_DURATION;

        int reward = challenge.rewardCoins;
        PlayerPrefs.SetInt("TotalCoins", PlayerPrefs.GetInt("TotalCoins", 0) + reward);
        PlayerPrefs.Save();
        ChallengeData.AdvanceChallenge();

        bannerPanel.SetActive(false);
        gracePanel.SetActive(false);
        popupPanel.SetActive(true);
        popupText.text  = "<b>CHALLENGE DONE!</b>\n+" + reward + " coins";
        popupText.color = new Color(0.2f, 0.95f, 0.35f);
    }

    void OnFailed()
    {
        state      = State.Failed;
        popupTimer = POPUP_DURATION;
        ChallengeData.AdvanceChallenge();

        bannerPanel.SetActive(false);
        gracePanel.SetActive(false);
        popupPanel.SetActive(true);
        popupText.text  = "<b>CHALLENGE FAILED!</b>";
        popupText.color = new Color(1f, 0.15f, 0.15f);

        PlayerDeath death = FindFirstObjectByType<PlayerDeath>();
        if (death != null) death.Die();
    }

    void BuildUI()
    {
        GameObject cObj = new GameObject("ChallengeCanvas");
        uiCanvas = cObj.AddComponent<Canvas>();
        uiCanvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        uiCanvas.sortingOrder = 50;
        cObj.AddComponent<CanvasScaler>();
        cObj.AddComponent<GraphicRaycaster>();

        // ── Grace panel (centre screen) ──────────────────────────────────
        gracePanel = new GameObject("GracePanel");
        gracePanel.transform.SetParent(uiCanvas.transform, false);
        Image graceBg  = gracePanel.AddComponent<Image>();
        graceBg.color  = new Color(0f, 0f, 0f, 0.75f);
        RectTransform gpr = gracePanel.GetComponent<RectTransform>();
        gpr.anchorMin        = new Vector2(0.5f, 0.5f);
        gpr.anchorMax        = new Vector2(0.5f, 0.5f);
        gpr.pivot            = new Vector2(0.5f, 0.5f);
        gpr.sizeDelta        = new Vector2(440f, 130f);
        gpr.anchoredPosition = Vector2.zero;

        GameObject graceTextObj = new GameObject("GraceText");
        graceTextObj.transform.SetParent(gracePanel.transform, false);
        graceText           = graceTextObj.AddComponent<TextMeshProUGUI>();
        graceText.fontSize  = 18;
        graceText.color     = Color.white;
        graceText.alignment = TextAlignmentOptions.Center;
        Stretch(graceTextObj.GetComponent<RectTransform>(), 12f);

        // ── Banner panel (top centre, shown while running) ───────────────
        bannerPanel = new GameObject("BannerPanel");
        bannerPanel.transform.SetParent(uiCanvas.transform, false);
        Image bannerBg  = bannerPanel.AddComponent<Image>();
        bannerBg.color  = new Color(0.08f, 0.08f, 0.08f, 0.92f);
        RectTransform bpr = bannerPanel.GetComponent<RectTransform>();
        bpr.anchorMin        = new Vector2(0.5f, 1f);
        bpr.anchorMax        = new Vector2(0.5f, 1f);
        bpr.pivot            = new Vector2(0.5f, 1f);
        bpr.sizeDelta        = new Vector2(480f, 52f);
        bpr.anchoredPosition = new Vector2(0f, -8f);

        GameObject bannerTextObj = new GameObject("BannerText");
        bannerTextObj.transform.SetParent(bannerPanel.transform, false);
        bannerText           = bannerTextObj.AddComponent<TextMeshProUGUI>();
        bannerText.fontSize  = 15;
        bannerText.color     = Color.white;
        bannerText.alignment = TextAlignmentOptions.Top;
        RectTransform btr    = bannerTextObj.GetComponent<RectTransform>();
        btr.anchorMin        = new Vector2(0f, 1f);
        btr.anchorMax        = new Vector2(1f, 1f);
        btr.pivot            = new Vector2(0.5f, 1f);
        btr.sizeDelta        = new Vector2(-16f, 30f);
        btr.anchoredPosition = new Vector2(0f, -4f);

        // Progress bar background
        GameObject barBgObj = new GameObject("BarBg");
        barBgObj.transform.SetParent(bannerPanel.transform, false);
        Image barBgImg  = barBgObj.AddComponent<Image>();
        barBgImg.color  = new Color(0.25f, 0.25f, 0.25f);
        RectTransform bbr = barBgObj.GetComponent<RectTransform>();
        bbr.anchorMin        = new Vector2(0f, 0f);
        bbr.anchorMax        = new Vector2(1f, 0f);
        bbr.pivot            = new Vector2(0.5f, 0f);
        bbr.sizeDelta        = new Vector2(-16f, 10f);
        bbr.anchoredPosition = new Vector2(0f, 6f);

        // Progress bar fill
        GameObject barFillObj = new GameObject("BarFill");
        barFillObj.transform.SetParent(barBgObj.transform, false);
        bannerBar               = barFillObj.AddComponent<Image>();
        bannerBar.color         = new Color(1f, 0.55f, 0f);
        bannerBar.type          = Image.Type.Filled;
        bannerBar.fillMethod    = Image.FillMethod.Horizontal;
        bannerBar.fillOrigin    = (int)Image.OriginHorizontal.Left;
        bannerBar.fillAmount    = 0f;
        RectTransform bfr       = barFillObj.GetComponent<RectTransform>();
        bfr.anchorMin           = Vector2.zero;
        bfr.anchorMax           = Vector2.one;
        bfr.sizeDelta           = Vector2.zero;
        bfr.anchoredPosition    = Vector2.zero;

        // ── Popup panel (centre screen, result) ──────────────────────────
        popupPanel = new GameObject("PopupPanel");
        popupPanel.transform.SetParent(uiCanvas.transform, false);
        Image popupBg  = popupPanel.AddComponent<Image>();
        popupBg.color  = new Color(0f, 0f, 0f, 0.8f);
        RectTransform ppr = popupPanel.GetComponent<RectTransform>();
        ppr.anchorMin        = new Vector2(0.5f, 0.5f);
        ppr.anchorMax        = new Vector2(0.5f, 0.5f);
        ppr.pivot            = new Vector2(0.5f, 0.5f);
        ppr.sizeDelta        = new Vector2(400f, 110f);
        ppr.anchoredPosition = Vector2.zero;

        GameObject popupTextObj = new GameObject("PopupText");
        popupTextObj.transform.SetParent(popupPanel.transform, false);
        popupText           = popupTextObj.AddComponent<TextMeshProUGUI>();
        popupText.fontSize  = 26;
        popupText.alignment = TextAlignmentOptions.Center;
        Stretch(popupTextObj.GetComponent<RectTransform>(), 8f);

        gracePanel.SetActive(false);
        bannerPanel.SetActive(false);
        popupPanel.SetActive(false);
    }

    void Stretch(RectTransform rt, float pad = 0f)
    {
        rt.anchorMin        = Vector2.zero;
        rt.anchorMax        = Vector2.one;
        rt.offsetMin        = new Vector2(pad, pad);
        rt.offsetMax        = new Vector2(-pad, -pad);
        rt.anchoredPosition = Vector2.zero;
    }
}
