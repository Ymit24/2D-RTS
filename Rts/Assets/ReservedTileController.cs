using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ReservedTileController : MonoBehaviour
{
	private WorldController worldController;
	private SpriteRenderer[,] grid;

	public Sprite OverlaySprite;
	public string OverlayLayer;
	private int width;
	private int height;

	void Awake()
	{
		WorldController.Create += OnCreate;
	}
	
	void OnCreate ()
	{
		worldController = WorldController.current;

		width = worldController.World.Width;
		height = worldController.World.Height;
		grid = new SpriteRenderer[width,height];
		GameObject go = new GameObject("Reservered Tiles Overlay");
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				GameObject g = new GameObject("ReservedTileVisual");
				g.transform.SetParent(go.transform);
				SpriteRenderer r = g.AddComponent<SpriteRenderer>();
				r.color = Color.blue;
				r.sprite = OverlaySprite;
				r.sortingLayerName = OverlayLayer;
				
				g.transform.position = new Vector3(x * WorldController.TileSize, y * WorldController.TileSize, 0);
				g.transform.localScale = Vector2.one * (WorldController.TileSize * 0.75f);

				grid[x, y] = r;
			}
		}
	}

	private void UpdateTiles()
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				grid[x, y].color = worldController.World.GetTileAt(x, y).IsAvailable(-1) ? Color.white : Color.red;
			}
		}
	}
	
	void Update () {
		UpdateTiles();

		if (Input.GetKeyDown(KeyCode.H))
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					grid[x, y].enabled = !grid[x,y].enabled;
				}
			}
		}
	}
}
