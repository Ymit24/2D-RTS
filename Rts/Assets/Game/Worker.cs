using System;
using System.Linq;
using Game.Pathfinding;
using Game.Task;
using Ymit;

namespace Game
{
	public class Worker
	{
		private enum State
		{
			WaitingForTask,
			ExecutingTask
		}

		public float MoveSpeed = 2;
		public Action<Worker> OnMove;
		
		private TileCoord position;
		private State state;

		private BaseTask currentTask;
		private TaskSystem taskSystem;

		public class PathData
		{
			private Pathfinder pathfinder;
			private Node[] path;
			private int pathProgress = 0;

			public bool Finished
			{
				get
				{
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
				return NodeToTileCoord(path[pathProgress]);
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
				path = pathfinder.Solve(start, end);
			}

			public bool ReadyToStep(TileCoord position)
			{
				return position == GetCurrentTileCoord();
				//return (TileCoord.Distance(position, GetCurrentTileCoord()) < 0.1f);
			}
		}

		private PathData path;

		private float buildTimer = 0;

		private float movePercentage = 0;

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
				// Will eventually store current and next Tile to eliminate calling World.current.*
				return World.current.GetTileAt(position);
			}
		}

		public BaseTask CurrentTask
		{
			get
			{
				return currentTask;
			}
		}

		public Worker(TileCoord position, TaskSystem taskSystem, Pathfinder pathfinder)
		{
			this.position = position;
			this.taskSystem = taskSystem;
			this.path = new PathData(pathfinder);
		}

		public void Tick(float deltaTime)
		{
			switch (state)
			{
				case State.WaitingForTask:
				{
					// Do nothing, idle.
					// Maybe play some idle animation or something.
					currentTask = taskSystem.GetTask();
					if (currentTask != null)
					{
						state = State.ExecutingTask;

						if (currentTask is BaseTask.BuildTask) StartTask_Build(currentTask as BaseTask.BuildTask);
						if (currentTask is BaseTask.MoveToTask) StartTask_MoveTo(currentTask as BaseTask.MoveToTask);
					}
					break;
				}
				case State.ExecutingTask:
				{
					if (currentTask is BaseTask.BuildTask) HandleTask_Build(currentTask as BaseTask.BuildTask, deltaTime);
					if (currentTask is BaseTask.MoveToTask) HandleTask_MoveTo(currentTask as BaseTask.MoveToTask, deltaTime);
					break;
				}	
			}
		}

		protected void StartTask_Build(BaseTask.BuildTask buildTask)
		{
			World.current.ReserveBuilding(buildTask.toBuild, buildTask.buildTile);
			path.Find(position.Rounded(), buildTask.buildTile.TileCoord.Rounded());
		}
		
		protected void StartTask_MoveTo(BaseTask.MoveToTask moveToTask)
		{
			path.Find(position.Rounded(), moveToTask.target.Rounded());
		}
		
		protected void HandleTask_Build(BaseTask.BuildTask buildTask, float deltaTime)
		{
//			if (World.current.CanPlaceBuilding(buildTask.toBuild, buildTask.buildTile.X, buildTask.buildTile.Y) == false)
//			{
//				taskSystem.EnqueueTask( () => World.current.CanPlaceBuilding(buildTask.toBuild, buildTask.buildTile.X, buildTask.buildTile.Y) ? buildTask : null );
//				
//				currentTask = null;
//				state = State.WaitingForTask;
//				buildTimer = 0;
//						
//				path = null;
//				pathProgress = 0;
//				
//				return;
//			}
			TileCoord tileCoord = buildTask.buildTile.TileCoord;
			if (path.Finished == false)
			{
				MoveTo(deltaTime);
			}
			else
			{
				buildTimer += deltaTime;
				if (buildTimer >= buildTask.toBuild.BuildTime)
				{
					// build building.
					if (World.current.PlaceBuilding(buildTask.toBuild, World.current.GetTileAt(tileCoord)))
					{
						path.Complete();
						CompleteTask();
						
						buildTimer = 0;
					}
				}
			}
		}
		
		
		protected void HandleTask_MoveTo(BaseTask.MoveToTask moveToTask, float deltaTime)
		{
			if (path.Finished == false)
			{
				MoveTo(deltaTime);
			}
			else 
			{
				path.Complete();

				CompleteTask();
			}
		}

		protected void CompleteTask()
		{			
			currentTask = null;
			state = State.WaitingForTask;
		}

		private float lerp(float a, float b, float t)
		{
			return a + (b - a) * t;
		}
		
		private void MoveTo(float deltaTime)
		{
			if (path == null) return;
			if (path.Finished) return;
			
			if (path.ReadyToStep(position) == false)
			{
				movePercentage += deltaTime * MoveSpeed;
				TileCoord c = path.GetCurrentTileCoord();
				float x = lerp(position.x, c.x, movePercentage);
				float y = lerp(position.y, c.y, movePercentage);
				position.x = x;
				position.y = y;
//				TileCoord directionToMove = path.GetDirection(position);
//				position += directionToMove * MoveSpeed * deltaTime;
			}
			else
			{
//				position = path.GetCurrentTileCoord();
				path.Step();
				movePercentage = 0;
			}
			if (OnMove != null)
			{
				OnMove(this);
			}
		}
	}
}