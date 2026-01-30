using Logic.Ui;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Logic.Camera {
    public class CameraController : MonoBehaviour {
        [SerializeField] private float _sensitivity = 10f;

        [Header("Limits")]
        [SerializeField] private float _minVerticalAngle = -60f; 

        [SerializeField] private float _maxVerticalAngle = 60f; 
        [SerializeField] private float _minHorAngle = -85f; 
        [SerializeField] private float _maxHorAngle = 85f; 

        private float _currentVerticalAngle; 
        private float _currentHorAngle; 
        private GameInputs _input;
        private Transform _transform;

        private void Awake() {
            
            _transform = transform;
            
            _transform.localRotation = Quaternion.identity;
            SplashScreenLogic.OnSceneReady += Initialize;
        }

        private void Initialize() {
            _input = new GameInputs();

            _input.ForkliftInput.Camera.performed += RotateCam;
            _input.ForkliftInput.Enable();
            
            SplashScreenLogic.OnSceneReady -= Initialize;
        }

        private void RotateCam(InputAction.CallbackContext ctx) {
            Vector2 delta = ctx.ReadValue<Vector2>() * _sensitivity * Time.deltaTime;

            _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle - delta.y, _minVerticalAngle, _maxVerticalAngle);
            _currentHorAngle = Mathf.Clamp(_currentHorAngle+delta.x, _minHorAngle, _maxHorAngle);

            _transform.localRotation = Quaternion.Euler(_currentVerticalAngle, _currentHorAngle, 0);
        }

        private void OnDestroy() {
            _input.ForkliftInput.Camera.performed -= RotateCam;
            _input.ForkliftInput.Disable();
        }
    }
}