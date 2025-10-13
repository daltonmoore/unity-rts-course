using System;
using Units;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "PickRandomLocationWithinRendererBounds", story: "Set [TargetLocation] to a random point within [BuildingUnderConstruction]", category: "Action", id: "ce5da7a6d193a8a2db118819e53e4871")]
    public partial class PickRandomLocationWithinRendererBoundsAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<BaseBuilding> BuildingUnderConstruction;

        protected override Status OnStart()
        {
            if (BuildingUnderConstruction.Value == null || BuildingUnderConstruction.Value.MainRenderer == null) 
                return Status.Failure;

            Renderer renderer = BuildingUnderConstruction.Value.MainRenderer;
            
            Bounds bounds = renderer.bounds;

            TargetLocation.Value = new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                TargetLocation.Value.y,
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
            );
            
            return Status.Success;
        }
    }
}

