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

		private Tile lastTile, tileMovingTo;
		
		private BaseTask currentTask;
		private TaskSystem taskSystem;

		private PathData path;
		private bool needRepath;

		protected enum GatherState
		{
			MovingToResourceLocation,
			GatheringResource,
			MovingToDropoffLocation,
			DroppingResource
		}
		
		protected struct StateData
		{
			public float buildTimer;
			public GatherState gatherState;
			public float gatherTimer;
			public float dropingTimer;
		}
		private StateData stateData;

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

		public Worker(TileCoord position, TaskSystem taskSystem, Pathfinder pathfinder)
		{
			this.position = position;
			this.taskSystem = taskSystem;
			this.path = new PathData(pathfinder);

			pathfinder.Repath += OnRepath;
			this.lastTile = World.current.GetTileAt(position.Rounded());
			this.tileMovingTo = lastTile;
			
			this.stateData = new StateData();
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

						if (currentTask is BuildTask) StartTask_Build(currentTask as BuildTask);
						if (currentTask is MoveToTask) StartTask_MoveTo(currentTask as MoveToTask);
						if (currentTask is GatherTask) StartTask_Gather(currentTask as GatherTask);
					}
					break;
				}
				case State.ExecutingTask:
				{
					if (currentTask is BuildTask) HandleTask_Build(currentTask as BuildTask, deltaTime);
					if (currentTask is MoveToTask) HandleTask_MoveTo(currentTask as MoveToTask, deltaTime);
					if (currentTask is GatherTask) HandleTask_Gather(currentTask as GatherTask, deltaTime);
					break;
				}	
			}
		}

		protected void OnRepath()
		{
			path.Complete();
			needRepath = true;
		}
		
		void DoFindPath()
		{
			if (currentTask is BuildTask) path.Find(position.Rounded(), (currentTask as BuildTask).buildTile.TileCoord.Rounded());
			if (currentTask is MoveToTask) path.Find(position.Rounded(), (currentTask as MoveToTask).target.Rounded());
			if (currentTask is GatherTask)
			{
				switch (stateData.gatherState)
				{
					case GatherState.MovingToResourceLocation:
					{
						path.Find(position.Rounded(), (currentTask as GatherTask).resourceLocation.Rounded());
						break;
					}
					case GatherState.MovingToDropoffLocation:
					{
						path.Find(position.Rounded(), (currentTask as GatherTask).dropOffLocation.Rounded());
						break;
					}
				}
			}

			tileMovingTo = path.GetCurrentTile();
		}

		protected void StartTask_Build(BuildTask buildTask)
		{
			//World.current.ReserveBuilding(buildTask.toBuild, buildTask.buildTile);
			DoFindPath();
		}
		
		protected void StartTask_MoveTo(MoveToTask moveToTask)
		{
			DoFindPath();
			//path.Find(position.Rounded(), moveToTask.target.Rounded());
		}

		protected void StartTask_Gather(GatherTask gatherTask)
		{
			SetGatherState(GatherState.MovingToResourceLocation);
		}
		
		protected void HandleTask_Build(BuildTask buildTask, float deltaTime)
		{
			TileCoord tileCoord = buildTask.buildTile.TileCoord;
			MoveTo(deltaTime);
			if (position == tileCoord)
			{
				stateData.buildTimer += deltaTime;
				if (stateData.buildTimer >= buildTask.toBuild.BuildTime)
				{
					// build building.
					if (World.current.PlaceBuilding(buildTask.toBuild, World.current.GetTileAt(tileCoord)))
					{
						path.Complete();
						CompleteTask();
						
						stateData.buildTimer = 0;
					}
				}
			}
		}
		
		
		protected void HandleTask_MoveTo(MoveToTask moveToTask, float deltaTime)
		{
			if (MoveTo(deltaTime) && position == moveToTask.target)
			{
				path.Complete();

				CompleteTask();
			}
		}

		protected void HandleTask_Gather(GatherTask gatherTask, float deltaTime)
		{
			switch (stateData.gatherState)
			{
				case GatherState.MovingToResourceLocation:
				{
					if (MoveTo(deltaTime) && position == gatherTask.resourceLocation)
					{
						path.Complete();
						SetGatherState(GatherState.GatheringResource);
					}
					break;
				}
				case GatherState.GatheringResource:
				{
					stateData.gatherTimer += deltaTime;
					if (stateData.gatherTimer >= 2) // don't hardcode gather time!
					{
						SetGatherState(GatherState.MovingToDropoffLocation);
					}
					break;
				}
				case GatherState.MovingToDropoffLocation:
				{
					if (MoveTo(deltaTime) && position == gatherTask.dropOffLocation)
					{
						path.Complete();
						SetGatherState(GatherState.DroppingResource);
					}
					break;
				}
				case GatherState.DroppingResource:
				{
					stateData.dropingTimer += deltaTime;
					if (stateData.dropingTimer >= 2) // don't hardcode drop time!
					{
						CompleteTask();
					}
					break;
				}
			}
		}

		protected void SetGatherState(GatherState newState)
		{
			if ((currentTask is GatherTask) == false) return;
			GatherTask gatherTask = currentTask as GatherTask;
			Ymit.UI.DebugFadeLabelMouse("Updated gather state! " + newState.ToString());
			stateData.gatherState = newState;
			switch (stateData.gatherState)
			{
				case GatherState.MovingToResourceLocation:
				{
					//path.Find(position.Rounded(), gatherTask.resourceLocation.Rounded());
					DoFindPath();
					break;
				}
				case GatherState.GatheringResource:
				{
					// stateData.GatheredResources = 0;
					stateData.gatherTimer = 0;
					break;
				}
				case GatherState.MovingToDropoffLocation:
				{
					//path.Find(position.Rounded(), gatherTask.dropOffLocation.Rounded());
					DoFindPath();
					break;
				}
				case GatherState.DroppingResource:
				{
					stateData.dropingTimer = 0;
					break;
				}
			}
		}

		protected void CompleteTask()
		{
			currentTask = null;
			movePercentage = 0;
			state = State.WaitingForTask;
			Ymit.UI.DebugFadeLabelMouse("Completed Task!");
		}

		private float lerp(float a, float b, float t)
		{
			return a + (b - a) * t;
		}

		private float clamp(float x, float min, float max)
		{
			if (x < min)
				return min;
			else if (x > max)
				return max;
			else
				return x;
		}
		
		private bool MoveTo(float deltaTime)
		{
//			if (path == null) return;
//			if (path.Finished) return;
			
			//if (path.ReadyToStep(position) == false)

			if (tileMovingTo != null)
			{
				TileCoord c = tileMovingTo.TileCoord;

				movePercentage += deltaTime * MoveSpeed;
				movePercentage = clamp(movePercentage, 0, 1);

				float x = lerp(position.x, c.x, movePercentage);
				float y = lerp(position.y, c.y, movePercentage);
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