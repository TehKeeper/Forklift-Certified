using System;
using UnityEngine;

namespace Logic {
    public interface IObservableCustom<T>{
        T Subscribe(IObserver<Vector3> observer);
        void UnSubscribe(IObserver<Vector3> observer);
    }
}