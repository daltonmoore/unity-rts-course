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
    [NodeDescription(name: "TranslatePosition", story: "[Self] moves to [TargetLocation] at speed [Speed]", category: "Action/Navigation", id: "4227583eeb0567b18a62f46e2bb81e3b")]
    public partial class TranslatePositionAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<float> Speed;
        
        private Animator _animator;
        private NavMeshAgent _agent;
        private float _endTime;
        private Vector3 _direction;
        private Transform _selfTransform;

        protected override Status OnStart()
        {
            if (Self.Value == null) return Status.Failure;

            _animator = Self.Value.GetComponent<Animator>();
            
            if (Self.Value.TryGetComponent(out _agent))
            {
                _agent.enabled = false;
            }
            
            _selfTransform = Self.Value.transform;
            float distance = Vector3.Distance(_selfTransform.position, TargetLocation.Value);
            _endTime = Time.time + distance / Speed;
            _direction = (TargetLocation.Value - _selfTransform.position).normalized;
            
            _selfTransform.forward = _direction;
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Time.time >= _endTime) return Status.Success;
            
            if (_animator != null)
            {
                _animator.SetFloat(AnimationConstants.SPEED, Speed);
            }

            _selfTransform.position += _direction * Speed * Time.deltaTime;
            
            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (_animator != null)
            {
                _animator.SetFloat(AnimationConstants.SPEED, 0);
            }
            
            if (_agent != null)
            {
                _agent.enabled = true;
            }
        }
    }
}

