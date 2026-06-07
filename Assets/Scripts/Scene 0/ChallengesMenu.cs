using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChallengesMenu : MonoBehaviour
{
    struct Achievement
    {
        public string title, description, key;
        public int target;
        public Achievement(string t, string d, string k, int tgt)
        { title = t; description = d; key = k; target = tgt; }
    }

    static readonly Achievement[] Achievements =
    {
        // Single-run score
        new Achievement("First Steps",    "Get 5 coins in one run",        "BestScore",    5),
        new Achievement("Coin Seeker",    "Get 20 coins in one run",       "BestScore",   20),
        new Achievement("Coin Addict",    "Get 50 coins in one run",       "BestScore",   50),
        new Achievement("Coin Maniac",    "Get 100 coins in one run",      "BestScore",  100),
        new Achievement("Unstoppable",    "Get 150 coins in one run",      "BestScore",  150),
        new Achievement("Legend",         "Get 200 coins in one run",      "BestScore",  200),
        new Achievement("God Mode",       "Get 350 coins in one run",      "BestScore",  350),
        new Achievement("Transcendent",   "Get 500 coins in one run",      "BestScore",  500),
        new Achievement("Immortal",       "Get 750 coins in one run",      "BestScore",  750),
        new Achievement("The Absolute",   "Get 1000 coins in one run",     "BestScore", 1000),
        // Total coins
        new Achievement("Pocket Full",    "Collect 100 coins total",       "TotalCoins",   100),
        new Achievement("Coin Hoarder",   "Collect 500 coins total",       "TotalCoins",   500),
        new Achievement("Rich",           "Collect 1000 coins total",      "TotalCoins",  1000),
        new Achievement("Coin Legend",    "Collect 5000 coins total",      "TotalCoins",  5000),
        new Achievement("Millionaire",    "Collect 10,000 coins total",    "TotalCoins", 10000),
        new Achievement("Coin Emperor",   "Collect 25,000 coins total",    "TotalCoins", 25000),
        new Achievement("Coin God",       "Collect 50,000 coins total",    "TotalCoins", 50000),
        new Achievement("Beyond Reason",  "Collect 100,000 coins total",   "TotalCoins",100000),
    };

    const float ROW_H = 54f;

    GameObject        menuRoot;
    TextMeshProUGUI[] statusTexts;
    Image[]           fillBars;
    bool              visible    = false;
    bool              wasVisible = false;

    void Start() => BuildUI();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            visible = !visible;
            menuRoot.SetActive(visible);
        }

        if (visible && !wasVisible) CheckCompleted();
        wasVisible = visible;
        if (!visible) return;

        int cycle = PlayerPrefs.GetInt("ChallengeCycle", 0);
        for (int i = 0; i < Achievements.Length; i++)
        {
            int  val  = PlayerPrefs.GetInt(Achievements[i].key, 0);
            int  tgt  = Achievements[i].target;
            bool done = PlayerPrefs.GetInt("ChalClaim_" + cycle + "_" + i, 0) == 1;

            fillBars[i].fillAmount = done ? 1f : Mathf.Clamp01((float)val / tgt);

            if (done)
            {
                statusTexts[i].text  = "Completed";
                statusTexts[i].color = new Color(0.35f, 1f, 0.5f);
                fillBars[i].color    = new Color(0.25f, 0.8f, 0.4f);
            }
            else
            {
                statusTexts[i].text  = val + " / " + tgt;
                statusTexts[i].color = new Color(0.65f, 0.65f, 0.65f);
                fillBars[i].color    = new Color(1f, 0.55f, 0.1f);
            }
        }
    }

    void CheckCompleted()
    {
        int  cycle  = PlayerPrefs.GetInt("ChallengeCycle", 0);
        bool anyNew = false;

        for (int i = 0; i < Achievements.Length; i++)
        {
            if (PlayerPrefs.GetInt("ChalClaim_" + cycle + "_" + i, 0) == 1) continue;
            if (PlayerPrefs.GetInt(Achievements[i].key, 0) >= Achievements[i].target)
            {
                PlayerPrefs.SetInt("ChalClaim_" + cycle + "_" + i, 1);
                anyNew = true;
            }
        }

        if (!anyNew) return;

        bool allDone = true;
        for (int i = 0; i < Achievements.Length; i++)
            if (PlayerPrefs.GetInt("ChalClaim_" + cycle + "_" + i, 0) == 0)
            { allDone = false; break; }

        if (allDone) PlayerPrefs.SetInt("ChallengeCycle", cycle + 1);
        PlayerPrefs.Save();
    }

    void BuildUI()
    {
        GameObject cObj = new GameObject("AchievementsCanvas");
        Canvas canvas   = cObj.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        cObj.AddComponent<CanvasScaler>();
        cObj.AddComponent<GraphicRaycaster>();

        menuRoot = new GameObject("MenuRoot");
        menuRoot.transform.SetParent(canvas.transform, false);
        Stretch(menuRoot.AddComponent<RectTransform>());

        // Dim
        GameObject dim = new GameObject("Dim");
        dim.transform.SetParent(menuRoot.transform, false);
        dim.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.65f);
        Stretch(dim.GetComponent<RectTransform>());

        // Panel
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(menuRoot.transform, false);
        panel.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.08f, 1f);
        RectTransform pr = panel.GetComponent<RectTransform>();
        pr.anchorMin        = new Vector2(0.5f, 0.5f);
        pr.anchorMax        = new Vector2(0.5f, 0.5f);
        pr.pivot            = new Vector2(0.5f, 0.5f);
        pr.sizeDelta        = new Vector2(460f, 500f);
        pr.anchoredPosition = Vector2.zero;

        // Title
        MakeLabel(panel.transform, "ACHIEVEMENTS", 22, Color.white, FontStyles.Bold,
            new Vector2(0f,1f), new Vector2(1f,1f), new Vector2(0.5f,1f),
            new Vector2(0f, 44f), new Vector2(0f, -12f));

        // Divider
        GameObject div = new GameObject("Divider");
        div.transform.SetParent(panel.transform, false);
        div.AddComponent<Image>().color = new Color(1f,1f,1f,0.08f);
        RectTransform dr = div.GetComponent<RectTransform>();
        dr.anchorMin = new Vector2(0f,1f); dr.anchorMax = new Vector2(1f,1f);
        dr.pivot     = new Vector2(0.5f,1f);
        dr.sizeDelta = new Vector2(-32f, 1f);
        dr.anchoredPosition = new Vector2(0f, -56f);

        // ── Scroll area ──────────────────────────────────────────────────────
        GameObject scrollObj = new GameObject("ScrollArea");
        scrollObj.transform.SetParent(panel.transform, false);
        ScrollRect scroll        = scrollObj.AddComponent<ScrollRect>();
        scroll.horizontal        = false;
        scroll.vertical          = true;
        scroll.scrollSensitivity = 50f;
        scroll.movementType      = ScrollRect.MovementType.Clamped;
        RectTransform scrollRt   = scrollObj.GetComponent<RectTransform>();
        scrollRt.anchorMin       = new Vector2(0f, 0f);
        scrollRt.anchorMax       = new Vector2(1f, 1f);
        scrollRt.offsetMin       = new Vector2(0f, 36f);
        scrollRt.offsetMax       = new Vector2(0f, -58f);

        // Viewport (masked)
        GameObject vp          = new GameObject("Viewport");
        vp.transform.SetParent(scrollObj.transform, false);
        vp.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.002f);
        vp.AddComponent<Mask>().showMaskGraphic = false;
        RectTransform vpRt     = vp.GetComponent<RectTransform>();
        vpRt.anchorMin         = Vector2.zero;
        vpRt.anchorMax         = Vector2.one;
        vpRt.sizeDelta         = Vector2.zero;
        vpRt.anchoredPosition  = Vector2.zero;
        scroll.viewport        = vpRt;

        // Content (scrollable container) — Image forces RectTransform before SetParent
        float contentH         = Achievements.Length * ROW_H;
        GameObject content     = new GameObject("Content");
        RectTransform ctRt     = content.AddComponent<RectTransform>();
        content.transform.SetParent(vp.transform, false);
        ctRt.anchorMin         = new Vector2(0f, 1f);
        ctRt.anchorMax         = new Vector2(1f, 1f);
        ctRt.pivot             = new Vector2(0.5f, 1f);
        ctRt.sizeDelta         = new Vector2(0f, contentH);
        ctRt.anchoredPosition  = Vector2.zero;
        scroll.content         = ctRt;

        // ── Achievement rows ────────────────────────────────────────────────
        statusTexts = new TextMeshProUGUI[Achievements.Length];
        fillBars    = new Image[Achievements.Length];

        for (int i = 0; i < Achievements.Length; i++)
        {
            float y = -i * ROW_H;

            // Subtle row separator
            if (i > 0)
            {
                GameObject sep = new GameObject("Sep" + i);
                sep.transform.SetParent(content.transform, false);
                sep.AddComponent<Image>().color = new Color(1f,1f,1f,0.04f);
                RectTransform sr2 = sep.GetComponent<RectTransform>();
                sr2.anchorMin = new Vector2(0f,1f); sr2.anchorMax = new Vector2(1f,1f);
                sr2.pivot     = new Vector2(0.5f,1f);
                sr2.sizeDelta = new Vector2(-16f, 1f);
                sr2.anchoredPosition = new Vector2(0f, y);
            }

            // Achievement name
            MakeLabel(content.transform, Achievements[i].title, 14, Color.white, FontStyles.Normal,
                new Vector2(0f,1f), new Vector2(0.6f,1f), new Vector2(0f,1f),
                new Vector2(0f, 18f), new Vector2(16f, y - 7f));

            // Description
            MakeLabel(content.transform, Achievements[i].description, 10, new Color(0.45f,0.45f,0.45f), FontStyles.Normal,
                new Vector2(0f,1f), new Vector2(0.6f,1f), new Vector2(0f,1f),
                new Vector2(0f, 14f), new Vector2(16f, y - 24f));

            // Status text (right)
            GameObject sObj  = new GameObject("Status" + i);
            sObj.transform.SetParent(content.transform, false);
            statusTexts[i]   = sObj.AddComponent<TextMeshProUGUI>();
            statusTexts[i].fontSize  = 13;
            statusTexts[i].alignment = TextAlignmentOptions.MidlineRight;
            RectTransform sRt = sObj.GetComponent<RectTransform>();
            sRt.anchorMin     = new Vector2(0.6f, 1f);
            sRt.anchorMax     = new Vector2(1f,   1f);
            sRt.pivot         = new Vector2(1f, 1f);
            sRt.sizeDelta     = new Vector2(-16f, 36f);
            sRt.anchoredPosition = new Vector2(0f, y - 4f);

            // Progress bar background
            GameObject barBg = new GameObject("BarBg" + i);
            barBg.transform.SetParent(content.transform, false);
            barBg.AddComponent<Image>().color = new Color(0.18f, 0.18f, 0.18f);
            RectTransform bbRt = barBg.GetComponent<RectTransform>();
            bbRt.anchorMin = new Vector2(0f, 1f); bbRt.anchorMax = new Vector2(1f, 1f);
            bbRt.pivot     = new Vector2(0.5f, 1f);
            bbRt.sizeDelta = new Vector2(-32f, 4f);
            bbRt.anchoredPosition = new Vector2(0f, y - 46f);

            // Progress bar fill
            GameObject barFill    = new GameObject("BarFill" + i);
            barFill.transform.SetParent(barBg.transform, false);
            fillBars[i]           = barFill.AddComponent<Image>();
            fillBars[i].type      = Image.Type.Filled;
            fillBars[i].fillMethod = Image.FillMethod.Horizontal;
            fillBars[i].fillOrigin = (int)Image.OriginHorizontal.Left;
            fillBars[i].fillAmount = 0f;
            RectTransform bfRt    = barFill.GetComponent<RectTransform>();
            bfRt.anchorMin = Vector2.zero; bfRt.anchorMax = Vector2.one;
            bfRt.sizeDelta = Vector2.zero; bfRt.anchoredPosition = Vector2.zero;
        }

        // Close hint
        MakeLabel(panel.transform, "E  —  close     scroll to see more", 11,
            new Color(1f,1f,1f,0.25f), FontStyles.Normal,
            new Vector2(0f,0f), new Vector2(1f,0f), new Vector2(0.5f,0f),
            new Vector2(0f, 28f), new Vector2(0f, 8f));

        menuRoot.SetActive(false);
    }

    void MakeLabel(Transform parent, string text, float size, Color color, FontStyles style,
                   Vector2 ancMin, Vector2 ancMax, Vector2 pivot, Vector2 sizeDelta, Vector2 pos)
    {
        GameObject obj = new GameObject("Lbl");
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = size;
        tmp.color     = color;
        tmp.fontStyle = style;
        tmp.alignment = pivot.x < 0.45f ? TextAlignmentOptions.TopLeft
                      : pivot.x > 0.55f ? TextAlignmentOptions.TopRight
                      : TextAlignmentOptions.Center;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin        = ancMin;
        rt.anchorMax        = ancMax;
        rt.pivot            = pivot;
        rt.sizeDelta        = sizeDelta;
        rt.anchoredPosition = pos;
    }

    void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero; rt.anchoredPosition = Vector2.zero;
    }
}
