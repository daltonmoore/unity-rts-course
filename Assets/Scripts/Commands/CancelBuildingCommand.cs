using Units;
using UnityEngine;

namespace Commands
{
    [CreateAssetMenu(fileName = "Cancel Building", menuName = "Units/Commands/Cancel Building", order = 130)]
    public class CancelBuildingCommand : ActionBase
    {
        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is IBuildingBuilder;
        }

        public override void Handle(CommandContext context)
        {
            IBuildingBuilder builder = (IBuildingBuilder)context.Commandable;
            builder.CancelBuilding();
        }
    }
}