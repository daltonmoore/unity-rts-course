using System;
using Environment;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Gather Supplies", story: "[Unit] gathers [Amount] supplies from [GatherableSupplies]", category: "Action/Units", id: "f2693d8c7c8ba858b9a81b75d19c7697")]
    public partial class GatherSuppliesAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Unit;
        [SerializeReference] public BlackboardVariable<int> Amount;
        [SerializeReference] public BlackboardVariable<GatherableSupply> GatherableSupplies;

        private float _gatherStartTime;
    
        protected override Status OnStart()
        {
            _gatherStartTime = Time.time;
            GatherableSupplies.Value.BeginGather();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (_gatherStartTime + GatherableSupplies.Value.SupplySO.BaseGatherTime <= Time.time)
            {
                int amountGathered = GatherableSupplies.Value.EndGather();
                return Status.Success;
            }
        
            return Status.Running;       
        }
    }
}

