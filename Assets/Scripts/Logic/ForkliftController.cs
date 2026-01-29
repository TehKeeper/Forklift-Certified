using UnityEngine;

namespace Logic {
    [RequireComponent(typeof(Rigidbody))]
    public class ForkliftController : MonoBehaviour {
        [SerializeField] private float _force = 100;
        
        
        private Rigidbody _rigidBody;

        //private ForkliftInput _input;

        // Start is called before the first frame update
        void Awake() {
            _rigidBody = GetComponent<Rigidbody>();
            
            
            //_inputControl =  new Forklift
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
