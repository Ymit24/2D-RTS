using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
using Game.Task;
using Game.Pathfinding;

namespace Game
{
    public class World
    {
        static public World current;
        public Action<Building> OnBuildingBuilt, OnBuildingDestroyed;
        public Action<Worker> OnWorkerCreated, OnWorkerDestroyed;
        public Action<Combat> OnCombatCreated, OnCombatDestroyed;
        
        Tile[,] tiles;
        int width;
        int height;

        private List<Worker> workers;
        private TaskSystem taskSystem;
        private Pathfinder pathfinder;

        private Dictionary<BuildingType, Building> buildingPrototypes;

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public TaskSystem TaskSystem
        {
            get
            {
                return taskSystem;
            }
        }

        public Pathfinder Pathfinder
        {
            get
            {
                return pathfinder;
            }
        }
        
        public World(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.tiles = new Tile[width, height];

            current = this;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    this.tiles[x, y] = new Tile(x, y);
                }
            }

            taskSystem = new TaskSystem();
            pathfinder = new Pathfinder(width,height,false);
            buildingPrototypes = new Dictionary<BuildingType, Building>();
            BuildPrototype(BuildingType.HQ, 3, 2.5f);
            BuildPrototype(BuildingType.BARRACKS, 2, 1.0f);
            BuildPrototype(BuildingType.GOLDMINE, 1, 0.5f);
            
            UpdatePathfindingGraph();
            
            workers = new List<Worker>();
        }

        private void UpdatePathfindingGraph()
        {
            bool[,] obstacles = new bool[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    obstacles[x, y] = tiles[x, y].Walkable == false;
                }
            }
            pathfinder.WeightGraph(obstacles);
        }

        public Tile GetTileAt(TileCoord coord)
        {
            return GetTileAt(coord.x, coord.y);
        }
        
        /// <summary>
        /// Get's the tile at the given position.
        /// (Casts to int to determine tile)
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <returns></returns>
        public Tile GetTileAt(float tileX, float tileY)
        {
            if (tileX < 0 || tileX > width - 1 || tileY < 0 || tileY > height - 1) return null;
            return tiles[(int)tileX, (int)tileY];
        }

        public Worker GetWorkerAt(TileCoord tileCoord)
        {
            foreach (Worker worker in workers)
            {
                if (worker.Tile.TileCoord == tileCoord)
                {
                    return worker;
                }
            }

            return null;
        }

        public Worker[] GetWorkers()
        {
            return workers.ToArray();
        }
        
        // this might need to be replaced with a building prototyping system
        public void BuildPrototype(BuildingType type, int size, float buildTime = 1)
        {
            if (buildingPrototypes.ContainsKey(type) == false)
            {
                buildingPrototypes.Add(type, new Building(type, size, buildTime));
            }
        }
        
        public Building CreateFromPrototype(BuildingType type)
        {
            return new Building(buildingPrototypes.ContainsKey(type) ? buildingPrototypes[type] : null);
        }

        public bool CanPlaceBuilding(Building building, int tileX, int tileY)
        {
            for (int i = 0; i < building.Size; i++)
            {
                for (int j = 0; j < building.Size; j++)
                {
                    Tile tile = GetTileAt(i + tileX, j + tileY);
                    if (tile == null)
                    {
                        return false;
                    }

                    if (tile.IsAvailable(building.BuildingId) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        public bool CanPlaceBuilding(BuildingType type, int tileX, int tileY)
        {
            for (int i = 0; i < buildingPrototypes[type].Size; i++)
            {
                for (int j = 0; j < buildingPrototypes[type].Size; j++)
                {
                    Tile tile = GetTileAt(i + tileX, j + tileY);
                    if (tile == null)
                    {
                        return false;
                    }

                    if (tile.Building != null || tile.ReserveId != -1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool ReserveBuilding(Building building, Tile root)
        {
            if (CanPlaceBuilding(building, root.X, root.Y) == false) return false;
            for (int i = 0; i < building.Size; i++)
            {
                for (int j = 0; j < building.Size; j++)
                {
                    Tile tile = GetTileAt(i + root.X, j + root.Y);
                    tile.Reserve(building.BuildingId);
                }
            }

            return true;
        }
        
        public bool PlaceBuilding(Building building, Tile root)
        {
            if (CanPlaceBuilding(building, root.X, root.Y) == false) return false;
            Tile[] buildingtiles = new Tile[building.Size * building.Size];
            buildingtiles[0] = root;
            for (int i = 0; i < building.Size; i++)
            {
                for (int j = 0; j < building.Size; j++)
                {
                    Tile tile = GetTileAt(i + root.X, j + root.Y);
                    if (tile == null) continue;
                    tile.PlaceBuilding(building);
                    buildingtiles[i + j * building.Size] = tile;
                }
            }

            building.PlaceOnTiles(buildingtiles);
            if (OnBuildingBuilt != null)
            {
                OnBuildingBuilt(building);
            }
            UpdatePathfindingGraph();
            return true;
        }

        public void DestroyBuilding(Building building)
        {
            for (int i = 0; i < building.Size; i++)
            {
                for (int j = 0; j < building.Size; j++)
                {
                    Tile tile = GetTileAt(i + building.RootTile.X, j + building.RootTile.Y);
                    if (tile == null) continue;
                    tile.RemoveBuilding();
                }
            }
            if (OnBuildingDestroyed != null)
            {
                OnBuildingDestroyed(building);
            }
            UpdatePathfindingGraph();
        }

        // Should be called from WorldController,
        // one of the few externally required calls
        public void Tick(float deltaTime)
        {
            foreach (Worker worker in workers)
            {
                worker.Tick(deltaTime);
            }
        }

        public void CreateWorker(TileCoord position)
        {
            Worker worker = new Worker(position, taskSystem, pathfinder);
            workers.Add(worker);
            if (OnWorkerCreated != null)
            {
                OnWorkerCreated(worker);
            }
        }

        public void DestroyWorker(Worker worker)
        {
            if (worker.CurrentTask != null)
            {
                taskSystem.AddTask(worker.CurrentTask);
            }
            workers.Remove(worker);
            if (OnWorkerDestroyed != null)
            {
                OnWorkerDestroyed(worker);
            }
        }

        public void CreateCombat(TileCoord position)
        {
            
        }
    }

    /*
    
    class NetWorld : World {
        private INetServer server;
        public override void CreateWorker(TileCoord coord) : base(coord) {
            server.Send(...);
        }
    }

    */
}