using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    static public BuildingController current;
    public Sprite BuildingSprite, HQSprite, BXSprite;
    
    protected class BuildingVisual
    {
        // Probably want a building ID system instead of a direct reference, this is only used for finding the visual out of a list
        private Building building;
        private GameObject[] visuals;
        private int sizeInTiles;
        private int visualIndex;

        public Building Building
        {
            get
            {
                return building;
            }
        }

        public GameObject[] Visuals
        {
            get
            {
                return visuals;
            }
        }
        
        // Probably should take some kind of building ID and the building's size instead
        public BuildingVisual(Building building)
        {
            this.building = building;
            sizeInTiles = building.Size;
            visuals = new GameObject[sizeInTiles * sizeInTiles];
        }

        public void AddVisual(GameObject gameObject)
        {
            if (visualIndex == sizeInTiles*sizeInTiles) return;
            visuals[visualIndex++] = gameObject;
        }
        // Don't need a remove function because the entire class will be 
        // destroyed when the gameobjects are removed. Maybe instead just
        // deactive the visuals or use a pooling system to save performance.
    }

    private List<BuildingVisual> buildingVisuals;
    private GameObject BuildingHolder;

    protected void Awake()
    {
        if (current == null) current = this;
    }
    
    public void Setup()
    {
        buildingVisuals = new List<BuildingVisual>();
        BuildingHolder = new GameObject("Buildings");
        Game.World.current.OnBuildingDestroyed += OnBuildingDestroyed;
        Game.World.current.OnBuildingBuilt += OnBuildingBuilt;
    }

    protected void CreateBuilding(Building building)
    {
        BuildingVisual visual = new BuildingVisual(building);
        Tile tile = building.RootTile;
        for (int i = 0; i < building.Size; i++)
        {
            for (int j = 0; j < building.Size; j++)
            {
                GameObject buildingGO = new GameObject("Building_" + building.Type, typeof(SpriteRenderer));
                buildingGO.transform.position = new Vector2((i + tile.X) * WorldController.TileSize, (j + tile.Y) * WorldController.TileSize);
                buildingGO.transform.localScale = new Vector2(WorldController.TileSize, WorldController.TileSize);
                buildingGO.transform.SetParent(BuildingHolder.transform);

                SpriteRenderer sr = buildingGO.GetComponent<SpriteRenderer>();
                //sr.sprite = BuildingSprite;
                if (building.Type == BuildingType.HQ)       sr.sprite = HQSprite;
                if (building.Type == BuildingType.BARRACKS) sr.sprite = BXSprite;
//                if (building.Type == BuildingType.HQ)       sr.color = new Color(0.07f, 0f, 0.67f);
//                if (building.Type == BuildingType.BARRACKS) sr.color = new Color(0f, 0.67f, 0.13f);

                visual.AddVisual(buildingGO);
            }
        }
        buildingVisuals.Add(visual);
    }
    
    protected void DestroyBuilding(Building building)
    {
        for (int i = 0; i < buildingVisuals.Count; i++)
        {
            if (buildingVisuals[i].Building == building)
            {
                for (int j = 0; j < buildingVisuals[i].Visuals.Length; j++)
                {
                    Destroy(buildingVisuals[i].Visuals[j]);
                }
                buildingVisuals.RemoveAt(i);
                return;
            }
        }
    }

    private void OnBuildingBuilt(Building building)
    {
        CreateBuilding(building);
    }

    private void OnBuildingDestroyed(Building building)
    {
        DestroyBuilding(building);
    }
}
