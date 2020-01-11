using System.Collections.Concurrent;

namespace GloomyTale.Core
{
    public static class ConcurrentQueueExtensions
    {
        #region Methods

        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            while (queue.Count > 0)
                queue.TryDequeue(out T item);
        }

        #endregion
    }
}