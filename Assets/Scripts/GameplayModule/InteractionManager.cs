using UnityEngine;

namespace GameplayModule
{
    public class InteractionManager : MonoBehaviour
    {
        public bool allowInteractions = true;
        public bool _isCarryingBag = false;

        public delegate void OnCarryingStatusChange(bool newValue);

        public event OnCarryingStatusChange OnCarryingStatusChangeEvent;

        public bool IsCarryingBag
        {
            get { return _isCarryingBag; }
            set
            {
                if (_isCarryingBag != value)
                {
                    _isCarryingBag = value;
                    OnCarryingStatusChangeEvent(value);
                }
            }
        }
    }
}
