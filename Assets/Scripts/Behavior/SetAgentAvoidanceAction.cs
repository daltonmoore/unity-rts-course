using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set Agent Avoidance", story: "Set [Agent] avoidance quality to [AvoidanceQuality]", category: "Action/Navigation", id: "93ba6262e71ae931c83b9913f23e8ac1")]
    public partial class SetAgentAvoidanceAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<int> AvoidanceQuality;

        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out NavMeshAgent agent) || AvoidanceQuality.Value > 4 || AvoidanceQuality.Value < 0)
            {
                return Status.Failure;
            }
            
            agent.obstacleAvoidanceType = (ObstacleAvoidanceType)AvoidanceQuality.Value;
            return Status.Success;
        }
    }
}

