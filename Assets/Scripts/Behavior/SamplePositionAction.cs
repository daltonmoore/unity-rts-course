using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Sample Position", story: "Set [TargetLocation] to the closest point on the NavMesh to [Target]", category: "Action/Navigation", id: "e2cdd36a9a7ea6610bfd0220a8fcdfae")]
    public partial class SamplePositionAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> Radius = new(5);

        protected override Status OnStart()
        {
            if (Target.Value == null || !Target.Value.TryGetComponent(out NavMeshAgent agent)) return Status.Failure;
            
            NavMeshQueryFilter queryFilter = new()
            {
                agentTypeID = agent.agentTypeID,
                areaMask = agent.areaMask
            };

            if (NavMesh.SamplePosition(Target.Value.transform.position, out NavMeshHit hit, Radius, queryFilter))
            {
                TargetLocation.Value = hit.position;
                return Status.Success;
            }
            
            return Status.Failure;
        }
    }
}

