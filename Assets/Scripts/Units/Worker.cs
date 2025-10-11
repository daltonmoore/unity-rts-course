
using Behavior;
using Environment;
using EventBus;
using Events;
using Unity.Behavior;
using UnityEngine;

namespace Units
{
    public class Worker : AbstractUnit, IBuildingBuilder
    {
        public bool HasSupplies {
            get
            {
            if (GraphAgent != null && GraphAgent.GetVariable("SupplyAmountHeld", out BlackboardVariable<int> heldAmount))
            {
                return heldAmount.Value > 0;
            }
            return false;
            }
        }
        protected override void Start()
        {
            base.Start();
            if (GraphAgent.GetVariable("GatherSuppliesEvent",
                    out BlackboardVariable<GatherSuppliesEventChannel> eventChannelVariable))
            {
                eventChannelVariable.Value.Event += HandleGatherSupplies;
            }
        }
        
        public void Gather(GatherableSupply supply)
        {
            GraphAgent.SetVariableValue("GatherableSupply", supply);
            GraphAgent.SetVariableValue("Command", UnitCommands.Gather);
        }

        public void ReturnSupplies(GameObject commandPost)
        {
            GraphAgent.SetVariableValue("CommandPost", commandPost);
            GraphAgent.SetVariableValue("Command", UnitCommands.ReturnSupplies);
        }

        public GameObject Build(BuildingSO building, Vector3 targetLocation)
        {
            GameObject instance = Instantiate(building.Prefab, targetLocation, Quaternion.identity);
            
            if (instance.TryGetComponent(out BaseBuilding baseBuilding))
            {
                baseBuilding.ShowGhostVisuals();
            }
            else
            {
                Debug.LogError($"Missing BaseBuilding on Prefab for BuildingSO \"{building.name}\"! Cannot build!");
                return null;
            }
            
            GraphAgent.SetVariableValue("Ghost", instance);
            GraphAgent.SetVariableValue("BuildingSO", building);
            GraphAgent.SetVariableValue("TargetLocation", targetLocation);
            GraphAgent.SetVariableValue("Command", UnitCommands.BuildBuilding);
            
            return instance;
        }

        private void HandleGatherSupplies(GameObject self, int amount, SupplySO supplySO)
        {
            Bus<SupplyEvent>.Raise(new SupplyEvent(amount, supplySO));
        }
    }
}
