using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Logic {
    [RequireComponent(typeof(Rigidbody))]
    public class ForkliftController : MonoBehaviour, IObservableCustom<Vector3> {
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

            SplashScreenLogic.OnSceneReady+=Initialize;
            
        }

        private void Initialize() {
            _input = new GameInputs();
            _input.ForkliftInput.EngineToggle.performed += ToggleEngine;

            _input.ForkliftInput.Enable();

            _fork.Initialize();
            
            SplashScreenLogic.OnSceneReady-=Initialize;
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

        public Vector3 Subscribe(IObserver<Vector3> observer) {
            return _fork.Subscribe(observer);
        }

        public void UnSubscribe(IObserver<Vector3> observer) {
            _fork.UnSubscribe(observer);
        }
    }
}