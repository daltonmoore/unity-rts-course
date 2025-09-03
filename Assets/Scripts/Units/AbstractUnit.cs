using System;
using EventBus;
using Events;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class AbstractUnit : MonoBehaviour, ISelectable, IMoveable
    {
        [SerializeField] private DecalProjector decal;
        
        public float agentRadius => _agent.radius;
        
        private NavMeshAgent _agent;
        
        private void Awake() 
        { 
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        }

        public void Select()
        {
            decal?.gameObject.SetActive(true);
            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this));
        }

        public void Deselect()
        {
            decal?.gameObject.SetActive(false);
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
        }

        public void MoveTo(Vector3 position)
        {
            _agent.SetDestination(position);
        }
    }
}