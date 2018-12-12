using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class DummyNetworkUpdater : MonoBehaviour {

    private World world;
	void Awake ()
	{
		WorldController.OnCreate += OnCreate;
	}

	private void OnCreate()
	{
		world = World.current;
		world.OnBuildingBuilt += OnBuildingBuilt;
		world.OnBuildingDestroyed += OnBuildingDestroyed;
		world.OnWorkerCreated += OnWorkerCreated;
		world.OnWorkerDestroyed += OnWorkerDestroyed;

		for (int x = 0; x < world.Width; x++)
		{
			for (int y = 0; y < world.Height; y++)
			{
				Tile tile = world.GetTileAt(x, y);
				if (tile != null)
				{
					tile.OnReserve += OnTileReserved;
					tile.OnUnreserve += OnTileUnreserved;
				}
			}
		}
	}

	private void OnTileUnreserved(Tile tile)
	{
		Debug.Log("Tile at " + tile.TileCoord.ToString() + " was unreserved");
	}

	private void OnTileReserved(Tile tile, int reserveID)
	{
		Debug.Log("Tile at " + tile.TileCoord.ToString() + " was reserved with an ID of " + reserveID);
	}
	
	private void OnBuildingBuilt(Building building)
	{
		Debug.Log(
			"Building built of type " + building.Type.ToString() + " at " + building.RootTile.TileCoord.ToString());
	}
	
	private void OnBuildingDestroyed(Building building)
	{
		Debug.Log(
		"Building destroyed of type " + building.Type.ToString() + " at " + building.RootTile.TileCoord.ToString());
	}

	private void OnWorkerCreated(Worker worker)
	{
		Debug.Log("Worker created at " + worker.Position.ToString());
		worker.OnMove += OnWorkerMove;
	}

	private void OnWorkerDestroyed(Worker worker)
	{
		Debug.Log("Worker destroyed at " + worker.Position.ToString());
		worker.OnMove -= OnWorkerMove;
	}

	private void OnWorkerMove(Worker worker)
	{
		//Debug.Log("Worker moved to " + worker.Position.ToString());
	}
}
