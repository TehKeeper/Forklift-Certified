using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Logic {
    [RequireComponent(typeof(Rigidbody))]
    public class ForkliftController : MonoBehaviour {
        [SerializeField] private float _moveForce = 100;
        [SerializeField] private float _rotationForce = 100;

        [Space]
        [SerializeField] private ForkController _fork;


        private Rigidbody _rigidBody;

        private GameInputs _input;
        private Transform _transform;
        private Vector2 _controlVectors;
        private bool _engineWorks;

        // Start is called before the first frame update
        void Awake() {
            _rigidBody = GetComponent<Rigidbody>();
            _transform = transform;

            _input = new GameInputs();
            _input.ForkliftInput.EngineToggle.performed += ToggleEngine;
            
            _input.ForkliftInput.Enable();
            
            _fork.Initialize();
        }

        private void ToggleEngine(InputAction.CallbackContext context) {
            _engineWorks = !_engineWorks;
        }

        // Update is called once per frame
        void Update() {
            if (_engineWorks) {
                _controlVectors = _input.ForkliftInput.Movement.ReadValue<Vector2>();

                _rigidBody.AddForce(_transform.forward * _moveForce * _controlVectors.y);
                _rigidBody.AddTorque(_transform.up * _rotationForce * _controlVectors.x);

                _fork.Update(_input);
            }
        }
    }

    [Serializable]
    public class ForkController {
        [SerializeField] private Transform _forkTransform;
        [SerializeField] private float _speed = 1;
        [SerializeField] private float _minHeight;
        [SerializeField] private float _maxHeigh;

        private float _liftValue;
        private bool _initialized;

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
            Debug.Log($"LiftValue: {_liftValue}");

            Vector3 localPos = _forkTransform.localPosition;
            _forkTransform.localPosition =
                new Vector3(localPos.x, Mathf.Lerp(_minHeight, _maxHeigh, _liftValue), localPos.z);
        }
    }
}