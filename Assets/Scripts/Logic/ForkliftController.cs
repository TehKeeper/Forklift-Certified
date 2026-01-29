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

        // Start is called before the first frame update
        void Awake() {
            _rigidBody = GetComponent<Rigidbody>();
            _transform = transform;

            _input = new GameInputs();
            _input.ForkliftInput.Movement.performed += MovementControls;


            _input.ForkliftInput.Enable();
            Debug.Log("Forklift is ON");
            //_inputControl =  new Forklift
        }

        private void MovementControls(InputAction.CallbackContext context) {
            _controlVectors = context.ReadValue<Vector2>();
        }

        // Update is called once per frame
        void Update() {
            _controlVectors = _input.ForkliftInput.Movement.ReadValue<Vector2>();

            _rigidBody.AddForce(_transform.forward * _moveForce * _controlVectors.y);
            _rigidBody.AddTorque(_transform.up * _rotationForce * _controlVectors.x);
            
            _fork.Update(_input);
        }
    }

    [Serializable]
    public class ForkController {
        [SerializeField] private Transform _forkTransform;
        [SerializeField] private float _speed;
        [SerializeField] private float _minHeight;
        [SerializeField] private float _maxHeigh;

        private float _liftValue;

        public void Update(GameInputs input) {

            _liftValue = Mathf.Clamp01(_liftValue + input.ForkliftInput.ForkControl.ReadValue<float>());
            Debug.Log($"LiftValue: {_liftValue}");
        }
    }
}