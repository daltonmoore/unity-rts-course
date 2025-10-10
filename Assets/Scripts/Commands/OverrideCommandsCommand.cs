using Units;
using UnityEngine;

namespace Commands
{
    [CreateAssetMenu(fileName = "Override Commands", menuName = "Units/Commands/Override", order = 110)]
    public class OverrideCommandsCommand : ActionBase
    {
        [field: SerializeField] public ActionBase[] Commands { get; private set; }
        
        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable != null;
        }

        public override void Handle(CommandContext context)
        {
            context.Commandable.SetCommandOverrides(Commands);
        }
    }
}