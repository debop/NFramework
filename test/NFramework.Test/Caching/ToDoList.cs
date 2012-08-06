using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Caching {
    /// <summary>
    /// Bamboo Prevalence Cache 에 저장될 내용
    /// </summary>
    [Serializable]
    public class ToDoList : MarshalByRefObject {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private ConcurrentDictionary<Guid, TaskCacheItem> _tasks = new ConcurrentDictionary<Guid, TaskCacheItem>();

        public ToDoList() {
            if(IsDebugEnabled)
                log.Debug("Task 저장을 위한 Prevalence Cached 용 ToDoList가 생성되었습니다.");
        }

        /// <summary>
        /// 완료되지 못한 Task들
        /// </summary>
        public IList<TaskCacheItem> PendingTasks {
            get { return _tasks.Values.Where(task => task.IsDone == false).ToList(); }
        }

        public void AddTask(TaskCacheItem taskCacheItem) {
            taskCacheItem.ShouldNotBeNull("task");
            taskCacheItem.Validate();
            taskCacheItem.Id.ShouldNotBeNull("Task.Id");
            taskCacheItem.Id.ShouldNotBeEquals(Guid.Empty, "Task.Id");

            taskCacheItem.CreateDate = DateTime.UtcNow; // PrevalenceEngine.Now;

            var added = _tasks.TryAdd(taskCacheItem.Id, taskCacheItem);

            if(IsDebugEnabled)
                log.Debug("Task를 캐시에 저장을 시도했습니다. 저장여부={0}, Task={1}", added, taskCacheItem);
        }

        public TaskCacheItem GetTask(Guid taskId) {
            TaskCacheItem taskCacheItem;
            if(_tasks.TryGetValue(taskId, out taskCacheItem))
                return taskCacheItem;

            return null;
        }

        public IList<TaskCacheItem> GetAllTasks() {
            return _tasks.Values.ToList();
        }

        /// <summary>
        /// 지정한 Task를 완료 시켜버린다.
        /// </summary>
        /// <param name="taskId"></param>
        public void MarkTaskAsDone(Guid taskId) {
            TaskCacheItem taskCacheItem;
            if(_tasks.TryGetValue(taskId, out taskCacheItem)) {
                taskCacheItem.IsDone = true;
            }
            else {
                if(log.IsWarnEnabled)
                    log.Warn("해당 Task가 캐시에 없습니다. taskId=" + taskId);
            }
        }

        public bool RemoveTask(Guid taskId) {
            if(IsDebugEnabled)
                log.Debug("Task[{0}] 를 캐시에서 삭제합니다.", taskId);

            TaskCacheItem removedTaskCacheItem;
            return _tasks.TryRemove(taskId, out removedTaskCacheItem);
        }

        public void Clear() {
            _tasks.Clear();
        }
    }
}