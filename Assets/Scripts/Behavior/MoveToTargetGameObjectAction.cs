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
    [NodeDescription(name: "Move to Target GameObject", story: "[Agent] moves to [TargetGameObject]", category: "Action/Navigation", id: "2558f818da8f7f59d8c5eca3ef159164")]
    public partial class MoveToTargetGameObjectAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> TargetGameObject;
        [SerializeReference] public BlackboardVariable<float> MoveThreshold = new(0.25f);

        private NavMeshAgent _agent;
        private Animator _animator;
        private Vector3 _lastPosition;
        
        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out _agent) || TargetGameObject.Value == null)
            {
                return Status.Failure;
            }
            
            _agent.TryGetComponent(out _animator);

            Vector3 targetPosition = GetTargetPosition();
            
            if (Vector3.Distance(_agent.transform.position, targetPosition) <= _agent.stoppingDistance)
            {
                return Status.Success;       
            }

            _agent.SetDestination(targetPosition);
            _lastPosition = targetPosition;
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (_animator != null)
            {
                _animator.SetFloat(AnimationConstants.SPEED, _agent.velocity.magnitude);
            }

            Vector3 targetPosition = GetTargetPosition();
            if (Vector3.Distance(targetPosition, _lastPosition) >= MoveThreshold.Value)
            {
                _agent.SetDestination(targetPosition);
                _lastPosition = targetPosition;
                return Status.Running;
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

        private Vector3 GetTargetPosition()
        {
            Vector3 targetPosition;
            if (TargetGameObject.Value.TryGetComponent(out Collider collider))
            {
                targetPosition = collider.ClosestPoint(_agent.transform.position);
            }
            else
            {
                targetPosition = TargetGameObject.Value.transform.position;
            }

            return targetPosition;
        }
    }
}

