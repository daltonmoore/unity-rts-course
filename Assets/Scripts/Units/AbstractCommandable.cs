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
        [field: SerializeField] public bool IsSelected { get; protected set; }
        [field: SerializeField] public int CurrentHealth { get; protected set; }
        [field: SerializeField] public int MaxHealth { get; protected set; }
        [field: SerializeField] public BaseCommand[] AvailableCommands { get; private set; }
        [field: SerializeField] public AbstractUnitSO UnitSO { get; private set; }

        [SerializeField] protected DecalProjector decal;
        
        public delegate void HealthUpdatedEvent(AbstractCommandable commandable, int lastHealth, int newHealth);
        public event HealthUpdatedEvent OnHealthUpdated;

        private BaseCommand[] _initialCommands;

        protected virtual void Start()
        {
            _initialCommands = AvailableCommands;
        }

        public virtual void Select()
        {
            if (decal != null)
            {
                decal.gameObject.SetActive(true);
            }
            
            IsSelected = true;
            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
        }

        public virtual void Deselect()
        {
            if (decal != null)
            {
                decal.gameObject.SetActive(false);
            }
            
            IsSelected = false;
            SetCommandOverrides(null);
            
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
        }

        public void SetCommandOverrides(BaseCommand[] overrides)
        {
            if (overrides == null || overrides.Length == 0)
            {
                AvailableCommands = _initialCommands;
            }
            else
            {
                AvailableCommands = overrides;
            }

            if (IsSelected)
            {
                Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
            }
        }

        public void Heal(int amount)
        {
            int lastHealth = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
            OnHealthUpdated?.Invoke(this, lastHealth, CurrentHealth);
        }
    }
}