using Units;
using UnityEngine;

namespace Commands
{
    [CreateAssetMenu(fileName = "Build Building Action", menuName = "Units/Commands/Build Building", order = 120)]
    public class BuildBuildingCommand : ActionBase
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
            
            return true;
        }

        public override void Handle(CommandContext context)
        {
            IBuildingBuilder builder = (IBuildingBuilder)context.Commandable;
            if (context.Hit.collider != null && context.Hit.collider.TryGetComponent(out BaseBuilding building))
            {
                builder.ResumeBuilding(building);
            }
            else
            {
                builder.Build(BuildingSO, context.Hit.point);
            }


        }
    }
}