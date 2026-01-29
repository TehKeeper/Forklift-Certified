using System;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Logic {
    public class BoxController : MonoBehaviour, IObserver<Vector3> {
        [SerializeField] private LayerMask _layerMask;

        private Rigidbody _rigidbody;
        private bool _dormant;
        private Transform _driver;
        private Vector3 _offset;
        private Transform _transform;
        private IObservableCustom<Vector3> _observableCustom;
        private bool _checkFloor;


        private void Awake() {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update() {
            if (_checkFloor) {
                if (Physics.Linecast(_transform.position, _transform.position + Vector3.down * 0.1f, out RaycastHit hit,
                        layerMask:_layerMask)) {
                    AwakeBody();
                }
            }
        }

        private void OnTriggerEnter(Collider other) {
            Debug.Log("CONTACT!");
            if (!other.CompareTag("Fork")) {
                return;
            }

            if (_dormant)
                return;

            _ = DelayCheck();

            _dormant = true;
            _transform.parent = other.transform;
            _rigidbody.isKinematic = true;
        }

        private async UniTask DelayCheck() {
            await UniTask.WaitForSeconds(1f);

            _checkFloor = true;
        }

        public void OnCollisionEnter(Collision collision) {
            Debug.Log($"Collision!, dormant: {_dormant}");
            AwakeBody();
        }

        private void AwakeBody() {
            Debug.Log($"Awake Body");
            if (!_dormant)
                return;


            _rigidbody.isKinematic = false;

            _transform.parent = null;
            _dormant = false;
            _checkFloor = false;
        }

        public void OnNext(Vector3 value) {
            _transform.position = _offset + value;
        }

        public void OnCompleted() {
        }

        public void OnError(Exception error) {
        }
    }
}