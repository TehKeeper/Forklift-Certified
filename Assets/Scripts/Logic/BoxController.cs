using System;
using Cysharp.Threading.Tasks;
using Tools;
using UnityEngine;

namespace Logic {
    /// <summary>Cargo box reactions and physics behaviour</summary>
    public class BoxController : MonoBehaviour, IObserver<Vector3>, IActivatable {
        [SerializeField] private LayerMask _layerMask;

        private Rigidbody _rigidbody;
        private bool _dormant;
        private Transform _driver;
        private Vector3 _offset;
        private Transform _transform;
        private IObservableCustom<Vector3> _observableCustom;
        private bool _checkFloor;
        private GameObject _gameObj;

        public event Action<BoxController> OnDeactivate;

        private Collider[] _colliders;

        private void Awake() {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _gameObj = gameObject;
            _colliders = GetComponentsInChildren<Collider>();
        }

        private void Update() {
            if (_checkFloor) {
                if (Physics.Linecast(_transform.position, _transform.position + Vector3.down * 0.1f, out RaycastHit hit,
                        layerMask: _layerMask)) {
                    AwakeBody();
                }
            }
        }

        private void OnTriggerEnter(Collider other) {
            ReleaseContact(other);
            ForkContact(other);
        }

        /// <summary>Reaction on contact with fork of forklift</summary>
        private void ForkContact(Collider other) {
            if (!other.CompareTag("Fork")) {
                return;
            }

            Debug.Log("Fork Contact");

            if (_dormant)
                return;

            _ = DelayCheck();

            _dormant = true;
            _transform.parent = other.transform;

            _rigidbody.isKinematic = true;
        }

        public void OnCollisionEnter(Collision collision) {
            AwakeBody();
        }

        /// <summary>Reaction on contact with Drop Zone. Disables all colliders, lifts up and rotates object</summary>
        private void ReleaseContact(Collider other) {
            if (!other.CompareTag("DropZone")) {
                return;
            }

            foreach (Collider item in _colliders) {
                item.enabled = false;
            }

            _transform.parent = null;

            _checkFloor = false;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = false;
            _rigidbody.constraints = RigidbodyConstraints.None;

            _rigidbody.AddForce(Vector3.up * 20f, ForceMode.Impulse);
            Vector3 torque = new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1),
                UnityEngine.Random.Range(-1, 1)) * 100f;
            _rigidbody.AddTorque(torque);

            _ = WaitForDispose();
        }

        private async UniTask DelayCheck() {
            await UniTask.WaitForSeconds(1f);
            Debug.Log("Delay Check");
            _checkFloor = true;
        }


        /// <summary>Releases object from fork control </summary>
        private void AwakeBody() {
            Debug.Log($"Awake Body");
            if (!_dormant) {
                _checkFloor = false;
                return;
            }

            _rigidbody.isKinematic = false;

            _transform.parent = null;
            _dormant = false;
            _checkFloor = false;
        }

        #region Observer methods

        public void OnNext(Vector3 value) {
            _transform.position = _offset + value;
        }

        public void OnCompleted() {
        }

        public void OnError(Exception error) {
        }

        #endregion

        public void SetPosition(Vector3 newPos) {
            _transform.position = newPos;
        }

        public void SetRotation(Quaternion newRotation) {
            _transform.rotation = newRotation;
        }

        #region Activate and Deactivate methods

        public void Activate() {
            _gameObj.SetActive(true);
            _rigidbody.WakeUp();
            foreach (Collider item in _colliders) {
                item.enabled = true;
            }

            //_checkFloor = true;
            _rigidbody.useGravity = true;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        /// <summary>Delay before object will be returned to pool</summary>
        private async UniTask WaitForDispose() {
            await UniTask.WaitForSeconds(5);
            OnDeactivate?.Invoke(this);
        }


        public void Deactivate() {
            _transform.rotation = Quaternion.identity;
            _rigidbody.Sleep();
            gameObject.SetActive(false);
        }

        #endregion

        private void OnDestroy() {
            OnDeactivate = null;
        }
    }
}