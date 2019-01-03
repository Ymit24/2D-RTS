namespace Game.Task
{
    public class BaseTask
    {        
    }
    
    public class BuildTask : BaseTask
    {
        public Tile buildTile;
        public Building toBuild;
    }

    public class MoveToTask : BaseTask
    {
        public TileCoord target;
    }

    public class GatherTask : BaseTask
    {
        public TileCoord resourceLocation;
        public TileCoord dropOffLocation;
    }
}