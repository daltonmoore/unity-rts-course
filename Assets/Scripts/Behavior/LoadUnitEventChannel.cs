using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

#if UNITY_EDITOR
namespace Behavior
{
    [CreateAssetMenu(menuName = "Behavior/Event Channels/Load Unit Event Channel")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "Load Unit Event Channel", message: "[Self] loads [TargetGameObject] into itself", category: "Events", id: "53a6d941e7f5e5fb4c6d7f387c1f1935")]
    public sealed partial class LoadUnitEventChannel : EventChannel<GameObject, GameObject> { }
}

