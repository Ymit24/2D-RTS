namespace Game.Pathfinding
{
    public class PathData
    {
        private Pathfinder pathfinder;
        private Tile[] path;
        private int pathProgress = 0;

        public bool Finished
        {
            get
            {
                if (path == null) return true;
                return pathProgress == path.Length;
            }
        }

        public PathData(Pathfinder pathfinder)
        {
            this.pathfinder = pathfinder;
        }

        public void Step()
        {
            pathProgress++;
        }

        private TileCoord NodeToTileCoord(Node node)
        {
            return new TileCoord(node.x, node.y);
        }

        public TileCoord GetCurrentTileCoord()
        {
            //return NodeToTileCoord(path[pathProgress]);
            return path[pathProgress].TileCoord;
        }

        public Tile GetCurrentTile()
        {
            if (Finished) return null;
            return path[pathProgress];
        }

        public Tile GetNextTile()
        {
            if (pathProgress + 1 >= path.Length) return null;
            return path[pathProgress + 1];
        }
			
        public TileCoord GetDirection(TileCoord position)
        {
            return TileCoord.Normalize(GetCurrentTileCoord() - position);
        }

        public void Complete()
        {
            path = null;
            pathProgress = 0;
        }

        public void Find(TileCoord start, TileCoord end)
        {
            path = pathfinder.NodePathToTileArray(pathfinder.Solve(start, end));
        }

        public bool ReadyToStep(TileCoord position)
        {
            return position == GetCurrentTileCoord();
            //return (TileCoord.Distance(position, GetCurrentTileCoord()) < 0.1f);
        }
    }

}