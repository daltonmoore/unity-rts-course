using Player;
using Units;
using UnityEngine;

namespace Commands
{
    [CreateAssetMenu(fileName = "Build Unit", menuName = "Buildings/Commands/Build Unit", order = 120)]
    public class BuildUnitCommand : ActionBase
    {
        [field: SerializeField] public AbstractUnitSO Unit { get; private set; }
        
        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is BaseBuilding && HasEnoughSupplies();
        }

        public override void Handle(CommandContext context)
        {
            if (!HasEnoughSupplies()) return;
            
            BaseBuilding building = (BaseBuilding)context.Commandable;
            building.BuildUnit(Unit);
        }
        
        private bool HasEnoughSupplies() => Unit.Cost.Minerals <= Supplies.Minerals && Unit.Cost.Gas <= Supplies.Gas;
    }
}