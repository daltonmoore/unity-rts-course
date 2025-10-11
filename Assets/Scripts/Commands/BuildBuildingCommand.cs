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
            return context.Commandable is IBuildingBuilder;
        }

        public override void Handle(CommandContext context)
        {
            ((IBuildingBuilder)context.Commandable).Build(BuildingSO, context.Hit.point);
        }
    }
}