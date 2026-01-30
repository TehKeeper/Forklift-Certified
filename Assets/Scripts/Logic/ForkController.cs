using System;
using UnityEngine;

namespace Logic {
    /// <summary>Controls fork of forklift </summary>
    [Serializable]
    public class ForkController : IObservableCustom<Vector3>, IDisposable {
        [SerializeField] private Transform _forkTransform;
        [SerializeField] private float _speed = 1;
        [SerializeField] private float _minHeight;
        [SerializeField] private float _maxHeigh;

        private float _liftValue;
        private bool _initialized;

        private Vector3 _defaultPosition;
        private event Action<Vector3> OnPositionChanged;

        public void Initialize() {
            if (_initialized)
                return;

            if (!_forkTransform) {
                Debug.Log("Missing fork transform!");
                return;
            }

            _defaultPosition = _forkTransform.localPosition;
            _defaultPosition = new Vector3(_defaultPosition.x, _minHeight, _defaultPosition.z);
            _initialized = true;
        }

        public void Update(GameInputs input) {
            if (!_initialized)
                return;

            _liftValue =
                Mathf.Clamp01(_liftValue +
                              input.ForkliftInput.ForkControl.ReadValue<float>() * Time.deltaTime * _speed);

            _forkTransform.localPosition =
                new Vector3(_defaultPosition.x, Mathf.Lerp(_minHeight, _maxHeigh, _liftValue), _defaultPosition.z);

            OnPositionChanged?.Invoke(_forkTransform.position);
        }

        public Vector3 Subscribe(IObserver<Vector3> observer) {
            OnPositionChanged += observer.OnNext;
            return _forkTransform.position;
        }

        public void UnSubscribe(IObserver<Vector3> observer) {
            OnPositionChanged -= observer.OnNext;
        }

        public void Dispose() {
            OnPositionChanged = null;
        }
    }
}