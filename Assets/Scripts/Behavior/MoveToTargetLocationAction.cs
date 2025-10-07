using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Utilities;
using Action = Unity.Behavior.Action;

namespace Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "MoveToTargetLocation",
        description: "My version of the NavigateToLocationAction. Navigates a GameObject to a specified position using NavMeshAgent.",
        story: "[Agent] moves to [TargetLocation]",
        category: "Action/Navigation",
        id: "91ae82aa1575a2ddc34d75f70ca27237")]
    public partial class MoveToTargetLocationAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;

        private NavMeshAgent _agent;
        private Animator _animator;
    
        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out _agent))
            {
                return Status.Failure;
            }

            _agent.TryGetComponent(out _animator);

            if (Vector3.Distance(_agent.transform.position, TargetLocation.Value) <= _agent.stoppingDistance)
            {
                return Status.Success;       
            }

            _agent.SetDestination(TargetLocation.Value);
        
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (_animator != null)
            {
                _animator.SetFloat(AnimationConstants.SPEED, _agent.velocity.magnitude);
            }
            
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                return Status.Success;
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (_animator != null)
            {
                _animator.SetFloat(AnimationConstants.SPEED, 0);
            }
        }
    }
}

