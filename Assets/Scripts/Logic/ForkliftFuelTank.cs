using System;
using Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Logic {
    [Serializable]
    public class ForkliftFuelTank : IDisposable {
        public UnityEvent<float> _onValueChanged;
        public GenericEnumStateMachine<FuelTankState> StateMachine = new();

        [Range(0, 1)]
        [SerializeField] private float _fill = 1;

        [SerializeField] private float _consumptionRate = 0.02f;

        public void ConsumeFuel() {
            if (_fill > 0) {
                _fill = Mathf.Clamp01(_fill - Time.deltaTime * _consumptionRate);
            }

            _onValueChanged?.Invoke(_fill);
            StateMachine.SetState(StateOnFuel());
        }

        public FuelTankState StateOnFuel() {
            if (_fill > 0.5f)
                return FuelTankState.Full;

            if (_fill > 0)
                return FuelTankState.HalfEmpty;

            return FuelTankState.Empty;
        }

        public void Dispose() {
            StateMachine.Dispose();
            _onValueChanged.RemoveAllListeners();
        }
    }
}