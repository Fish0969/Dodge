using System.Collections.Generic;
using UnityEngine;

public static class ChallengeData
{
    public struct Challenge
    {
        public string name;
        public string description;
        public int    coinsRequired;
        public float  timeLimit;
        public int    rewardCoins;

        public Challenge(string n, string d, int c, float t, int r)
        { name = n; description = d; coinsRequired = c; timeLimit = t; rewardCoins = r; }
    }

    public static readonly Challenge[] All =
    {
        new Challenge("Starter",      "Collect 2 coins in 10 seconds",  2, 10f,  10),
        new Challenge("Warm Up",      "Collect 3 coins in 10 seconds",  3, 10f,  15),
        new Challenge("Flash",        "Collect 3 coins in 6 seconds",   3,  6f,  20),
        new Challenge("Lightning",    "Collect 4 coins in 8 seconds",   4,  8f,  25),
        new Challenge("Speed Rush",   "Collect 5 coins in 10 seconds",  5, 10f,  30),
        new Challenge("Blitz",        "Collect 5 coins in 8 seconds",   5,  8f,  35),
        new Challenge("Turbo",        "Collect 6 coins in 10 seconds",  6, 10f,  40),
        new Challenge("Coin Frenzy",  "Collect 6 coins in 8 seconds",   6,  8f,  45),
        new Challenge("Overdrive",    "Collect 8 coins in 12 seconds",  8, 12f,  50),
        new Challenge("Speed Demon",  "Collect 8 coins in 10 seconds",  8, 10f,  55),
        new Challenge("Hyper",        "Collect 10 coins in 14 seconds", 10, 14f, 60),
        new Challenge("Elite Run",    "Collect 10 coins in 12 seconds", 10, 12f, 65),
        new Challenge("Nightmare",    "Collect 12 coins in 14 seconds", 12, 14f, 75),
        new Challenge("Insane Rush",  "Collect 12 coins in 12 seconds", 12, 12f, 85),
        new Challenge("God Run",      "Collect 15 coins in 14 seconds", 15, 14f,110),
        new Challenge("Transcendent", "Collect 18 coins in 15 seconds", 18, 15f,150),
    };

    public static Challenge GetActive()
    {
        int idx = Mathf.Clamp(PlayerPrefs.GetInt("ChallengeIndex", 0), 0, All.Length - 1);
        return All[idx];
    }

    // Call after every challenge attempt — picks the next unique challenge
    public static void AdvanceChallenge()
    {
        int current = Mathf.Clamp(PlayerPrefs.GetInt("ChallengeIndex", 0), 0, All.Length - 1);

        List<int> queue = ParseQueue(PlayerPrefs.GetString("ChallengeQueue", ""));
        if (queue.Count == 0)
            queue = ShuffledQueueExcluding(current);

        PlayerPrefs.SetInt("ChallengeIndex", queue[0]);
        queue.RemoveAt(0);
        PlayerPrefs.SetString("ChallengeQueue", SerializeQueue(queue));
        PlayerPrefs.Save();
    }

    static List<int> ShuffledQueueExcluding(int exclude)
    {
        var pool = new List<int>();
        for (int i = 0; i < All.Length; i++)
            if (i != exclude) pool.Add(i);

        var rng = new System.Random();
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            int tmp = pool[i]; pool[i] = pool[j]; pool[j] = tmp;
        }
        return pool;
    }

    static List<int> ParseQueue(string s)
    {
        var list = new List<int>();
        if (string.IsNullOrEmpty(s)) return list;
        foreach (string part in s.Split(','))
            if (int.TryParse(part.Trim(), out int v)) list.Add(v);
        return list;
    }

    static string SerializeQueue(List<int> q) => string.Join(",", q);
}
