using System;
using Units;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Serialization;

namespace Behavior
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "Building Is In Progress", story: "[BaseBuilding] is being built", category: "Conditions", id: "196e9de395c110cd795e2f5e0962589b")]
    public partial class BuildingIsInProgressCondition : Condition
    {
        [FormerlySerializedAs("BaseBuilding")] [SerializeReference] public BlackboardVariable<BaseBuilding> baseBuilding;

        public override bool IsTrue()
        {
            return baseBuilding.Value != null 
                   && baseBuilding.Value.Progress.State == BuildingProgress.BuildingState.Building;
        }
    }
}
