using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager S;
    public List<Sprite> gems = new List<Sprite>();
    public GameObject tile;
    public int xSize, ySize;
    private GameObject[,] tiles;
    public bool isShifting { get; set; }

    private void Start()
    {
        S = GetComponent<BoardManager>();
        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
    }

    private void CreateBoard(float xOffset, float yOffset)
    {
        tiles = new GameObject[xSize, ySize];
        float startX = transform.position.x;
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[ySize];
        Sprite previousBelow = null;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newTile = Instantiate(
                    tile,
                    new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0),
                    tile.transform.rotation);
                tiles[x, y] = newTile;

                newTile.transform.parent = transform;

                List<Sprite> possibleGems = new List<Sprite>();
                possibleGems.AddRange(gems);
                possibleGems.Remove(previousLeft[y]);
                possibleGems.Remove(previousBelow);

                Sprite newSprite = possibleGems[Random.Range(0, possibleGems.Count)];
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
                newTile.name = newSprite.name;

                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }
}
