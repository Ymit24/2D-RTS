using System;
using System.Linq;
using System.Collections.Generic;
using Game.Pathfinding;
using Game.Task;
//using Ymit;

namespace Game
{
	public enum State
	{
		WaitingForTask,
		ExecutingTask
	}
	
	public class Worker : Unit
	{
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
		
		public Worker(TileCoord position, TaskSystem globalTaskSystem, Pathfinder pathfinder)
		: base(position, globalTaskSystem, pathfinder)
		{
			this.stateData = new StateData();
		}

		public void Tick(float deltaTime)
		{
			switch (state)
			{
				case State.WaitingForTask:
				{
					BaseTask task = localTaskSystem.GetTask();
					if (task != null)
					{
						currentTask = task;
					}
					else
					{
						currentTask = globalTaskSystem.GetTask();
					}

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

		protected override void DoFindPath()
		{
			if (currentTask is BuildTask) path.Find(position.Rounded(), (currentTask as BuildTask).buildTile.TileCoord.Rounded());
			// this is a task all units can do, so it is in the unit class
			//if (currentTask is MoveToTask) path.Find(position.Rounded(), (currentTask as MoveToTask).target.Rounded());
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

			// This catches all global tasks, e.g. moving.
			// It also sets tileMovingTo = path.GetCurrentTile();
			base.DoFindPath();
		}

		protected void StartTask_Build(BuildTask buildTask)
		{
			DoFindPath();
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
			//Ymit.UI.DebugFadeLabelMouse("Updated gather state! " + newState.ToString());
			stateData.gatherState = newState;
			switch (stateData.gatherState)
			{
				case GatherState.MovingToResourceLocation:
				{
					DoFindPath();
					break;
				}
				case GatherState.GatheringResource:
				{
					stateData.gatherTimer = 0;
					break;
				}
				case GatherState.MovingToDropoffLocation:
				{
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

		
	}
}