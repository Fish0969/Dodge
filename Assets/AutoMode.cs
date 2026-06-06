using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoMode : MonoBehaviour
{
    public Movement    movement;
    public Transform[] tiles = new Transform[9];

    bool            autoOn = false;
    TextMeshProUGUI label;

    float gridMinX, gridMaxX, gridMinY, gridMaxY;

    void Start()
    {
        gridMinX = tiles[0].position.x;
        gridMaxX = tiles[2].position.x;
        gridMinY = tiles[0].position.y;
        gridMaxY = tiles[6].position.y;

        BuildUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            autoOn     = !autoOn;
            label.text = autoOn ? "Auto Mode On" : "";
        }

        if (!autoOn || movement.IsMoving) return;

        Think();
    }

    void Think()
    {
        HashSet<int> threatenedRows = new HashSet<int>();
        HashSet<int> threatenedCols = new HashSet<int>();

        foreach (Obstacle obs in FindObjectsByType<Obstacle>(FindObjectsSortMode.None))
        {
            Vector3 p = obs.transform.position;
            bool inRange =
                obs.direction.x > 0 ? p.x <= gridMaxX :
                obs.direction.x < 0 ? p.x >= gridMinX :
                obs.direction.y > 0 ? p.y <= gridMaxY :
                                      p.y >= gridMinY;

            if (!inRange) continue;
            foreach (int r in obs.threatenedRows) threatenedRows.Add(r);
            foreach (int c in obs.threatenedCols) threatenedCols.Add(c);
        }

        int bestCol = -1, bestRow = -1, bestScore = -1;

        for (int col = 0; col < 3; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                if (threatenedRows.Contains(row) || threatenedCols.Contains(col)) continue;

                int score = 0;
                if (col == movement.GridCol && row == movement.GridRow) score += 1;
                if (HasCoinAt(col, row)) score += 10;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestCol   = col;
                    bestRow   = row;
                }
            }
        }

        if (bestCol < 0) return;

        int dc = bestCol == movement.GridCol ? 0 : (bestCol > movement.GridCol ? 1 : -1);
        int dr = bestRow == movement.GridRow ? 0 : (bestRow > movement.GridRow ? 1 : -1);

        if (Mathf.Abs(bestCol - movement.GridCol) >= Mathf.Abs(bestRow - movement.GridRow))
            movement.RequestMove(dc, 0);
        else
            movement.RequestMove(0, dr);
    }

    bool HasCoinAt(int col, int row)
    {
        try
        {
            GameObject[] coins   = GameObject.FindGameObjectsWithTag("Coin");
            Vector3      tilePos = tiles[row * 3 + col].position;
            foreach (GameObject coin in coins)
                if (Vector2.Distance(coin.transform.position, tilePos) < 0.3f)
                    return true;
        }
        catch { }
        return false;
    }

    void BuildUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject cObj = new GameObject("AutoCanvas");
            canvas = cObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            cObj.AddComponent<CanvasScaler>();
            cObj.AddComponent<GraphicRaycaster>();
        }

        GameObject textObj = new GameObject("AutoModeLabel");
        textObj.transform.SetParent(canvas.transform, false);
        label           = textObj.AddComponent<TextMeshProUGUI>();
        label.text      = "";
        label.fontSize  = 18;
        label.color     = Color.white;
        label.alignment = TextAlignmentOptions.TopRight;

        RectTransform rt    = textObj.GetComponent<RectTransform>();
        rt.anchorMin        = Vector2.one;
        rt.anchorMax        = Vector2.one;
        rt.pivot            = Vector2.one;
        rt.sizeDelta        = new Vector2(200f, 40f);
        rt.anchoredPosition = new Vector2(-10f, -10f);
    }
}
