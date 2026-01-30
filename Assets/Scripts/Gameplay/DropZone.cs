using UnityEngine;

namespace Gameplay {
    /// <summary>Zone that reacts to box placement</summary>
    public class DropZone : MonoBehaviour {
        [SerializeField] private SpawnManager _spawner;

        private void OnTriggerEnter(Collider other) {
            Debug.Log("Dropped!");
            if (!_spawner)
                return;
            
            if (other.CompareTag("Cargo"))
                _spawner.SpawnBox();
        }
    }
}