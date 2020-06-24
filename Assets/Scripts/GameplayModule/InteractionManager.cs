using UnityEngine;

namespace GameplayModule
{
    public class InteractionManager : MonoBehaviour
    {
        public bool allowInteractions = true;
        private bool _isCarryingBag;

        public delegate void OnCarryingStatusChange(bool newValue);

        public event OnCarryingStatusChange OnCarryingStatusChangeEvent;

        public bool IsCarryingBag
        {
            get => _isCarryingBag;
            set
            {
                if (_isCarryingBag != value)
                {
                    _isCarryingBag = value;
                    OnCarryingStatusChangeEvent?.Invoke(value);
                }
            }
        }
    }
}
