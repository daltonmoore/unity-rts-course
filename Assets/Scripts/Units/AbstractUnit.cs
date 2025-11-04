using System;
using System.Collections.Generic;
using EventBus;
using Events;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Units
{
    [RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))]
    public abstract class AbstractUnit : AbstractCommandable, IMoveable, IAttacker
    {
        public float AgentRadius => Agent.radius;
        
        [field: SerializeField] public ParticleSystem AttackingParticleSystem { get; private set; }
        [SerializeField] private DamageableSensor damageableSensor;
        
        protected BehaviorGraphAgent GraphAgent;
        protected NavMeshAgent Agent;
        protected Animator Animator;
        protected UnitSO _unitSO;

        protected virtual void Awake() 
        { 
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            GraphAgent = GetComponent<BehaviorGraphAgent>();
            _unitSO = UnitSO as UnitSO;
            GraphAgent.SetVariableValue("Command", UnitCommands.Stop);
            GraphAgent.SetVariableValue("AttackConfig", _unitSO.AttackConfig);
            GraphAgent.SetVariableValue<GameObject>("TargetGameObject", null);
        }

        protected override void Start()
        {
            base.Start();
            CurrentHealth = UnitSO.Health;
            MaxHealth = UnitSO.Health;
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));

            if (damageableSensor != null)
            {
                damageableSensor.OnUnitEnter += HandleUnitEnter;
                damageableSensor.OnUnitExit += HandleUnitExit;
                damageableSensor.SetupFrom(_unitSO.AttackConfig);
            }
        }

        public void MoveTo(Vector3 position)
        {
            SetCommandOverrides(null);
            GraphAgent.SetVariableValue("TargetLocation", position);
            GraphAgent.SetVariableValue("Command", UnitCommands.Move);
        }

        public void Stop()
        {
            SetCommandOverrides(null);
            GraphAgent.SetVariableValue("Command", UnitCommands.Stop);
        }

        protected virtual void OnDestroy()
        {
            Bus<UnitDeathEvent>.Raise(new UnitDeathEvent(this));
        }
        
        private void HandleUnitEnter(IDamageable damageable)
        {
            List<GameObject> nearbyEnemies = SetNearbyEnemiesOnBlackboard();

            if (GraphAgent.GetVariable("TargetGameObject", out BlackboardVariable<GameObject> target) 
                && target.Value == null && nearbyEnemies.Count > 0)
            {
                GraphAgent.SetVariableValue("TargetGameObject", nearbyEnemies[0]);
            }
        }
        
        private void HandleUnitExit(IDamageable damageable)
        {
            List<GameObject> nearbyEnemies = SetNearbyEnemiesOnBlackboard();

            if (!GraphAgent.GetVariable("TargetGameObject", out BlackboardVariable<GameObject> target)
                || damageable.Transform.gameObject != target.Value) return;

            if (nearbyEnemies.Count > 0)
            {
                GraphAgent.SetVariableValue("TargetGameObject", nearbyEnemies[0]);
            }
            else
            {
                GraphAgent.SetVariableValue<GameObject>("TargetGameObject", null);
                GraphAgent.SetVariableValue("TargetLocation", damageable.Transform.position);
            }
        }

        private List<GameObject> SetNearbyEnemiesOnBlackboard()
        {
            List<GameObject> nearbyEnemies = damageableSensor.Damageables.ConvertAll(d => d.Transform.gameObject);
            nearbyEnemies.Sort(new ClosestGameObjectComparer(transform.position));
            GraphAgent.SetVariableValue("NearbyEnemies", nearbyEnemies);
            
            return nearbyEnemies;
        }

        public void Attack(IDamageable damageable)
        {
            GraphAgent.SetVariableValue("TargetGameObject", damageable.Transform.gameObject);
            GraphAgent.SetVariableValue("Command", UnitCommands.Attack);
        }

        public void Attack(Vector3 location)
        {
            GraphAgent.SetVariableValue<GameObject>("TargetGameObject", null);
            GraphAgent.SetVariableValue("TargetLocation", location);
            GraphAgent.SetVariableValue("Command", UnitCommands.Attack);
        }
    }
}