using Units;
using UnityEngine;

namespace Commands
{
    [CreateAssetMenu(fileName = "Cancel Building", menuName = "Units/Commands/Cancel Building", order = 130)]
    public class CancelBuildingCommand : BaseCommand
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
        
        public override bool IsLocked(CommandContext context) => false;
    }
}