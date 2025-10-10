using Commands;
using EventBus;
using Events;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Units
{
    public abstract class AbstractCommandable : MonoBehaviour, ISelectable
    {
        [field: SerializeField] public int CurrentHealth { get; private set; }
        [field: SerializeField] public int MaxHealth { get; private set; }
        [field: SerializeField] public ActionBase[] AvailableCommands { get; private set; }
        [field: SerializeField] public UnitSO UnitSO { get; private set; }

        [SerializeField] private DecalProjector decal;

        private ActionBase[] _initialCommands;

        protected virtual void Start()
        {
            CurrentHealth = UnitSO.Health;
            MaxHealth = UnitSO.Health;
            _initialCommands = AvailableCommands;
        }

        public void Select()
        {
            if (decal != null)
            {
                decal.gameObject.SetActive(true);
            }
            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
        }

        public void Deselect()
        {
            if (decal != null)
            {
                decal.gameObject.SetActive(false);
            }
            
            SetCommandOverrides(null);
            
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
        }

        public void SetCommandOverrides(ActionBase[] overrides)
        {
            if (overrides == null || overrides.Length == 0)
            {
                AvailableCommands = _initialCommands;
            }
            else
            {
                AvailableCommands = overrides;
            }
            
            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
        }
    }
}