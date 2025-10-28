using System;
using Units;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Utilities;
using Action = Unity.Behavior.Action;

namespace Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Attack Target", story: "[Self] attacks [Target] until it dies", category: "Action", id: "c18b6a0c91bce405fd6ac8c2f316c3e3")]
    public partial class AttackTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<AttackConfigSO> AttackConfig;
        
        private NavMeshAgent _agent;
        private Transform _selfTransform;
        private Animator _animator;
        
        private IDamageable _targetDamageable;
        private Transform _targetTransform;

        private float _lastAttackTime;

        protected override Status OnStart()
        {
            if (!HasValidInputs()) return Status.Failure;
            
            _selfTransform = Self.Value.transform;
            _agent = Self.Value.GetComponent<NavMeshAgent>();
            _animator = Self.Value.GetComponent<Animator>();

            _targetDamageable = Target.Value.GetComponent<IDamageable>();
            _targetTransform = Target.Value.transform;

            if (_animator != null)
            {
                _animator.SetBool(AnimationConstants.ATTACK, true);
            }

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Target.Value == null || _targetDamageable.CurrentHealth == 0) return Status.Success;
            
            if (Vector3.Distance(_targetTransform.position, _selfTransform.position) >= AttackConfig.Value.AttackRange)
            {
                _agent.SetDestination(_targetTransform.position);
                _agent.isStopped = false;
                return Status.Running;
            }
            
            _agent.isStopped = true;

            if (Time.time >= _lastAttackTime + AttackConfig.Value.AttackDelay)
            {
                _lastAttackTime = Time.time;
                _targetDamageable.TakeDamage(AttackConfig.Value.Damage);
            }
            
            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (_animator != null)
            {
                _animator.SetBool(AnimationConstants.ATTACK, false);
            }
        }
        
        private bool HasValidInputs() => Self.Value != null 
                                         && Self.Value.TryGetComponent(out NavMeshAgent _) 
                                         && Target.Value != null 
                                         && Target.Value.TryGetComponent(out IDamageable _)
                                         && AttackConfig.Value != null;
    }
}

