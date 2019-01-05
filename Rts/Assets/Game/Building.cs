namespace Game
{
    public enum BuildingType { HQ, BARRACKS, GOLDMINE }
    public class Building
    {
        protected static int CURRENTBUILDINGID = 0;
        private Tile[] tiles;
        private BuildingType type;

        private int size;
        private float buildTime;
        private int buildingID;
        
        public Tile RootTile
        {
            get
            {
                if (tiles.Length > 0)
                    return tiles[0];
                else
                    return null;
            }
        }

        public int Size
        {
            get
            {
                return size;
            }
        }

        public BuildingType Type
        {
            get
            {
                return type;
            }
        }

        public float BuildTime
        {
            get
            {
                return buildTime;
            }
        }

        public int BuildingId
        {
            get
            {
                return buildingID;
            }
        }

        // maybe make this a static 'CreateBuildingFromPrototype' function that calls private constructor
        public Building(Building copy)
        {
            this.type = copy.type;
            this.size = copy.size;
            this.buildTime = copy.buildTime;
            
            this.buildingID = CURRENTBUILDINGID++;
        }
        
        // maybe make this a static 'CreateBuildingPrototype' function that calls private constructor
        public Building(BuildingType type, int size, float buildTime = 1)
        {
            this.type = type;
            this.size = size;
            this.buildTime = buildTime;

            this.buildingID = CURRENTBUILDINGID++; // probably don't need this for prototypes
        }
        
        public void PlaceOnTiles(Tile[] tiles)
        {
            this.tiles = tiles;
        }
    }
}