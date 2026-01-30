using System;
using UnityEngine;

namespace Tools {
    public class GenericEnumStateMachine<T> where T : Enum {
        public T CurrentState { get; private set; }
        public event Action<T> OnStateChanged;

        private bool _logState;

        public bool SetLogState(bool b) => _logState = b;

        public GenericEnumStateMachine() {
            CurrentState = default;
        }

        public void SetState(T newState) {
            if (CurrentState.Equals(newState))
                return;
            CurrentState = newState;
            if (_logState) {
                Debug.Log($"{typeof(T)} state machine new state: {CurrentState}");
            }
            OnStateChanged?.Invoke(newState);
        }
    }
}