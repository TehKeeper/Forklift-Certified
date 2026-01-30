using System;
using UnityEngine;

namespace Logic {
    public interface IObservableCustom<T>{
        /// <summary>Subscribes observer to a value change </summary>
        /// <returns>Current observable value</returns>
        T Subscribe(IObserver<T> observer);
        /// <summary>Unsubscribes observer to a value change </summary>
        void UnSubscribe(IObserver<T> observer);
    }
}