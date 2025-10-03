using EventBus;
using Events;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    [RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))]
    public abstract class AbstractUnit : AbstractCommandable, IMoveable
    {
        public float AgentRadius => _agent.radius;
        
        protected BehaviorGraphAgent GraphAgent;
        
        private NavMeshAgent _agent;

        private void Awake() 
        { 
            _agent = GetComponent<NavMeshAgent>();
            GraphAgent = GetComponent<BehaviorGraphAgent>();
            GraphAgent.SetVariableValue("Command", UnitCommands.Stop);
        }

        protected override void Start()
        {
            base.Start();
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        }

        public void MoveTo(Vector3 position)
        {
            GraphAgent.SetVariableValue("TargetLocation", position);
            GraphAgent.SetVariableValue("Command", UnitCommands.Move);
        }

        public void Stop()
        {
            GraphAgent.SetVariableValue("Command", UnitCommands.Stop);
        }
    }
}