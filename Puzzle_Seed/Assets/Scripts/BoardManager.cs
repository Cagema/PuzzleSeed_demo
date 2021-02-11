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

    public IEnumerator FindNullTiles()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
            }
        }
        FindComboTiles();
    }

    public void FindComboTiles()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                tiles[x, y].GetComponent<Tile>().ClearAllMatches();
            }
        }
    }

    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .4f)
    {
        
        isShifting = true;
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;

        for (int y = yStart; y < ySize; y++)
        {
            SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
            if (render.sprite == null)
            {
                nullCount++;
            }
            renders.Add(render);

            print("x = " + x + ", y = " + y + ", " + render.name);
        }
        print("done!");
        
        for (int i = 0; i < nullCount; i++)
        {
            yield return new WaitForSeconds(shiftDelay);
            if (renders.Count == 1 && nullCount == 1)
            {
                renders[0].sprite = GetNewSprite(x, ySize - 1);
                break;
            }
            for (int k = 0; k < renders.Count - 1; k++)
            {
                renders[k].sprite = renders[k + 1].sprite;
                renders[k + 1].sprite = GetNewSprite(x, ySize - 1);
            }

        }
        print("down done");

        isShifting = false;
    }

    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> possibleGems = new List<Sprite>();
        possibleGems.AddRange(gems);

       /* if (x > 0)
        {
            possibleGems.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < xSize - 1)
        {
            possibleGems.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0)
        {
            possibleGems.Remove(tiles[x, y-1].GetComponent<SpriteRenderer>().sprite);
        }
       */
        return possibleGems[Random.Range(0, possibleGems.Count)];
    }
}
