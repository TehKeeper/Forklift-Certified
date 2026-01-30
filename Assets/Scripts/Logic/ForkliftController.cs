using System;
using Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Logic {
    [RequireComponent(typeof(Rigidbody))]
    public class ForkliftController : MonoBehaviour, IObservableCustom<Vector3> {
        [SerializeField] private float _moveForce = 100;
        [SerializeField] private float _rotationForce = 100;

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

        // Start is called before the first frame update
        void Awake() {
            _rigidBody = GetComponent<Rigidbody>();
            _transform = transform;

            SplashScreenLogic.OnSceneReady += Initialize;

            _fuelTank.StateMachine.OnStateChanged += ChangeBehaviour;
        }

        private void ChangeBehaviour(FuelTankState obj) {
            switch (obj) {
                case FuelTankState.Full:
                    _fuelMod = 1;
                    break;
                case FuelTankState.HalfEmpty:
                    _fuelMod = 0.75f;
                    break;
                case FuelTankState.Empty:
                    _fuelMod = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
            }
            Debug.Log($"Fuel Mod: {_fuelMod}");
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
        }
        
        void Update() {
            if (_engineWorks) {
                _controlVectors = _input.ForkliftInput.Movement.ReadValue<Vector2>();

                _rigidBody.AddForce(_transform.forward * _moveForce * _controlVectors.y  * _fuelMod);
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
        
    }

    public enum FuelTankState {
        Full = 0,
        HalfEmpty = 1,
        Empty = 2
    }

    [Serializable]
    public class ForkliftFuelTank {
        public UnityEvent<float> _onValueChanged;
        public GenericEnumStateMachine<FuelTankState> StateMachine = new();

        [Range(0, 1)]
        [SerializeField] private float _fill = 1;

        [SerializeField] private float _consumptionRate = 0.02f;

        public void ConsumeFuel() {
            if (_fill > 0) {
                _fill = Mathf.Clamp01(_fill - Time.deltaTime * _consumptionRate);
            }

            _onValueChanged?.Invoke(_fill);
            StateMachine.SetState(StateOnFuel());
            
        }

        public FuelTankState StateOnFuel() {
            if (_fill > 0.5f)
                return FuelTankState.Full;

            if (_fill > 0)
                return FuelTankState.HalfEmpty;

            return FuelTankState.Empty;
        }
    }
}