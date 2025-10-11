using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using EventBus;
using Events;
using Units;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private Rigidbody cameraTarget;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private new Camera camera;
        [SerializeField] private CameraConfig cameraConfig;
        [SerializeField] private LayerMask selectableUnitsLayers;
        [SerializeField] private LayerMask floorLayers;
        [FormerlySerializedAs("gatherableSuppliesLayers")] [SerializeField] private LayerMask interactableLayers;
        [SerializeField] private RectTransform selectionBox;
        [SerializeField] private Texture2D cursorTexture;

        private Vector2 _startingMousePosition;

        private ActionBase _activeAction;
        private GameObject _ghostInstance;
        private bool _wasMouseDownOnUI;
        private CinemachineFollow _cinemachineFollow;
        private float _zoomStartTime;
        private float _rotationStartTime;
        private Vector3 _startingFollowOffset;
        private float _maxRotationAmount;
        private HashSet<AbstractUnit> _aliveUnits = new (100);
        private HashSet<AbstractUnit> _addedUnits = new (24);
        private List<ISelectable> _selectedUnits = new (12);

        private void Awake()
        {
            if (!cinemachineCamera.TryGetComponent(out _cinemachineFollow))
            {
                Debug.LogError("Cinemachine Camera did not have CinemachineFollow component. Zoom functionality will not work!");
            }

            _startingFollowOffset = _cinemachineFollow.FollowOffset;
            _maxRotationAmount = Mathf.Abs(_cinemachineFollow.FollowOffset.z);
            // Cursor.lockState = CursorLockMode.Confined;
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
            
            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
            Bus<UnitSpawnEvent>.OnEvent += HandleUnitSpawn;
            Bus<ActionSelectedEvent>.OnEvent += HandleActionSelected;
        }

        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
            Bus<UnitSpawnEvent>.OnEvent -= HandleUnitSpawn;
            Bus<ActionSelectedEvent>.OnEvent -= HandleActionSelected;
        }

        private void Update()
        {
            HandlePanning();
            HandleZooming();
            HandleRotation();
            HandleRightClick();
            HandleDragSelect();
            HandleGhostPrefab();
        }

        private void HandleGhostPrefab()
        {
            if (_ghostInstance == null) return;

            if (Keyboard.current.escapeKey.wasReleasedThisFrame)
            {
                Destroy(_ghostInstance);
                _ghostInstance = null;
                _activeAction = null;
                return;
            }
            
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, floorLayers))
            {
                _ghostInstance.transform.position = hit.point;
            }
        }

        private void HandleActionSelected(ActionSelectedEvent evt)
        {
            _activeAction = evt.Action;

            if (!_activeAction.RequiresClickToActivate)
            {
                ActivateAction(new RaycastHit());
            }
            else if (_activeAction.GhostPrefab != null)
            {
                _ghostInstance = Instantiate(_activeAction.GhostPrefab);
            }
        }

        private void HandleUnitSpawn(UnitSpawnEvent evt) => _aliveUnits.Add(evt.Unit);

        private void HandleUnitSelected(UnitSelectedEvent evt)
        {
            if (!_selectedUnits.Contains(evt.Unit))
            {
                _selectedUnits.Add(evt.Unit);
            }
        }

        private void HandleUnitDeselected(UnitDeselectedEvent evt) => _selectedUnits.Remove(evt.Unit);

        private void HandleDragSelect()
        {
            if (selectionBox == null) { return; }
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleMouseDown();
            }
            else if (Mouse.current.leftButton.isPressed && !Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleMouseDrag();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                HandleMouseUp();
            }
        }

        private void HandleMouseUp()
        {
            if (!_wasMouseDownOnUI && _activeAction is null && !Keyboard.current.shiftKey.isPressed)
            {
                DeselectAllUnits();
            }

            HandleLeftClick();
            foreach (AbstractUnit unit in _addedUnits)
            {
                unit.Select();
            }
            selectionBox.gameObject.SetActive(false);
        }

        private void HandleMouseDrag()
        {
            if (_activeAction is not null || _wasMouseDownOnUI) { return; }
            
            Bounds selectionBoxBounds = ResizeSelectionBox();

            foreach (AbstractUnit unit in _aliveUnits)
            {
                Vector2 unitPosition = camera.WorldToScreenPoint(unit.transform.position);
                if (selectionBoxBounds.Contains(unitPosition))
                {
                    _addedUnits.Add(unit);
                }
            }
        }

        private void HandleMouseDown()
        {
            selectionBox.sizeDelta = Vector2.zero;
            selectionBox.gameObject.SetActive(true);
            _startingMousePosition = Mouse.current.position.ReadValue();
            _addedUnits.Clear();
            _wasMouseDownOnUI = EventSystem.current.IsPointerOverGameObject();
        }

        private void DeselectAllUnits()
        {
            ISelectable[] currentlySelectedUnits = _selectedUnits.ToArray();

            foreach (ISelectable selectable in currentlySelectedUnits)
            {
                selectable.Deselect();
            }
        }

        private Bounds ResizeSelectionBox()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
                
            float width = mousePosition.x - _startingMousePosition.x;
            float height = mousePosition.y - _startingMousePosition.y;

            selectionBox.anchoredPosition = _startingMousePosition + new Vector2(width / 2, height / 2);
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
            
            return new Bounds(selectionBox.anchoredPosition, selectionBox.sizeDelta);
        }

        private void HandleRightClick()
        {
            if (_selectedUnits.Count == 0) { return; }
            
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Mouse.current.rightButton.wasReleasedThisFrame
                && Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, interactableLayers | floorLayers))
            {
                List<AbstractUnit> abstractUnits = new (_selectedUnits.Count);
                foreach (ISelectable selectable in _selectedUnits)
                {
                    if (selectable is AbstractUnit unit)
                    {
                        abstractUnits.Add(unit);
                    }
                }

                for (int i = 0; i < abstractUnits.Count; i++)
                {
                    CommandContext context = new(abstractUnits[i], hit, i);
                    foreach (ICommand command in abstractUnits[i].AvailableCommands)
                    {
                        if (command.CanHandle(context))
                        {
                            command.Handle(context);
                            break;
                        }
                    }
                }
            }
        }

        private void HandleLeftClick()
        {
            if (camera == null) { return; } 
            
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            
            if (_activeAction is null 
                && Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, selectableUnitsLayers) 
                && hit.collider.TryGetComponent(out ISelectable selectable))
            {
                selectable.Select();
            }
            else if (_activeAction is not null 
                     && !EventSystem.current.IsPointerOverGameObject()
                     && Physics.Raycast(ray, out hit, float.MaxValue, interactableLayers | floorLayers))
            {
                ActivateAction(hit);
            }
            
        }

        private void ActivateAction(RaycastHit hit)
        {
            if (_ghostInstance != null)
            {
                Destroy(_ghostInstance);
                _ghostInstance = null;
            }
            List<AbstractCommandable> abstractCommandables = _selectedUnits
                .Where((commandable) => commandable is AbstractCommandable)
                .Cast<AbstractCommandable>()
                .ToList();

            for (int i = 0; i < abstractCommandables.Count; i++)
            {
                CommandContext context = new(abstractCommandables[i], hit, i);
                if (_activeAction.CanHandle(context))
                {
                    _activeAction.Handle(context);
                }
            }
                
            _activeAction = null;
        }

        private void HandleRotation()
        {
            if (ShouldSetRotationStartTime())
            {
                _rotationStartTime = Time.time;
            }
        
            float rotationTime = Mathf.Clamp01((Time.time - _rotationStartTime) * cameraConfig.RotationSpeed);
        
            Vector3 targetFollowOffset;

            if (Keyboard.current.pageDownKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                    _maxRotationAmount,
                    _cinemachineFollow.FollowOffset.y,
                    0
                );
            }
            else if (Keyboard.current.pageUpKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                    -_maxRotationAmount,
                    _cinemachineFollow.FollowOffset.y,
                    0
                );
            }
            else
            {
                targetFollowOffset = new Vector3(
                    _startingFollowOffset.x,
                    _cinemachineFollow.FollowOffset.y,
                    _startingFollowOffset.z
                );
            }

            _cinemachineFollow.FollowOffset = Vector3.Slerp(_cinemachineFollow.FollowOffset,
                targetFollowOffset,
                rotationTime);
        }

        private bool ShouldSetRotationStartTime()
        {
            return Keyboard.current.pageUpKey.wasPressedThisFrame 
                   || Keyboard.current.pageDownKey.wasPressedThisFrame
                   || Keyboard.current.pageUpKey.wasReleasedThisFrame
                   || Keyboard.current.pageDownKey.wasReleasedThisFrame;
        }

        private void HandleZooming()
        {
            if (ShouldSetZoomStartTime())
            {
                _zoomStartTime = Time.time;
            }

            float zoomTime = Mathf.Clamp01((Time.time - _zoomStartTime) * cameraConfig.ZoomSpeed);
            Vector3 targetFollowOffset;
        
            if (Keyboard.current.endKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                    _cinemachineFollow.FollowOffset.x,
                    cameraConfig.MinZoomDistance,
                    _cinemachineFollow.FollowOffset.z
                );
            }
            else
            {
                targetFollowOffset = new Vector3(
                    _cinemachineFollow.FollowOffset.x,
                    _startingFollowOffset.y,
                    _cinemachineFollow.FollowOffset.z
                );
            }
        
            _cinemachineFollow.FollowOffset = Vector3.Slerp(
                _cinemachineFollow.FollowOffset,
                targetFollowOffset,
                zoomTime
            );
        }

        private bool ShouldSetZoomStartTime()
        {
            return Keyboard.current.endKey.wasPressedThisFrame 
                   || Keyboard.current.endKey.wasReleasedThisFrame;
        }

        private void HandlePanning()
        {
            Vector2 moveAmount = GetKeyboardMoveAmount();
            moveAmount += GetMouseMoveAmount();
            cameraTarget.linearVelocity = new Vector3(moveAmount.x, 0, moveAmount.y);
        }

        private Vector2 GetMouseMoveAmount()
        {
            Vector2 moveAmount = Vector2.zero;
            
            if (!cameraConfig.EnableEdgePan) { return moveAmount; }
            
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            if (mousePosition.x <= cameraConfig.EdgePanSize)
            {
                moveAmount.x -= cameraConfig.MousePanSpeed;
            }
            else if (mousePosition.x >= screenWidth - cameraConfig.EdgePanSize)
            {
                moveAmount.x += cameraConfig.MousePanSpeed;
            }

            if (mousePosition.y >= screenHeight - cameraConfig.EdgePanSize)
            {
                moveAmount.y += cameraConfig.MousePanSpeed;
            }
            else if (mousePosition.y <= cameraConfig.EdgePanSize)
            {
                moveAmount.y -= cameraConfig.MousePanSpeed;           
            }
            
            return moveAmount;
        }

        private Vector2 GetKeyboardMoveAmount()
        {
            Vector2 moveAmount = Vector2.zero;
            
            if (Keyboard.current.upArrowKey.isPressed)
            {
                moveAmount.y += cameraConfig.KeyboardPanSpeed;
            }
        
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                moveAmount.x += cameraConfig.KeyboardPanSpeed;
            }
        
            if (Keyboard.current.leftArrowKey.isPressed)
            {
                moveAmount.x -= cameraConfig.KeyboardPanSpeed;
            }

            if (Keyboard.current.downArrowKey.isPressed)
            {
                moveAmount.y -= cameraConfig.KeyboardPanSpeed;
            }

            return moveAmount;
        }
    }
}