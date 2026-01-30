using System;
using System.Collections.Generic;
using Logic.Ui;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Logic {
    /// <summary>Main forklift movement controller</summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ForkliftController : MonoBehaviour, IObservableCustom<Vector3> {
        [SerializeField] private float _moveForce = 100;
        [SerializeField] private float _rotationForce = 100;
        [SerializeField] private AudioSource _engine;

        [Space]
        [SerializeField] private ForkController _fork;

        [Space]
        [SerializeField] private ForkliftFuelTank _fuelTank;

        [SerializeField] private UnityEvent<bool> _engineSwitch;


        private Rigidbody _rigidBody;

        private GameInputs _input;
        private Transform _transform;
        private Vector2 _controlVectors;
        private bool _engineWorks;
        private float _fuelMod = 1;

        private List<IDisposable> _disposers = new List<IDisposable>();

        void Awake() {
            _rigidBody = GetComponent<Rigidbody>();
            _transform = transform;

            SplashScreenLogic.OnSceneReady += Initialize;

            _fuelTank.StateMachine.OnStateChanged += ChangeBehaviour;

            _disposers.Add(_fuelTank);
            _disposers.Add(_fork);
        }

        private void ChangeBehaviour(FuelTankState obj) {
            switch (obj) {
                case FuelTankState.Full:
                    _fuelMod = 1;
                    _engine.pitch = 1f;
                    break;
                case FuelTankState.HalfEmpty:
                    _fuelMod = 0.75f;
                    _engine.pitch = 0.8f;
                    break;
                case FuelTankState.Empty:
                    _fuelMod = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
            }
            
        }

        private void Initialize() {
            _input = new GameInputs();
            _input.ForkliftInput.EngineToggle.performed += ToggleEngine;

            _input.ForkliftInput.Enable();

            _fork.Initialize();

            SplashScreenLogic.OnSceneReady -= Initialize;
        }

        private void ToggleEngine(InputAction.CallbackContext context) {
            _engineWorks = !_engineWorks;
            _engineSwitch?.Invoke(_engineWorks);
            if (_engine)
                if (_engineWorks)
                    _engine.Play();
                else
                    _engine.Stop();
        }

        void Update() {
            if (_engineWorks) {
                _controlVectors = _input.ForkliftInput.Movement.ReadValue<Vector2>();

                _rigidBody.AddForce(_transform.forward * _moveForce * _controlVectors.y * _fuelMod);
                _rigidBody.AddTorque(_transform.up * _rotationForce * _controlVectors.x * _fuelMod);

                _fork.Update(_input);

                _fuelTank.ConsumeFuel();
            }
        }

        public Vector3 Subscribe(IObserver<Vector3> observer) {
            return _fork.Subscribe(observer);
        }

        public void UnSubscribe(IObserver<Vector3> observer) {
            _fork.UnSubscribe(observer);
        }

        private void OnDestroy() {
            SplashScreenLogic.OnSceneReady -= Initialize;

            _fuelTank.StateMachine.OnStateChanged -= ChangeBehaviour;

            foreach (IDisposable disposer in _disposers) {
                disposer.Dispose();
            }
        }
    }
}