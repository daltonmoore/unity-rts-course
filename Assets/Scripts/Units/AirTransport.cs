using System;
using System.Collections.Generic;
using System.Linq;
using Behavior;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public class AirTransport : AbstractUnit, ITransporter
    {
        public int Capacity => _unitSO.TransportConfig.Capacity;
        [field: SerializeField] public int UsedCapacity { get; private set; }
        [field: SerializeField] public float MaxUnloadDistance { private set; get; } = 0.25f;

        private List<ITransportable> _loadedUnits = new(8);

        public List<ITransportable> GetLoadedUnits() => _loadedUnits.ToList();
        
        protected override void Start()
        {
            base.Start();
            
            if (GraphAgent.GetVariable("LoadUnitEventChannel", 
                    out BlackboardVariable<LoadUnitEventChannel> loadUnitEventChannel))
            {
                loadUnitEventChannel.Value.Event += HandleLoadUnit;
            }
        }

        private void HandleLoadUnit(GameObject self, GameObject targetGameObject)
        {
            
            targetGameObject.SetActive(false);
            targetGameObject.transform.SetParent(self.transform);
            ITransportable transportable = targetGameObject.GetComponent<ITransportable>();
            UsedCapacity += transportable.TransportCapacityUsage;
            
            _loadedUnits.Add(transportable);
            
            if (GraphAgent.GetVariable("LoadUnitTargets", out BlackboardVariable<List<GameObject>> loadUnitTargets))
            {
                loadUnitTargets.Value.Remove(targetGameObject);
                GraphAgent.SetVariableValue("LoadUnitTargets", loadUnitTargets.Value);
            }
            
            if (UsedCapacity >= Capacity)
            {
                GraphAgent.SetVariableValue("Command", UnitCommands.Stop);
                GraphAgent.SetVariableValue("LoadUnitTargets",
                    new List<GameObject>(_unitSO.TransportConfig.Capacity));
            }
        }



        public void Load(ITransportable unit)
        {
            if (UsedCapacity + unit.TransportCapacityUsage > Capacity) return;
            
            if (GraphAgent.GetVariable("LoadUnitTargets", out BlackboardVariable<List<GameObject>> loadUnitTargets))
            {
                loadUnitTargets.Value.Add(unit.Transform.gameObject);
                GraphAgent.SetVariableValue("LoadUnitTargets", loadUnitTargets.Value);
            }
            
            GraphAgent.SetVariableValue("Command", UnitCommands.LoadUnits);
        }

        public void Load(ITransportable[] units)
        {
            throw new System.NotImplementedException();
        }

        public bool Unload(ITransportable unit)
        {
            NavMeshQueryFilter queryFilter = new()
            {
                areaMask = unit.Agent.areaMask,
                agentTypeID = unit.Agent.agentTypeID
            };

            if (Physics.Raycast(
                    transform.position,
                    Vector3.down,
                    out RaycastHit raycastHit,
                    float.MaxValue, 
                    _unitSO.TransportConfig.SafeDropLayers)
                && NavMesh.SamplePosition(raycastHit.point, out NavMeshHit hit, MaxUnloadDistance, queryFilter))
            {
                UsedCapacity -= unit.TransportCapacityUsage;
                unit.Transform.SetParent(null);
                unit.Transform.position = hit.position;
                unit.Transform.gameObject.SetActive(true);
                unit.Agent.Warp(hit.position);

                if (unit is IMoveable moveable)
                {
                    moveable.MoveTo(hit.position);
                }
                
                _loadedUnits.Remove(unit);
                return true;
            }
            
            return false;
        }

        public bool UnloadAll()
        {
            for (int i = _loadedUnits.Count - 1; i >= 0; i--)
            {
                if (Unload(_loadedUnits[i]))
                {
                    // Debug.Log($"Unloaded unit {_loadedUnits[i].Transform.name}!");
                }
                else
                {
                    Debug.Log($"Did not unload unit {_loadedUnits[i].Transform.name}!");
                }
            }
            
            return true;
        }
    }
}
