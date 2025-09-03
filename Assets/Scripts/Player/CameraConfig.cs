using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class CameraConfig
    { 
        [field: SerializeField] public bool enableEdgePan { get; private set; } = true;
        [field: SerializeField] public float mousePanSpeed { get; private set; } = 10f;
        [field: SerializeField] public float edgePanSize { get; private set; } = 50f;
        
        [field: SerializeField] public float keyboardPanSpeed { get; private set; } = 10f;
        
        [field: SerializeField] public float minZoomDistance { get; private set; } = 7.5f;
        [field: SerializeField] public float zoomSpeed { get; private set; } = 1f;
        
        [field: SerializeField] public float rotationSpeed { get; private set; } = 1f;
    }
}