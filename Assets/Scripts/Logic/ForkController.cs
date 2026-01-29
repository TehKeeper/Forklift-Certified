using System;
using UnityEngine;

namespace Logic {
    [Serializable]
    public class ForkController  : IObservableCustom<Vector3>{
        [SerializeField] private Transform _forkTransform;
        [SerializeField] private float _speed = 1;
        [SerializeField] private float _minHeight;
        [SerializeField] private float _maxHeigh;

        private float _liftValue;
        private bool _initialized;

        private event Action<Vector3> OnPositionChanged;

        public void Initialize() {
            if(_initialized)
                return;

            if (!_forkTransform) {
                Debug.Log("Missing fork transform!");
                return;
            }

            _initialized = true;
        }

        public void Update(GameInputs input) {
            if(!_initialized)
                return;
            
            _liftValue =
                Mathf.Clamp01(_liftValue +
                              input.ForkliftInput.ForkControl.ReadValue<float>() * Time.deltaTime * _speed);
            

            Vector3 localPos = _forkTransform.localPosition;
            _forkTransform.localPosition =
                new Vector3(localPos.x, Mathf.Lerp(_minHeight, _maxHeigh, _liftValue), localPos.z);
            
            OnPositionChanged?.Invoke(_forkTransform.position);
        }

        public Vector3 Subscribe(IObserver<Vector3> observer) {
            OnPositionChanged += observer.OnNext;
            return _forkTransform.position;
        }
        
        public void UnSubscribe(IObserver<Vector3> observer) {
            OnPositionChanged -= observer.OnNext;
        }
    }
}