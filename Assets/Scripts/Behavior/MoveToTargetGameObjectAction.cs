using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Move to Target GameObject", story: "[Agent] moves to [TargetGameObject]", category: "Action/Navigation", id: "2558f818da8f7f59d8c5eca3ef159164")]
    public partial class MoveToTargetGameObjectAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> TargetGameObject;

        private NavMeshAgent _agent;
        
        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out _agent))
            {
                return Status.Failure;
            }

            Vector3 destination = GetDestination();
            
            if (Vector3.Distance(_agent.transform.position, destination) <= _agent.stoppingDistance)
            {
                return Status.Success;       
            }

            _agent.SetDestination(destination);
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                return Status.Success;
            }
            
            return Status.Running;
        }
        
        private Vector3 GetDestination()
        {
            Vector3 destination;
            if (TargetGameObject.Value.TryGetComponent(out Collider collider))
            {
                destination = collider.ClosestPoint(_agent.transform.position);
            }
            else
            {
                destination = TargetGameObject.Value.transform.position;
            }

            return destination;
        }
    }
}

