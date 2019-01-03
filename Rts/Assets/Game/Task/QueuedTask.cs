using System;

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
}