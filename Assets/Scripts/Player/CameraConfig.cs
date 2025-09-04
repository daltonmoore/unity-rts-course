using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [System.Serializable]
    public class CameraConfig
    {
        // Mouse
        [field: FormerlySerializedAs("<mousePanSpeed>k__BackingField")] 
        [field: SerializeField] public float MousePanSpeed { get; private set; } = 10f;
        
        // Pan
        [field: FormerlySerializedAs("<enableEdgePan>k__BackingField")] 
        [field: SerializeField] public bool EnableEdgePan { get; private set; } = true;
        
        [field: FormerlySerializedAs("<edgePanSize>k__BackingField")] 
        [field: SerializeField] public float EdgePanSize { get; private set; } = 50f;

        [field: FormerlySerializedAs("<keyboardPanSpeed>k__BackingField")] 
        [field: SerializeField] public float KeyboardPanSpeed { get; private set; } = 10f;

        // Zoom
        [field: FormerlySerializedAs("<minZoomDistance>k__BackingField")] 
        [field: SerializeField] public float MinZoomDistance { get; private set; } = 7.5f;
        
        [field: FormerlySerializedAs("<zoomSpeed>k__BackingField")] 
        [field: SerializeField] public float ZoomSpeed { get; private set; } = 1f;

        // Rotation
        [field: FormerlySerializedAs("<rotationSpeed>k__BackingField")] 
        [field: SerializeField] public float RotationSpeed { get; private set; } = 1f;
    }
}