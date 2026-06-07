using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CPSCounter : MonoBehaviour
{
    TextMeshProUGUI label;

    // Timestamps of recent key presses within the last second
    Queue<float> pressTimestamps = new Queue<float>();

    static readonly KeyCode[] watchedKeys =
    {
        KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D,
        KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow
    };

    void Start()
    {
        BuildUI();
    }

    void Update()
    {
        float now = Time.unscaledTime;

        // Record any key press this frame
        foreach (KeyCode key in watchedKeys)
            if (Input.GetKeyDown(key))
                pressTimestamps.Enqueue(now);

        // Drop presses older than 1 second
        while (pressTimestamps.Count > 0 && now - pressTimestamps.Peek() > 1f)
            pressTimestamps.Dequeue();

        label.text = "CPS: " + pressTimestamps.Count;
    }

    void BuildUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject cObj = new GameObject("CPSCanvas");
            canvas = cObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            cObj.AddComponent<CanvasScaler>();
            cObj.AddComponent<GraphicRaycaster>();
        }

        GameObject textObj = new GameObject("CPSLabel");
        textObj.transform.SetParent(canvas.transform, false);
        label           = textObj.AddComponent<TextMeshProUGUI>();
        label.text      = "CPS: 0";
        label.fontSize  = 18;
        label.color     = Color.white;
        label.alignment = TextAlignmentOptions.BottomLeft;

        RectTransform rt    = textObj.GetComponent<RectTransform>();
        rt.anchorMin        = Vector2.zero;
        rt.anchorMax        = Vector2.zero;
        rt.pivot            = Vector2.zero;
        rt.sizeDelta        = new Vector2(120f, 40f);
        rt.anchoredPosition = new Vector2(10f, 10f);
    }
}
