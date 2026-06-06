using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public float pickupRadius = 0.3f;
    public int score = 0;

    void Update()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject coin in coins)
        {
            if (Vector2.Distance(transform.position, coin.transform.position) < pickupRadius)
            {
                score++;
                Destroy(coin);
            }
        }
    }
}
