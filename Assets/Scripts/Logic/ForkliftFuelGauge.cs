using UnityEngine;

namespace Logic {
    public class ForkliftFuelGauge : MonoBehaviour {
        [SerializeField] private float _meterLimit = 60;
        [SerializeField] private Transform _meterRoot;
        [SerializeField] private Renderer[] _renderers;
        private MaterialPropertyBlock _propertyBlock;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");


        private void Awake() {
            _propertyBlock = new MaterialPropertyBlock();
        }

        public void OnValueChanged(float v) {
            Debug.Log($"On OnValueChanged {v}");
            _meterRoot.localEulerAngles = new Vector3(0, (_meterLimit * (v - 0.5f)) * 2f, 0);
        }

        public void TurnedOn(bool b) {
            _propertyBlock.SetColor(EmissionColor, b ? Color.white : Color.black);

            Debug.Log($"On Part on {b}");
            foreach (Renderer render in _renderers) {
                render.SetPropertyBlock(_propertyBlock);
                if (b)
                    render.material.EnableKeyword("_EMISSION");
                else
                    render.material.DisableKeyword("_EMISSION");
            }
        }
    }
}