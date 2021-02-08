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
					previousSelected.Deselect();
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
		secondRender.sprite = render.sprite;
		render.sprite = tempSprite;
		// *Sound* SFXManager.S.PlaySFX(Clip.Swap);
    }

	private GameObject GetAdjacent(Vector2 castDir)
    {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
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
}
