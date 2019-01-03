using Game.Pathfinding;
using Game.Task;

namespace Game
{
    public class Combat : Unit
    {
        public Combat(TileCoord position, TaskSystem globalTaskSystem, Pathfinder pathfinder)
        : base(position, globalTaskSystem, pathfinder)
        {
        }
    }
}