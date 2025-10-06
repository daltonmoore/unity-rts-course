
using Environment;
using Unity.Behavior;

namespace Units
{
    public class Worker : AbstractUnit
    { 
        
        public void Gather(GatherableSupply supply)
        {
            GraphAgent.SetVariableValue("GatherableSupply", supply);
            GraphAgent.SetVariableValue("Command", UnitCommands.Gather);
        }
    }
}
