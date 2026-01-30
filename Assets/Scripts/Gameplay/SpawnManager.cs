using Cysharp.Threading.Tasks;
using Logic;
using Tools;
using UnityEngine;

namespace Gameplay {
    /// <summary>Spawns containters on one of several random points</summary>
    public class SpawnManager : MonoBehaviour {
        [SerializeField] private BoxController _boxPrefab;

        [SerializeField] private Transform[] _spawnPoint;
        private GenericObjectPool<BoxController> _pool;

        private BoxController _cachedBox;

        private void Awake() {
            _pool = new GenericObjectPool<BoxController>(CreateNewBox);
            SpawnBox(0);
        }

        /// <summary>Spawn box on point</summary>
        public void SpawnBox(int spawnPointId = -1) {
            _cachedBox = _pool.TryTake();
            Transform spawnPoint = _spawnPoint[spawnPointId < 0 ? Random.Range(0, _spawnPoint.Length) : spawnPointId];
            Vector3 spawnPointPosition = spawnPoint.position;
            Quaternion spawnPointRotation = spawnPoint.rotation;
            
            _cachedBox.SetPosition(spawnPointPosition + Vector3.up * 10f);
            _cachedBox.SetRotation(spawnPointRotation);

            _ = DriveBox(_cachedBox, spawnPointPosition, spawnPointRotation);
        }

        private async UniTask DriveBox(BoxController boxController, Vector3 spawnPoint, Quaternion spawnPointRotation) {
            float t = 5f;

            boxController.SetDrive(true);
            while (t > 0) {
                t -= Time.deltaTime;
                await UniTask.NextFrame();

                boxController.SetPosition(new Vector3(spawnPoint.x, spawnPoint.y + 10 * t / 5f, spawnPoint.z));
                boxController.SetRotation(Quaternion.Euler(0, t * 360f, 0));
            }

            boxController.SetDrive(false);

            _cachedBox.SetPosition(spawnPoint);
            _cachedBox.SetRotation(spawnPointRotation);

            t = 0;
        }

        /// <summary>Instantiating new box </summary>
        private BoxController CreateNewBox() {
            BoxController boxController = Instantiate(_boxPrefab,
                _spawnPoint[Random.Range(0, _spawnPoint.Length)].position, Quaternion.identity);
            boxController.OnDeactivate += _pool.Release;

            return boxController;
        }
    }
}