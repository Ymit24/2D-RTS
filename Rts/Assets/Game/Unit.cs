using System;
using System.Collections;
using Game.Task;
using Game.Pathfinding;
using Game.Util;
namespace Game
{
    public class Unit
    {
        protected TileCoord position;
        public float MoveSpeed = 2;
        public Action<Unit> OnMove;
        protected State state;
		
        protected Tile lastTile, tileMovingTo;
		
        protected TaskSystem localTaskSystem;
        protected BaseTask currentTask;
        protected TaskSystem globalTaskSystem;
		
        protected PathData path;
        protected bool needRepath;
		
        protected float movePercentage = 0;
        public TileCoord Position
        {
            get
            {
                return position;
            }
        }

        public Tile Tile
        {
            get
            {
                if (path != null && path.Finished == false) return path.GetCurrentTile();
                if (lastTile != null) return lastTile;
                Ymit.UI.DebugFadeLabelMouse("Failed to retrieve tile!");
                return null;
            }
        }

        public BaseTask CurrentTask
        {
            get
            {
                return currentTask;
            }
        }

        public TaskSystem LocalTaskSystem
        {
            get { return localTaskSystem; }
        }

        public Unit(TileCoord position, TaskSystem globalTaskSystem, Pathfinder pathfinder)
        {
            this.position = position;
            this.globalTaskSystem = globalTaskSystem;
            this.path = new PathData(pathfinder);

            pathfinder.Repath += OnRepath;
            this.lastTile = World.current.GetTileAt(position.Rounded());
            this.tileMovingTo = lastTile;
			
            localTaskSystem = new TaskSystem();
        }
        
        protected virtual void OnRepath()
        {
            path.Complete();
            needRepath = true;
        }

        protected virtual void DoFindPath()
        {
            if (currentTask is MoveToTask) path.Find(position.Rounded(), (currentTask as MoveToTask).target.Rounded());
            tileMovingTo = path.GetCurrentTile();
        }

        protected void StartTask_MoveTo(MoveToTask moveToTask)
        {
            DoFindPath();
        }
        
        protected void HandleTask_MoveTo(MoveToTask moveToTask, float deltaTime)
        {
            if (MoveTo(deltaTime) && position == moveToTask.target)
            {
                path.Complete();

                CompleteTask();
            }
        }
        
        protected void CompleteTask()
        {
            currentTask = null;
            movePercentage = 0;
            state = State.WaitingForTask;
        }
        
        protected virtual bool MoveTo(float deltaTime)
        {
            if (tileMovingTo != null)
            {
                TileCoord c = tileMovingTo.TileCoord;

                movePercentage += deltaTime * MoveSpeed;
                movePercentage = Utils.clamp(movePercentage, 0, 1);

                float x = Utils.lerp(position.x, c.x, movePercentage);
                float y = Utils.lerp(position.y, c.y, movePercentage);
                position.x = x;
                position.y = y;
            }

            bool finishedMove = movePercentage >= 1;
            if (finishedMove)
            {
                movePercentage = 0;
				
                lastTile = tileMovingTo;
                if (needRepath)
                {
                    DoFindPath();
                    needRepath = false;
                }
                else
                {
                    path.Step();
                    tileMovingTo = path.GetCurrentTile();
                }

            }
            if (OnMove != null)
            {
                OnMove(this);
            }

            return finishedMove;
        }
    }
}