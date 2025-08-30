using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private float keyboardPanSpeed = 10f;
    [SerializeField] private float minZoomDistance = 7.5f;
    [SerializeField] private float zoomSpeed = 1f;

    private CinemachineFollow _cinemachineFollow;
    private float _zoomStartTime;
    private Vector3 _startingFollowOffset;

    private void Awake()
    {
        if (!cinemachineCamera.TryGetComponent(out _cinemachineFollow))
        {
            Debug.LogError("Cinemachine Camera did not have CinemachineFollow component. Zoom functionality will not work!");
        }

        _startingFollowOffset = _cinemachineFollow.FollowOffset;
    }

    void Update()
    {
        HandlePanning();
        HandleZooming();
    }

    private void HandleZooming()
    {
        if (Keyboard.current.endKey.wasPressedThisFrame)
        {
            _zoomStartTime = Time.time;
        }
        
        Vector3 targetFollowOffset = new (
            _cinemachineFollow.FollowOffset.x,
            0,
            _cinemachineFollow.FollowOffset.z
        );
    }

    private void HandlePanning()
    {
        Vector2 moveAmount = Vector2.zero;

        if (Keyboard.current.upArrowKey.isPressed)
        {
            moveAmount.y += keyboardPanSpeed;
        }
        
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            moveAmount.x += keyboardPanSpeed;
        }
        
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            moveAmount.x -= keyboardPanSpeed;
        }

        if (Keyboard.current.downArrowKey.isPressed)
        {
            moveAmount.y -= keyboardPanSpeed;
        }
        moveAmount *= Time.deltaTime;
        cameraTarget.position += new Vector3(moveAmount.x, 0, moveAmount.y);
    }
}
