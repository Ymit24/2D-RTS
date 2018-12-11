using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Task
{
    public class QueuedTask
    {
        private Func<BaseTask> tryGetTaskFunc;

        public QueuedTask(Func<BaseTask> tryGetTaskFunc)
        {
            this.tryGetTaskFunc = tryGetTaskFunc;
        }

        public BaseTask TryDequeueTask()
        {
            return tryGetTaskFunc();
        }
    }
        
    public class BaseTask
    {
        public class BuildTask : BaseTask
        {
            public Tile buildTile;
            public Building toBuild;
        }

        public class MoveToTask : BaseTask
        {
            public TileCoord target;
        }
    }
    
    public class TaskSystem
    {
        private List<BaseTask> taskList;
        private List<QueuedTask> queuedTaskList;

        public TaskSystem()
        {
            taskList = new List<BaseTask>();
            queuedTaskList = new List<QueuedTask>();
        }

        public void AddTask(BaseTask baseTask)
        {
            taskList.Add(baseTask);
        }

        public void EnqueueTask(QueuedTask queuedTask)
        {
            queuedTaskList.Add(queuedTask);
        }

        public void EnqueueTask(Func<BaseTask> tryGetTaskFunc)
        {
            queuedTaskList.Add(new QueuedTask(tryGetTaskFunc));
        }
        
        public BaseTask GetTask()
        {
            if (taskList.Count > 0)
            {
                BaseTask baseTask = taskList[0];
                taskList.RemoveAt(0);
                return baseTask;
            }
            else
            {
                for (int i = 0; i < queuedTaskList.Count; i++)
                {
                    QueuedTask queuedTask = queuedTaskList[i];
                    BaseTask baseTask = queuedTask.TryDequeueTask();
                    if (baseTask != null)
                    {
                        AddTask(baseTask);
                        queuedTaskList.RemoveAt(i);
                        i--;
                    }
                }
            }

            return null;
        }
    }
}