using Environment;
using Units;
using UnityEngine;

namespace Commands
{
    [CreateAssetMenu(fileName = "Gather Action", menuName = "AI/Commands/Gather", order = 105)]
    public class GatherCommand : ActionBase
    {
        public override bool CanHandle(CommandContext context)
        {
            return context.Commandable is Worker 
                   && context.Hit.collider != null 
                   && context.Hit.collider.TryGetComponent(out GatherableSupply _);
        }

        public override void Handle(CommandContext context)
        {
            Worker worker = context.Commandable as Worker;
            worker.Gather(context.Hit.collider.GetComponent<GatherableSupply>());
        }
    }
}