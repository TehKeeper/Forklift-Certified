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
            SpawnNewBox(0);
        }

        public void SpawnNewBox(int spawnPointId = -1) {
            _cachedBox = _pool.TryTake();
            _cachedBox.SetPosition(_spawnPoint[spawnPointId < 0 ? Random.Range(0, _spawnPoint.Length) : spawnPointId]
                .position);
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