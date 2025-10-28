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
        [SerializeField] private DamageableSensor damageableSensor;
        protected BehaviorGraphAgent GraphAgent;
        protected NavMeshAgent Agent;
        protected Animator Animator;
        protected UnitSO _unitSO;

        private void Awake() 
        { 
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            GraphAgent = GetComponent<BehaviorGraphAgent>();
            _unitSO = UnitSO as UnitSO;
            GraphAgent.SetVariableValue("Command", UnitCommands.Stop);
            GraphAgent.SetVariableValue("AttackConfig", _unitSO.AttackConfig);
        }

        protected override void Start()
        {
            base.Start();
            CurrentHealth = UnitSO.Health;
            MaxHealth = UnitSO.Health;
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));

            if (damageableSensor != null)
            {
                damageableSensor.OnUnitEnter += HandleUnitEnterOrExit;
                damageableSensor.OnUnitExit += HandleUnitEnterOrExit;
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

        private void OnDestroy()
        {
            Bus<UnitDeathEvent>.Raise(new UnitDeathEvent(this));
        }
        
        private void HandleUnitEnterOrExit(IDamageable damageable)
        {
            Debug.Log($"Damageables nearby are {damageableSensor.Damageables.Count}");
            List<GameObject> nearbyEnemies = damageableSensor.Damageables.ConvertAll(d => d.Transform.gameObject);
            nearbyEnemies.Sort(new ClosestGameObjectComparer(transform.position));
            GraphAgent.SetVariableValue("NearbyEnemies", nearbyEnemies);
        }

        public void Attack(IDamageable damageable)
        {
            Debug.Log($"Attacking {damageable.Transform.name}");
            GraphAgent.SetVariableValue("TargetGameObject", damageable.Transform.gameObject);
            GraphAgent.SetVariableValue("Command", UnitCommands.Attack);
        }
    }
}