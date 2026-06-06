using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Grid")]
    // Fill left-to-right, bottom-to-top: index 0 = bottom-left, index 8 = top-right
    public Transform[] tiles = new Transform[9];

    [Header("Movement")]
    public float moveSpeed = 8f;

    private int gridCol = 1;
    private int gridRow = 1;
    private Coroutine slideCoroutine;

    void Start()
    {
        transform.position = TilePos(gridCol, gridRow);
    }

    void Update()
    {
        int dc = 0, dr = 0;

        if      (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))    dr =  1;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))  dr = -1;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))  dc = -1;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) dc =  1;

        if (dc == 0 && dr == 0) return;

        int newCol = Mathf.Clamp(gridCol + dc, 0, 2);
        int newRow = Mathf.Clamp(gridRow + dr, 0, 2);

        if (newCol == gridCol && newRow == gridRow) return;

        gridCol = newCol;
        gridRow = newRow;

        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideTo(TilePos(gridCol, gridRow)));
    }

    Vector3 TilePos(int col, int row)
    {
        Vector3 p = tiles[row * 3 + col].position;
        p.z = -1f;
        return p;
    }

    IEnumerator SlideTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }
}
