using System.Linq;
using Units;
using UnityEngine;

namespace Commands
{
    public abstract class ActionBase : ScriptableObject, ICommand
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: Range(0, 8)] [field: SerializeField] public int Slot { get; private set; }
        [field: SerializeField] public bool RequiresClickToActivate { get; private set; } = true;
        [field: SerializeField] public GameObject GhostPrefab { get; private set; }
        [field: SerializeField] public BuildingRestrictionSO[] Restrictions { get; private set; }
        
        public abstract bool CanHandle(CommandContext context);
        public abstract void Handle(CommandContext context);
        
        public bool AllRestrictionsPass(Vector3 point) => 
            Restrictions.Length == 0 || Restrictions.All(r => r.CanPlace(point));
    }
}