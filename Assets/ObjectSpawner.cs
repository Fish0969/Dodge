using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("References")]
    // Same tiles array as Movement.cs — fill identically
    public Transform[] tiles = new Transform[9];
    public Transform player;
    public Sprite obstacleSprite;

    [Header("Spawning")]
    public float spawnInterval = 1.5f;
    public float moveSpeed = 4f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnObstacle();
        }
    }

    void SpawnObstacle()
    {
        Camera cam = Camera.main;
        float camH = cam.orthographicSize;
        float camW = camH * cam.aspect;
        float pad  = 1f;

        Vector2 tileSize = tiles[0].GetComponent<SpriteRenderer>().bounds.size;
        float tileW = tileSize.x;
        float tileH = tileSize.y;

        // Collect rows/cols already threatened by active obstacles
        HashSet<int> curRows = new HashSet<int>();
        HashSet<int> curCols = new HashSet<int>();
        foreach (Obstacle obs in FindObjectsByType<Obstacle>(FindObjectsSortMode.None))
        {
            foreach (int r in obs.threatenedRows) curRows.Add(r);
            foreach (int c in obs.threatenedCols) curCols.Add(c);
        }

        // Try up to 10 random combos until one leaves a safe tile
        for (int attempt = 0; attempt < 10; attempt++)
        {
            int  side    = Random.Range(0, 4);
            bool stretch = Random.value > 0.5f;

            int[]   newRows;
            int[]   newCols;
            Vector3 spawnPos;
            Vector2 direction;
            Vector3 scale;

            if (side == 0 || side == 1) // left / right — threatens rows
            {
                int startRow = stretch ? Random.Range(0, 2) : Random.Range(0, 3);
                newRows = stretch ? new[] { startRow, startRow + 1 } : new[] { startRow };
                newCols = new int[0];

                float y      = stretch
                    ? (tiles[startRow * 3].position.y + tiles[(startRow + 1) * 3].position.y) / 2f
                    : tiles[startRow * 3].position.y;
                float scaleY = stretch ? tileH * 2f + tileH * 0.5f : tileH;
                float x      = side == 0 ? -camW - pad : camW + pad;
                spawnPos     = new Vector3(x, y, 0f);
                direction    = side == 0 ? Vector2.right : Vector2.left;
                scale        = new Vector3(tileW, scaleY, 1f);
            }
            else // top / bottom — threatens cols
            {
                int startCol = stretch ? Random.Range(0, 2) : Random.Range(0, 3);
                newCols = stretch ? new[] { startCol, startCol + 1 } : new[] { startCol };
                newRows = new int[0];

                float x      = stretch
                    ? (tiles[startCol].position.x + tiles[startCol + 1].position.x) / 2f
                    : tiles[startCol].position.x;
                float scaleX = stretch ? tileW * 2f + tileW * 0.5f : tileW;
                float y      = side == 2 ? -camH - pad : camH + pad;
                spawnPos     = new Vector3(x, y, 0f);
                direction    = side == 2 ? Vector2.up : Vector2.down;
                scale        = new Vector3(scaleX, tileH, 1f);
            }

            // Check: after adding this obstacle, is there still at least 1 safe row AND 1 safe col?
            HashSet<int> testRows = new HashSet<int>(curRows);
            HashSet<int> testCols = new HashSet<int>(curCols);
            foreach (int r in newRows) testRows.Add(r);
            foreach (int c in newCols) testCols.Add(c);

            if (testRows.Count < 3 && testCols.Count < 3)
            {
                CreateObstacle(spawnPos, scale, direction, newRows, newCols, camW, camH);
                return;
            }
        }
        // No fair lane found — skip this spawn cycle
    }

    void CreateObstacle(Vector3 pos, Vector3 scale, Vector2 direction, int[] rows, int[] cols, float camW, float camH)
    {
        GameObject obj = new GameObject("Obstacle");
        obj.transform.position   = pos;
        obj.transform.localScale = scale;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite       = obstacleSprite;
        sr.color        = Color.red;
        sr.sortingOrder = 1;

        BoxCollider2D col2d = obj.AddComponent<BoxCollider2D>();
        col2d.isTrigger = true;

        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType     = RigidbodyType2D.Kinematic;

        Obstacle obstacle = obj.AddComponent<Obstacle>();
        obstacle.Init(direction, moveSpeed, player, camW, camH, rows, cols);
    }
}
