using System.Collections.Concurrent;

namespace ReactRedux.Utilities {
    internal class BlockingContainerPool<T> {
        private readonly BlockingCollection<T> Pool;
        public BlockingContainerPool() {
            Pool = new BlockingCollection<T>();
        }

        public virtual void Return(T item) {
            Pool.Add(item);
        }

        public T Rent() {
            T item;
            while (!Pool.TryTake(out item, 50)) {
                //This could be logged to signal that our sizing is too small?
                //If the current thread has e.g. waited more than 2 times!?
            }
            return item;
        }
    }
}