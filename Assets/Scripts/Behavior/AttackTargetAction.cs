using System;
using System.Collections.Generic;
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
        [SerializeReference] public BlackboardVariable<List<GameObject>> NearbyEnemies;
        
        private NavMeshAgent _agent;
        private Transform _selfTransform;
        private Animator _animator;
        private AbstractUnit _unit;
        
        private IDamageable _targetDamageable;
        private Transform _targetTransform;

        private float _lastAttackTime;

        protected override Status OnStart()
        {
            if (!HasValidInputs()) return Status.Failure;
            
            _selfTransform = Self.Value.transform;
            _agent = Self.Value.GetComponent<NavMeshAgent>();
            _animator = Self.Value.GetComponent<Animator>();
            _unit = Self.Value.GetComponent<AbstractUnit>();

            _targetDamageable = Target.Value.GetComponent<IDamageable>();
            _targetTransform = Target.Value.transform;

            if (!NearbyEnemies.Value.Contains(Target.Value))
            {
                _agent.SetDestination(_targetTransform.position);
                _agent.isStopped = false;
                if (_animator != null)
                {
                    _animator.SetBool(AnimationConstants.ATTACK, false);
                }
            }

            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (Target.Value == null || _targetDamageable.CurrentHealth == 0) return Status.Success;

            if (_animator != null)
            {
                _animator.SetFloat(AnimationConstants.SPEED, _agent.velocity.magnitude);
            }
            
            if (!NearbyEnemies.Value.Contains(Target.Value))
            {
                return Status.Running;
            }
            
            _agent.isStopped = true;
            
            Quaternion lookRotation = Quaternion.LookRotation(
                (_targetTransform.position - _selfTransform.position).normalized,
                Vector3.up
            );
            _selfTransform.rotation = Quaternion.Euler(
                _selfTransform.rotation.eulerAngles.x, 
                lookRotation.eulerAngles.y, 
                _selfTransform.rotation.eulerAngles.z
            );
            
            if (_animator != null)
            {
                _animator.SetBool(AnimationConstants.ATTACK, true);
            }

            if (Time.time >= _lastAttackTime + AttackConfig.Value.AttackDelay)
            {
                _lastAttackTime = Time.time;
                if (_unit.AttackingParticleSystem != null)
                {
                    _unit.AttackingParticleSystem.Play();
                }

                if (!AttackConfig.Value.HasProjectileAttacks)
                {
                    _targetDamageable.TakeDamage(AttackConfig.Value.Damage);
                    // projectile attacks are handled by the specific subclass of AbstractUnit that shoot the projectile
                }
            }
            
            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (_animator != null)
            {
                _animator.SetBool(AnimationConstants.ATTACK, false);
                _animator.SetFloat(AnimationConstants.SPEED, 0);
            }
        }
        
        private bool HasValidInputs() => Self.Value != null 
                                         && Self.Value.TryGetComponent(out AbstractUnit _)
                                         && Self.Value.TryGetComponent(out NavMeshAgent _) 
                                         && Target.Value != null 
                                         && Target.Value.TryGetComponent(out IDamageable _)
                                         && AttackConfig.Value != null
                                         && NearbyEnemies.Value != null;
    }
}

