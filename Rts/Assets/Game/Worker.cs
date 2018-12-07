using System;
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

		public float MoveSpeed = 4;
		public Action<Worker> OnMove;
		
		private TileCoord position;
		private State state;

		private BaseTask currentTask;
		private TaskSystem taskSystem;

		private float buildTimer = 0;

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

		public Worker(TileCoord position, TaskSystem taskSystem)
		{
			this.position = position;
			this.taskSystem = taskSystem;
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
					}
					break;
				}
				case State.ExecutingTask:
				{
					if (currentTask is BaseTask.BuildTask) HandleTask_Build(currentTask as BaseTask.BuildTask, deltaTime);
					break;
				}	
			}
		}

		protected void HandleTask_Build(BaseTask.BuildTask buildTask, float deltaTime)
		{
			// Don't hardcode 1, worker should have some kind of range/reach value
			TileCoord tileCoord = buildTask.buildTile.TileCoord;
			if (TileCoord.Distance(position, tileCoord) > 1)
			{
				// Move to tile
				TileCoord directionToMove = TileCoord.Normalize(tileCoord - position);
				// Delta time should be updated once per frame and stored in some util class
				// Move code should move between two tiles, not in a direction
				position += directionToMove * MoveSpeed * deltaTime;
				if (OnMove != null)
				{
					OnMove(this);
				}
			}
			else
			{
				// wait for amount of time,
				buildTimer += deltaTime;
				if (buildTimer >= buildTask.toBuild.BuildTime)
				{
					// build building.
					if (World.current.PlaceBuilding(buildTask.toBuild, World.current.GetTileAt(tileCoord)))
					{
						currentTask = null;
						state = State.WaitingForTask;
						buildTimer = 0;	
					}
				}
			}
		}
	}
}