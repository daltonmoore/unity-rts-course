
using Behavior;
using Environment;
using EventBus;
using Events;
using Unity.Behavior;
using UnityEngine;

namespace Units
{
    public class Worker : AbstractUnit
    {
        protected override void Start()
        {
            base.Start();
            if (GraphAgent.GetVariable("GatherSuppliesEvent",
                    out BlackboardVariable<GatherSuppliesEventChannel> eventChannelVariable))
            {
                eventChannelVariable.Value.Event += HandleGatherSupplies;
            }
        }

        private void HandleGatherSupplies(GameObject self, int amount, SupplySO supplySO)
        {
            Bus<SupplyEvent>.Raise(new SupplyEvent(amount, supplySO));
        }

        public void Gather(GatherableSupply supply)
        {
            GraphAgent.SetVariableValue("GatherableSupply", supply);
            GraphAgent.SetVariableValue("Command", UnitCommands.Gather);
        }
    }
}
