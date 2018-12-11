using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Task
{
    
    
    public class TaskSystem
    {
        private List<BaseTask> taskList;
        private List<QueuedTask> queuedTaskList;

        public int TaskCount
        {
            get { return taskList.Count; }
        }
        
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