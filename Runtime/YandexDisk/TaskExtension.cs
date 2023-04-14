using System;
using System.Threading.Tasks;

namespace YandexDiskSDK
{
    public static class TaskExtension
    {
        public static Task RunAsync<TResult>(this Task<TResult> task, Action<TResult> continuation)
        {
            return task.ContinueWith((t) => continuation?.Invoke(t.Result));
        }
    }
}