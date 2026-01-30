using Logic;
using Tools;
using UnityEngine;

namespace Gameplay {
    public class SpawnManager : MonoBehaviour {
        [SerializeField] private BoxController _boxPrefab;

        [SerializeField] private Transform[] _spawnPoint;
        private GenericObjectPool<BoxController> _pool;

        private BoxController _cachedBox;

        private void Awake() {
            _pool = new GenericObjectPool<BoxController>(SpawnBox);
            SpawnNewBox();
        }

        private void SpawnNewBox() {
            _cachedBox = _pool.TryTake();
            _cachedBox.SetPosition(_spawnPoint[Random.Range(0, _spawnPoint.Length)].position);
            _cachedBox.SetRotation(Quaternion.identity);
        }

        private BoxController SpawnBox() {
            BoxController boxController = Instantiate(_boxPrefab,
                _spawnPoint[Random.Range(0, _spawnPoint.Length)].position, Quaternion.identity);
            boxController.OnDeactivate += _pool.Release;
            
            return boxController;
        }
    }
}