using System;
using Environment;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Utilities;
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

        private Animator _animator;
        private float _gatherStartTime;
    
        protected override Status OnStart()
        {
            if (GatherableSupplies.Value == null)
            {
                return Status.Failure;
            }

            if (Unit.Value.TryGetComponent(out _animator))
            {
                _animator.SetBool(AnimationConstants.IS_GATHERING, true);
            }
            
            _gatherStartTime = Time.time;
            GatherableSupplies.Value.BeginGather();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (_gatherStartTime + GatherableSupplies.Value.SupplySO.BaseGatherTime <= Time.time)
            {
                return Status.Success;
            }
        
            return Status.Running;       
        }

        protected override void OnEnd()
        {
            if (Unit.Value.TryGetComponent(out _animator))
            {
                _animator.SetBool(AnimationConstants.IS_GATHERING, false);
            }
            
            if (GatherableSupplies.Value == null) return;
            
            if (CurrentStatus == Status.Success)
            {
                Amount.Value = GatherableSupplies.Value.EndGather();
            }
            else
            { 
                GatherableSupplies.Value.AbortGather();
            }
        }
    }
}

