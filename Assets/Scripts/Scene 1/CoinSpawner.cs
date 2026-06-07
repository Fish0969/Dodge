using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("References")]
    // Same tiles array as Movement.cs — fill identically
    public Transform[] tiles = new Transform[9];
    public GameObject coinPrefab;
    [Range(1, 50)]  public int blueOrbChance    = 15;
    [Range(1, 500)] public int rainbowOrbChance = 100;

    private const int MAX_COINS = 2;
    private List<GameObject> activeCoins = new List<GameObject>();
    private bool active = true;

    void Start()
    {
        for (int i = 0; i < MAX_COINS; i++)
            SpawnCoin();
    }

    void Update()
    {
        if (!active) return;

        activeCoins.RemoveAll(c => c == null);

        while (activeCoins.Count < MAX_COINS)
            SpawnCoin();
    }

    public void StopAndClear()
    {
        active = false;
        foreach (GameObject coin in activeCoins)
            if (coin != null) Destroy(coin);
        activeCoins.Clear();
    }

    void SpawnCoin()
    {
        int tileIndex = RandomFreeTile();
        if (tileIndex < 0) return;

        Vector3 pos = tiles[tileIndex].position;
        pos.z = -0.5f;

        GameObject coin = Instantiate(coinPrefab, pos, Quaternion.identity);

        if (Random.Range(0, blueOrbChance) == 0)
        {
            coin.AddComponent<BlueOrb>();
            SpriteRenderer sr = coin.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.color = new Color(0.2f, 0.5f, 1f);
        }
        else if (Random.Range(0, rainbowOrbChance) == 0)
        {
            coin.AddComponent<RainbowOrb>();
        }

        activeCoins.Add(coin);
    }

    int RandomFreeTile()
    {
        List<int> occupied = new List<int>();
        foreach (GameObject c in activeCoins)
        {
            if (c == null) continue;
            for (int i = 0; i < tiles.Length; i++)
            {
                if (Vector2.Distance(c.transform.position, tiles[i].position) < 0.01f)
                    occupied.Add(i);
            }
        }

        List<int> free = new List<int>();
        for (int i = 0; i < tiles.Length; i++)
            if (!occupied.Contains(i))
                free.Add(i);

        if (free.Count == 0) return -1;
        return free[Random.Range(0, free.Count)];
    }
}
