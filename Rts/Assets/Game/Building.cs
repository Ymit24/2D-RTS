namespace Game
{
    public enum BuildingType { HQ, BARRACKS }
    public class Building
    {
        private Tile[] tiles;
        private BuildingType type;

        private int size;
        private float buildTime;
        
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

        public Building(BuildingType type, int size, float buildTime = 1)
        {
            this.type = type;
            this.size = size;
            this.buildTime = buildTime;
        }
        
        public void PlaceOnTiles(Tile[] tiles)
        {
            this.tiles = tiles;
        }
    }
}