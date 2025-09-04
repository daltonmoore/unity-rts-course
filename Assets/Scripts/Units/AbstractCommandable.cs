using EventBus;
using Events;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Units
{
    public abstract class AbstractCommandable : MonoBehaviour, ISelectable
    {
        [SerializeField] private DecalProjector decal;
        
        [field: SerializeField] public int Health { get; private set; }
        
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
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
        }
    }
}