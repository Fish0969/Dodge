using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int[] threatenedRows = new int[0];
    public int[] threatenedCols = new int[0];

    public Vector2 direction;
    public float speed;
    private Transform player;
    private float camW;
    private float camH;

    public void Init(Vector2 dir, float spd, Transform plr, float cw, float ch, int[] rows, int[] cols)
    {
        direction      = dir;
        speed          = spd;
        player         = plr;
        camW           = cw;
        camH           = ch;
        threatenedRows = rows;
        threatenedCols = cols;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        Vector3 p = transform.position;
        if (p.x < -camW - 2f || p.x > camW + 2f ||
            p.y < -camH - 2f || p.y > camH + 2f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (player != null && other.transform == player)
        {
            if (CoinPickup.IsRainbow)
            {
                Destroy(gameObject);
                return;
            }

            PlayerDeath death = player.GetComponent<PlayerDeath>();
            if (death != null)
                death.Die();
            else
                Destroy(player.gameObject);

            Destroy(gameObject);
        }
    }
}
