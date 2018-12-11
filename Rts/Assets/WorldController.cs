using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Task;
using Game.Pathfinding;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldController : MonoBehaviour
{
    static public WorldController current;

    public static Action Create;
    
    protected void Awake()
    {
        if (current == null) current = this;
    }
    
    private Game.World world;
    // Probably want a sprite manager, or specific TileController to manage this
    public Sprite TileSprite;
    public string TileLayer;
    
    [SerializeField] private int width;
    [SerializeField] private int height;
    
    public static readonly int TileSize = 32;

    public Game.World World
    {
        get
        {
            return world;
        }
    }
    
    protected void Start()
    {
        world = new World(width, height);

        GameObject tileHolder = new GameObject("Tiles");
        // either call a TileController.CreateTiles(world) function or make a TileController function call per tile.
        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                // Probably should call a TileController.CreateTile(x,y,prototype/type) function
                GameObject tile = new GameObject("Tile_" + x + "_" + y);
                
                tile.transform.SetParent(tileHolder.transform);
                tile.transform.position = new Vector2(x * TileSize, y * TileSize);
                tile.transform.localScale = new Vector2(TileSize, TileSize);
                
                SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                sr.sprite = TileSprite;
                sr.sortingLayerName = TileLayer;
                sr.color = new Color(0.08f, 0.5f, 0.18f);
            }
        }

        BuildingController.current.Setup();
        WorkerController.current.Setup();
        // Probably want this to belong to a BuildingController
        
        // That function likely will be removed so this is okay temporary code.
        MouseController.Create();
        MouseController.OnClick += OnClick;
        
        
        if (Create != null)
            Create();
    }

    

    // Probably want this to belong to a Utils class somewhere
    protected TileCoord ScreenToTileCoords(Vector2 position)
    {
        Vector2 mouse = (Vector2) Camera.main.ScreenToWorldPoint(position) + (Vector2.one * TileSize / 2);
        int tileX = (int) (Mathf.Floor(mouse.x / TileSize));
        int tileY = (int) (Mathf.Floor(mouse.y / TileSize));
        return new TileCoord(tileX, tileY);
    }


    protected void Update()
    {
        world.Tick(Time.fixedDeltaTime);
    }

    private int buildIndex = 0;
    public void SetBuildIndex(int index)
    {
        buildIndex = index;
    }

    private int action = 0;

    public void SetAction(int action)
    {
        this.action = action;
    }
    
    // Temporary debug code, okay for now.
    private GatherTask currentGatherTask;
    protected void OnClick(Vector2 position, int button, bool down)
    {
        if (down == false) return;
        
        TileCoord tileCoord = ScreenToTileCoords(position);
        Tile tile = world.GetTileAt((int)tileCoord.x, (int)tileCoord.y);
        
        if (tile == null) return;
        if (button == 0)
        {
            switch (action)
            {
                case 0:
                {
                    if (tile.Building != null) return;
                    BuildingType buildingType = BuildingType.HQ;
                    switch (buildIndex)
                    {
                        case 0:
                            buildingType = BuildingType.HQ;
                            break;
                        case 1:
                            buildingType = BuildingType.BARRACKS;
                            break;
                        case 2:
                            buildingType = BuildingType.GOLDMINE;
                            break;
                    }

                    if (world.CanPlaceBuilding(buildingType, tile.X, tile.Y) == true)
                    {
                        Building building = world.CreateFromPrototype(buildingType);
                        world.ReserveBuilding(building, tile);
                        world.TaskSystem.EnqueueTask(
                            () => world.CanPlaceBuilding(building, tile.X, tile.Y)
                                ? new BuildTask() {buildTile = tile, toBuild = building}
                                : null);
                    }

                    break;
                }
                case 1:
                   world.TaskSystem.AddTask(new MoveToTask() {target = tile.TileCoord});
                   break;
               case 2:
                   world.CreateWorker(tile.TileCoord);
                   break;
               case 3:
                   // Gather task
                   if (currentGatherTask == null)
                   {
                       currentGatherTask = new GatherTask();
                       Ymit.UI.DebugFadeLabelMouse("Created gather task!");
                       
                   }
                   else if (currentGatherTask.resourceLocation == null)
                   {
                       if (tile.Building != null)
                       {
                           if (tile.Building.Type == BuildingType.GOLDMINE)
                           {
                               currentGatherTask.resourceLocation = tile.TileCoord - new TileCoord(0,1);
                               Ymit.UI.DebugFadeLabelMouse("Set resource location!");
                           }
                       }
                   }
                   else if (currentGatherTask.resourceLocation != null && currentGatherTask.dropOffLocation == null)
                   {
                       if (tile.Building != null && tile.Building.Type == BuildingType.HQ)
                       {
                           currentGatherTask.dropOffLocation = tile.Building.RootTile.TileCoord - new TileCoord(0,1);
                           Ymit.UI.DebugFadeLabelMouse("Set drop off location!");
                       }
                   }
                   else if (currentGatherTask.resourceLocation != null && currentGatherTask.dropOffLocation != null)
                   {
                       //world.TaskSystem.AddTask(currentGatherTask);
                       world.TaskSystem.EnqueueTask(() => world.TaskSystem.TaskCount == 0 ? currentGatherTask : null);
                       //currentGatherTask = null;
                       Ymit.UI.DebugFadeLabelMouse("Added task!");
                   }
                   break;
               case 4: // destroy
                   Worker worker = world.GetWorkerAt(tile.TileCoord);
                   if (worker != null)
                   {
                       world.DestroyWorker(worker);
                   }

                   Building b = tile.Building;
                   if (b != null)
                   {
                       world.DestroyBuilding(b);
                   }
                   break;
            }
        }

        if (button == 1)
        {
            currentGatherTask = null;
        }
    }
}