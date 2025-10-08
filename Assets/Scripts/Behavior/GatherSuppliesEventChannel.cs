using System;
using Environment;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

#if UNITY_EDITOR
namespace Behavior
{
    [CreateAssetMenu(menuName = "Behavior/Event Channels/GatherSuppliesEventChannel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "GatherSuppliesEventChannel", message: "[Self] gathers [Amount] [Supplies]", category: "Events", id: "100485cf301f65ea82211aa70abfc0e3")]
    public sealed partial class GatherSuppliesEventChannel : EventChannel<GameObject, int, SupplySO> { }
}

