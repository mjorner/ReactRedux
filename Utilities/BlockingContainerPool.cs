using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace ReactRedux.Utilities {
    internal class BlockingContainerPool<T> {
        private readonly BlockingCollection<T> Pool;
        private readonly ILogger Logger; 
        public BlockingContainerPool(ILogger logger) {
            Pool = new BlockingCollection<T>();
            Logger = logger;
        }

        public virtual void Return(T item) {
            Pool.Add(item);
        }

        public T Rent() {
            T item;
            int count = 0;
            int timeout = 50;
            while (!Pool.TryTake(out item, timeout)) {
                if (count > 10) {
                    Logger.LogWarning($"Waited more than {count*timeout}ms for {typeof(T)}");
                }
                count++;
            }
            return item;
        }
    }
}