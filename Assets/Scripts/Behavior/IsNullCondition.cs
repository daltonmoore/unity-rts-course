using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Serialization;

namespace Behavior
{
    [Serializable, Unity.Properties.GeneratePropertyBag]
    [Condition(name: "IsNull", story: "[gameObject] is NULL", category: "Conditions", id: "2787a228e2bc53f5f34a51a5b8176251")]
    public partial class IsNullCondition : Condition
    {
        [SerializeReference] public BlackboardVariable<GameObject> gameObject;

        public override bool IsTrue()
        {
            if (gameObject.Type.IsValueType)
            {
                return false;
            }

            return gameObject.ObjectValue == null || gameObject.ObjectValue.Equals(null);
        }
    }
}
