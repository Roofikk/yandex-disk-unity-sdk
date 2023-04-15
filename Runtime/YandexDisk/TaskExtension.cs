<<<<<<< HEAD
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
=======
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
>>>>>>> 54935b7afcc8c9ace832f3baf9523de90599e286
}