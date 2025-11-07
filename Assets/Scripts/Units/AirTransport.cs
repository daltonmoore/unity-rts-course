using System.Collections.Generic;
using Behavior;
using Unity.Behavior;
using UnityEngine;

namespace Units
{
    public class AirTransport : AbstractUnit, ITransporter
    {
        public int Capacity => _unitSO.TransportConfig.Capacity;
        [field: SerializeField] public int UsedCapacity { get; private set; }
        
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
        }

        public List<ITransportable> GetLoadedUnits()
        {
            throw new System.NotImplementedException();
        }

        public void Load(ITransportable unit)
        {
            if (UsedCapacity + unit.TransportCapacityUsage > Capacity) return;
            
            GraphAgent.SetVariableValue("TargetGameObject", unit.Transform.gameObject);
            GraphAgent.SetVariableValue("Command", UnitCommands.LoadUnits);
        }

        public void Load(ITransportable[] units)
        {
            throw new System.NotImplementedException();
        }

        bool ITransporter.Unload(ITransportable unit)
        {
            unit.Transform.SetParent(null);
            unit.Transform.gameObject.SetActive(true);
            return true;
        }

        public bool UnloadAll()
        {
            throw new System.NotImplementedException();
        }
    }
}
