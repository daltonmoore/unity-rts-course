using EventBus;
using Events;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class AbstractUnit : AbstractCommandable, IMoveable
    {
        public float AgentRadius => _agent.radius;
        
        private NavMeshAgent _agent;
        
        private void Awake() 
        { 
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        }

        public void MoveTo(Vector3 position)
        {
            _agent.SetDestination(position);
        }
    }
}