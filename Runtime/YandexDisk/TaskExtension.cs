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

        public static void RunAsyncOnMainThread<TResult>(this Task<TResult> task, Action<TResult> continuation)
        {
            task.ConfigureAwait(true).GetAwaiter().OnCompleted(() =>
            {
                continuation?.Invoke(task.Result);
            });
        }
    }
}