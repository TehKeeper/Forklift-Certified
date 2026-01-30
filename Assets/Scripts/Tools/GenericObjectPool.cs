using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Tools {
    public class GenericObjectPool<T> where T : IActivatable {
        private readonly Func<T> _maker;
        private readonly Queue<T> _queue = new();
        private readonly Func<UniTask<T>> _asyncMaker;

        
        public GenericObjectPool(Func<T> maker) {
            _maker = maker;
        }

        #region UNSAFE

        public GenericObjectPool(Func<UniTask<T>> asyncMaker) {
            _asyncMaker = asyncMaker;
        }
        
        public async UniTask<T> TryTakeAsync() {
            if (_queue.Count > 0) {
                return _queue.Dequeue();
            }

            return _maker.Invoke();
        }

        #endregion
        
        public T TryTake() {
            if (_queue.Count > 0) {
                T tryTake = _queue.Dequeue();
                tryTake.Activate();
                return tryTake;
            }

            return _maker.Invoke();
        }

        public void Release(T go) {
            go.Deactivate();
            _queue.Enqueue(go);
        }
    }
}