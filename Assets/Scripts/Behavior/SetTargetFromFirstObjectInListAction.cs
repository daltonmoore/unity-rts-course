using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Set Target from First Object in List", story: "Set [Target] to the first item in [List]", category: "Action/Blackboard", id: "90b44be05664e1d7dd1ce7ec331d3e05")]
    public partial class SetTargetFromFirstObjectInListAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<List<GameObject>> List;

        protected override Status OnStart()
        {
            if (List.Value == null || List.Value.Count == 0) return Status.Failure;
            
            Target.Value = List.Value[0];
            return Status.Success;
        }
    }
}

