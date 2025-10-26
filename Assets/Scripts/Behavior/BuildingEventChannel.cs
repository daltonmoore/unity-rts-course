using System;
using Units;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

#if UNITY_EDITOR
namespace Behavior
{
    [CreateAssetMenu(menuName = "Behavior/Event Channels/Building Event Channel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "Building Event Channel", message: "[Self] [BuildingEventType] on [BaseBuilding]", category: "Events", id: "7a173b19c005ba45ab281964cef37e9c")]
    public sealed partial class BuildingEventChannel : EventChannel<GameObject, BuildingEventType, BaseBuilding> { }
}

