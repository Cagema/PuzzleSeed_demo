using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected = null;

	private SpriteRenderer render;
	private bool isSelected = false;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	private bool matchFound = false;
	void Awake()
	{
		render = GetComponent<SpriteRenderer>();
	}

	private void Select()
	{
		isSelected = true;
		render.color = selectedColor; // Change color selected sprite
		previousSelected = gameObject.GetComponent<Tile>();
		// *Sound* SFXManager.S.PlaySFX(Clip.Select);
	}

	private void Deselect()
	{
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}

    private void OnMouseDown()
    {
		if (render.sprite == null || BoardManager.S.isShifting)
        {
			return;
        }

		if (isSelected)
        {
			// Already selected?
			Deselect();
        }
        else
        {
			if (previousSelected == null)
            {
				// it is first tile selected
				Select();
            }
            else
            {
                if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
                {
					SwapSprite(previousSelected.render);
					previousSelected.ClearAllMatches();
					previousSelected.Deselect();
					ClearAllMatches();
					//BoardManager.S.FindComboTiles();
				}
                else
                {
					// Select not adjacent sprite
					previousSelected.Deselect();
					Select();
                }
            }
        }
    }

	public void SwapSprite(SpriteRenderer secondRender)
    {
		if (render.sprite == secondRender.sprite)
        {
			// Don't swap same sprites
			return;
        }

		Sprite tempSprite = secondRender.sprite;
		string tempName = secondRender.gameObject.name; //***

		secondRender.sprite = render.sprite;
		secondRender.gameObject.name = render.gameObject.name; //***

		render.sprite = tempSprite;
		render.gameObject.name = tempName; //***
		// *Sound* SFXManager.S.PlaySFX(Clip.Swap);
    }

	private GameObject GetAdjacent(Vector2 castDir)
    {
		RaycastHit2D hit = Physics2D.Raycast(transform.position + (new Vector3(castDir.x, castDir.y, 0) * 2.56f), castDir);
		if (hit.collider != null)
        {
			return hit.collider.gameObject;
        }
		return null;
    }

	private List<GameObject> GetAllAdjacentTiles()
    {
		List<GameObject> adjacentTiles = new List<GameObject>();
		for (int i = 0; i < adjacentDirections.Length; i++)
        {
			adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
		return adjacentTiles;
    }

	private List<GameObject> FindMatch(Vector2 castDir)
    {
		List<GameObject> matchingTiles = new List<GameObject>();
		RaycastHit2D hit = Physics2D.Raycast(transform.position + (new Vector3(castDir.x, castDir.y, 0) * 2.56f), castDir);
		while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
        {
			matchingTiles.Add(hit.collider.gameObject);
			hit = Physics2D.Raycast(hit.collider.transform.position + (new Vector3(castDir.x, castDir.y, 0) * 2.56f), castDir);
		}
		
		return matchingTiles;
	}

	private void ClearMatch(Vector2[] paths)
    {
		List<GameObject> matchingTiles = new List<GameObject>();
		for (int i = 0; i < paths.Length; i++)
        {
			matchingTiles.AddRange(FindMatch(paths[i]));
        }
		if (matchingTiles.Count >= 2)
        {
			for (int i = 0; i < matchingTiles.Count; i++)
            {
				matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }

			matchFound = true;
		}
    }

	public void ClearAllMatches()
    {
		if (render.sprite == null)
        {
			return;
        }

		ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
		ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
		if (matchFound)
        {
			render.sprite = null;
			matchFound = false;
            StopCoroutine(BoardManager.S.FindNullTiles());
			StartCoroutine(BoardManager.S.FindNullTiles());
			// *Sound* SFXManager.S.PlaySFX(Clip.Clear);
		}
    }
}
