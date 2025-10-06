using System;
using System.Collections.Generic;
using System.Linq;
using Environment;
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
    [NodeDescription(name: "Move to GatherableSupply", story: "[Agent] moves to [Supply] or nearby not busy supply", category: "Action/Navigation", id: "613c38aa5c81b13d0cdc428dc2357bf5")]
    public partial class MoveToGatherableSupplyAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GatherableSupply> Supply;
        [SerializeReference] public BlackboardVariable<float> SearchRadius = new(7f);
        [SerializeReference] public BlackboardVariable<SupplySO> GatherableSupplySO;

        private NavMeshAgent _agent;
        private LayerMask _suppliesMask;
        private Collider[] _nearbySupplyColliders = new Collider[20];
        
        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent(out _agent))
            {
                return Status.Failure;
            }

            _suppliesMask = LayerMask.GetMask("GatherableSupplies");
            _agent.SetDestination(GetDestination());
            
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (!_agent.pathPending && _agent.remainingDistance >= _agent.stoppingDistance)
            {
                return Status.Running;
            }

            if (!Supply.Value.IsBusy && Supply.Value.Amount > 0)
            {
                return Status.Success;
            }

            
            Physics.OverlapSphereNonAlloc(
                _agent.transform.position, 
                SearchRadius, 
                _nearbySupplyColliders, 
                _suppliesMask);
            
            
            var colliders = _nearbySupplyColliders.Where(collider => collider is not null 
                                                                     && collider.TryGetComponent(out GatherableSupply supply) 
                                                                     && !supply.IsBusy 
                                                                     && supply.SupplySO.Equals(Supply.Value.SupplySO)).ToArray();

            if (colliders.Length > 0)
            {
                Array.Sort(colliders, new ClosestColliderComparer(_agent.transform.position));
                Supply.Value = colliders[0].GetComponent<GatherableSupply>();
                _agent.SetDestination(GetDestination());
                return Status.Running;
            }
            
            return Status.Failure;
        }
        
        private Vector3 GetDestination()
        {
            Vector3 destination;
            if (Supply.Value.TryGetComponent(out Collider collider))
            {
                destination = collider.ClosestPoint(_agent.transform.position);
            }
            else
            {
                destination = Supply.Value.transform.position;
            }

            return destination;
        }
    }
}

