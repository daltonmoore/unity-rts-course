using System.Linq;
using Player;
using Units;
using UnityEngine;

namespace Commands
{
    [CreateAssetMenu(fileName = "Build Building Action", menuName = "Units/Commands/Build Building", order = 120)]
    public class BuildBuildingCommand : BaseCommand
    {
        [field: SerializeField] public BuildingSO BuildingSO { get; private set; }
        
        public override bool CanHandle(CommandContext context)
        {
            if (context.Commandable is not IBuildingBuilder) return false;

            if (context.Hit.collider != null)
            {
                return context.Hit.collider.TryGetComponent(out BaseBuilding building)
                    && BuildingSO == building.BuildingSO
                    && building.Progress.State is 
                        BuildingProgress.BuildingState.Paused or BuildingProgress.BuildingState.Destroyed;
            }
            
            return HasEnoughSupplies() && AllRestrictionsPass(context.Hit.point);
        }

        public override void Handle(CommandContext context)
        {
            IBuildingBuilder builder = (IBuildingBuilder)context.Commandable;
            if (context.Hit.collider != null && context.Hit.collider.TryGetComponent(out BaseBuilding building))
            {
                builder.ResumeBuilding(building);
            }
            else if (HasEnoughSupplies() && AllRestrictionsPass(context.Hit.point))
            {
                builder.Build(BuildingSO, context.Hit.point);
            }
        }

        public override bool IsLocked(CommandContext context) => !HasEnoughSupplies();

        private bool HasEnoughSupplies() => BuildingSO.Cost.Minerals <= Supplies.Minerals && BuildingSO.Cost.Gas <= Supplies.Gas;
        
    }
}