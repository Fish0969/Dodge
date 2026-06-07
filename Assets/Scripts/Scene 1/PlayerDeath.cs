using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public int   particleCount    = 8;
    public float particleSpeed    = 4f;
    public float particleLifetime = 0.6f;
    public Color particleColor    = new Color(1f, 0.4f, 0f); // orange

    bool dead = false;

    public void Die()
    {
        if (dead) return;
        dead = true;

        // Size particles relative to the player's actual world size
        float size = 0.3f;
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            size = sr.bounds.size.x * 0.35f;

        Sprite square = MakeSquareSprite();

        for (int i = 0; i < particleCount; i++)
        {
            float angle = i * (360f / particleCount) * Mathf.Deg2Rad;
            float speed = particleSpeed * Random.Range(0.7f, 1.3f);
            Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

            GameObject p = new GameObject("DeathParticle");
            p.transform.position   = transform.position;
            p.transform.localScale = Vector3.one * size;

            SpriteRenderer psr = p.AddComponent<SpriteRenderer>();
            psr.sprite       = square;
            psr.color        = particleColor;
            psr.sortingOrder = 10;

            ExplosionParticle ep = p.AddComponent<ExplosionParticle>();
            ep.Init(vel, particleLifetime, size);
        }

        CoinSpawner coinSpawner = FindFirstObjectByType<CoinSpawner>();
        if (coinSpawner != null)
            coinSpawner.StopAndClear();

        // Restore time in case the slow orb effect was active
        CoinPickup coinPickup = FindFirstObjectByType<CoinPickup>();
        if (coinPickup != null)
        {
            int best = PlayerPrefs.GetInt("BestScore", 0);
            if (coinPickup.score > best)
            {
                PlayerPrefs.SetInt("BestScore", coinPickup.score);
                PlayerPrefs.Save();
            }
            coinPickup.ResetSlow();
        }
        else
            Time.timeScale = 1f;

        Destroy(gameObject);
    }

    Sprite MakeSquareSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
    }
}

public class ExplosionParticle : MonoBehaviour
{
    Vector2 velocity;
    float   lifetime;
    float   startSize;
    float   elapsed;
    SpriteRenderer sr;

    public void Init(Vector2 vel, float life, float size)
    {
        velocity  = vel;
        lifetime  = life;
        startSize = size;
        sr        = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / lifetime;

        transform.position   += (Vector3)(velocity * Time.deltaTime);
        transform.localScale  = Vector3.one * startSize * (1f - t);

        if (sr != null)
        {
            Color c = sr.color;
            c.a      = 1f - t;
            sr.color = c;
        }

        if (elapsed >= lifetime) Destroy(gameObject);
    }
}
