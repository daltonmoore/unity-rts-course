using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Pick Closest Point on Collider", story: "Set [TargetLocation] to the closest point to [Target] on [Collider]", category: "Action", id: "5e307b0a646d69b7803e8215ef50bf78")]
    public partial class PickClosestPointOnColliderAction : Action
    {
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<GameObject> Collider;

        protected override Status OnStart()
        {
            if (Target.Value == null || Collider.Value == null || !Collider.Value.TryGetComponent(out Collider collider))
                return Status.Failure;
        
            TargetLocation.Value = collider.ClosestPoint(Target.Value.transform.position);
            
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = TargetLocation.Value;
            go.transform.localScale = Vector3.one * .5f;
            
            return Status.Success;
        }
    }
}

