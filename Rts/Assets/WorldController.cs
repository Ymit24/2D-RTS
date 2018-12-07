using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Task;
using UnityEditorInternal;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    static public WorldController current;

    protected void Awake()
    {
        if (current == null) current = this;
    }
    
    private Game.World world;
    // Probably want a sprite manager, or specific TileController, BuildingControllers to manage these
    public Sprite TileSprite;

    [SerializeField] private int width, height;
    
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
                sr.color = new Color(0.08f, 0.5f, 0.18f);
            }
        }
        BuildingController.current.Setup();
        WorkerController.current.Setup();
        // Probably want this to belong to a BuildingController
        
        // That function likely will be removed so this is okay temporary code.
        MouseController.Create();
        MouseController.OnClick += OnClick;
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
        world.Tick(Time.deltaTime);
    }
    
    // Temporary debug code, okay for now.
    protected void OnClick(Vector2 position, int button, bool down)
    {
        if (down == false) return;
        
        TileCoord tileCoord = ScreenToTileCoords(position);
        Tile tile = world.GetTileAt((int)tileCoord.x, (int)tileCoord.y);
        
        if (tile == null) return;
        if (button == 0)
        {
            if (tile.Building != null) return;
            int buildingTypeRandom = Random.Range(0, 2);
            Building building = world.BuildPrototype(buildingTypeRandom == 0 ? BuildingType.HQ : BuildingType.BARRACKS,
                buildingTypeRandom == 0 ? 3 : 2, 2);

            if (building == null) return;

            world.TaskSystem.AddTask(new BaseTask.BuildTask() {buildTile = tile, toBuild = building});
            
//            if (world.PlaceBuilding(building, tile) == false)
//            {
//                return;
//            }

            // Create blueprint building.
            //CreateBuilding(building, tile);
        }

        if (button == 1)
        {
            Building building = tile.Building;
            if (building != null)
            {
                world.DestroyBuilding(building);
            }

            Worker worker = world.GetWorkerAt(tile.TileCoord);
            if (worker != null)
            {
                world.DestroyWorker(worker);
            }
        }

        if (button == 2)
        {
            world.CreateWorker(tile.TileCoord);
        }
    }
}