using System;
using System.Threading.Tasks;

namespace WarbandServerMonitor.Common
{
    public static class TaskExtensions
    {
        public async static Task<TaskWithTimeoutResult> WithTimeout(this Task task, TimeSpan timeout)
        {
            return await Task.WhenAny(task, Task.Delay(timeout)) == task
                ? new TaskWithTimeoutResult(true)
                : new TaskWithTimeoutResult(false);
        }

        public async static Task<TaskWithTimeoutResult<T>> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
        {
            return await Task.WhenAny(task, Task.Delay(timeout)) != task
                ? new TaskWithTimeoutResult<T>(task.Result)
                : new TaskWithTimeoutResult<T>();
        }
    }

    public class TaskWithTimeoutResult<T> : TaskWithTimeoutResult
    {
        public T Result { get; private set; }

        public TaskWithTimeoutResult() : base(false) { }

        public TaskWithTimeoutResult(T result)
            : base(true)
        {
            Result = result;
        }
    }

    public class TaskWithTimeoutResult
    {
        public bool Success { get; private set; }

        public TaskWithTimeoutResult(bool result)
        {
            Success = result;
        }
    }
}