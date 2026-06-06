using System.Collections;
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

        // Tile spacing derived from grid positions
        float tileW = Mathf.Abs(tiles[1].position.x - tiles[0].position.x);
        float tileH = Mathf.Abs(tiles[3].position.y - tiles[0].position.y);

        int side      = Random.Range(0, 4);   // 0=left 1=right 2=bottom 3=top
        bool stretch  = Random.value > 0.5f;  // 50% chance to span 2 tiles

        Vector3 spawnPos;
        Vector2 direction;
        Vector3 scale;

        if (side == 0 || side == 1) // left / right — moves on X, stretches on Y
        {
            float y;
            float scaleY;

            if (stretch)
            {
                int startRow = Random.Range(0, 2); // rows 0-1 or 1-2
                y      = (tiles[startRow * 3].position.y + tiles[(startRow + 1) * 3].position.y) / 2f;
                scaleY = tileH * 2f;
            }
            else
            {
                int row = Random.Range(0, 3);
                y      = tiles[row * 3].position.y;
                scaleY = tileH;
            }

            float x  = side == 0 ? -camW - pad : camW + pad;
            spawnPos  = new Vector3(x, y, 0f);
            direction = side == 0 ? Vector2.right : Vector2.left;
            scale     = new Vector3(tileW, scaleY, 1f);
        }
        else // top / bottom — moves on Y, stretches on X
        {
            float x;
            float scaleX;

            if (stretch)
            {
                int startCol = Random.Range(0, 2); // cols 0-1 or 1-2
                x      = (tiles[startCol].position.x + tiles[startCol + 1].position.x) / 2f;
                scaleX = tileW * 2f;
            }
            else
            {
                int col = Random.Range(0, 3);
                x      = tiles[col].position.x;
                scaleX = tileW;
            }

            float y  = side == 2 ? -camH - pad : camH + pad;
            spawnPos  = new Vector3(x, y, 0f);
            direction = side == 2 ? Vector2.up : Vector2.down;
            scale     = new Vector3(scaleX, tileH, 1f);
        }

        GameObject obj = new GameObject("Obstacle");
        obj.transform.position   = spawnPos;
        obj.transform.localScale = scale;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite       = obstacleSprite;
        sr.sortingOrder = 1;

        BoxCollider2D col2d = obj.AddComponent<BoxCollider2D>();
        col2d.isTrigger = true;

        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType     = RigidbodyType2D.Kinematic;

        Obstacle obstacle = obj.AddComponent<Obstacle>();
        obstacle.Init(direction, moveSpeed, player, camW, camH);
    }
}
