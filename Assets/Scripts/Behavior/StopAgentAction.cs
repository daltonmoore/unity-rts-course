using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Stop Agent", story: "[Agent] stops moving", category: "Action/Navigation", id: "b005da095c4999f5717aa8854e8f9082")]
    public partial class StopAgentAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;

        protected override Status OnStart()
        {
            if (Agent.Value.TryGetComponent(out NavMeshAgent agent))
            {
                agent.ResetPath();
                return Status.Success;           
            }

            return Status.Failure;
        }
    }
}

