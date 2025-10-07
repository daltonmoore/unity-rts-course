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
        private Animator _animator;
        private LayerMask _suppliesMask;
        private Collider[] _nearbyAvailableSupplies = new Collider[20];
        private SupplySO _supplySO;
        
        protected override Status OnStart()
        {
            _suppliesMask = LayerMask.GetMask("GatherableSupplies");

            if (!HasValidInputs())
            {
                return Status.Failure;
            }
            
            _agent.TryGetComponent(out _animator);
            
            _agent.SetDestination(GetDestination());
            
            return Status.Running;
        }
        
        protected override Status OnUpdate()
        {
            if (_animator != null)
            {
                _animator.SetFloat(AnimationConstants.SPEED, _agent.velocity.magnitude);
            }
            
            if (_agent.pathPending || _agent.remainingDistance >= _agent.stoppingDistance)
            {
                return Status.Running;
            }

            if (Supply.Value != null && !Supply.Value.IsBusy && Supply.Value.Amount > 0)
            {
                return Status.Success;
            }
            
            var size = FindNearbyAvailableSuppliesNonAlloc(_nearbyAvailableSupplies);

            if (size > 0)
            {
                Array.Sort(_nearbyAvailableSupplies.Where(c => c is not null).ToArray(), new ClosestColliderComparer(_agent.transform.position));
                Supply.Value = _nearbyAvailableSupplies[0].GetComponent<GatherableSupply>();
                _agent.SetDestination(GetDestination());
                return Status.Running;
            }
            
            return Status.Failure;
        }

        protected override void OnEnd()
        {
            if (_animator != null)
            {
                _animator.SetFloat(AnimationConstants.SPEED, 0);
            }
        }

        private bool HasValidInputs()
        {
            if (!Agent.Value.TryGetComponent(out _agent) || (Supply.Value == null && _supplySO == null))
            {
                return false;
            }
            
            if (Supply.Value != null)
            {
                _supplySO = Supply.Value.SupplySO;
            }
            else
            {
                FindNearbyAvailableSuppliesNonAlloc(_nearbyAvailableSupplies);
                if (_nearbyAvailableSupplies.Length > 0)
                {
                    Array.Sort(_nearbyAvailableSupplies.Where(c => c is not null).ToArray(), new ClosestColliderComparer(_agent.transform.position));
                    Supply.Value = _nearbyAvailableSupplies[0].GetComponent<GatherableSupply>();
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private int FindNearbyAvailableSuppliesNonAlloc(Collider[] buffer)
        {
            // Fill the buffer with nearby colliders without allocations.
            int count = Physics.OverlapSphereNonAlloc(
                _agent.transform.position,
                SearchRadius.Value,
                buffer,
                _suppliesMask);

            // Compact the buffer in-place to keep only available and matching supplies; return the new count.
            int write = 0;
            for (int read = 0; read < count; read++)
            {
                var col = buffer[read];
                if (IsAvailableMatchingSupply(col))
                {
                    buffer[write++] = col;
                }
            }

            // Clear any remaining entries to avoid stale references being used elsewhere.
            for (int i = write; i < buffer.Length; i++)
            {
                buffer[i] = null;
            }

            return write;
        }

        private bool IsAvailableMatchingSupply(Collider collider)
        {
            if (collider == null) return false;
            if (!collider.TryGetComponent(out GatherableSupply supply)) return false;
            if (supply.IsBusy) return false;

            return supply.SupplySO.Equals(_supplySO);
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

