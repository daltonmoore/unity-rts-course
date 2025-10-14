using System;
using System.Collections.Generic;
using System.Linq;
using EventBus;
using Events;
using UI.Containers;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
    public class RuntimeUI : MonoBehaviour
    {
        [SerializeField] public ActionsUI actionsUI; 
        [SerializeField] public BuildingBuildingUI buildingBuildingUI;
        
        private static RuntimeUI _instance;
        private HashSet<AbstractCommandable> _commandables = new (12);
        
        private void Awake()
        {
            _instance = this;
            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
            Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
        }

        private void Start()
        {
            actionsUI.Disable();
            buildingBuildingUI.Disable();
        }

        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
        }

        private void HandleUnitSelected(UnitSelectedEvent evt)
        {
            if (evt.Unit is AbstractCommandable commandable)
            {
                _commandables.Add(commandable);
                actionsUI.EnableFor(_commandables);
            }

            if (_commandables.Count == 1 
                && evt.Unit is BaseBuilding building)
            {
                buildingBuildingUI.EnableFor(building);
            }
        }
        
        private void HandleUnitDeselected(UnitDeselectedEvent evt)
        {
            if (evt.Unit is AbstractCommandable commandable)
            {
                _commandables.Remove(commandable);

                RefreshUI();
            }
        }

        private void RefreshUI()
        {
            if (_commandables.Count > 0)
            {
                actionsUI.EnableFor(_commandables);
                    
                if (_commandables.Count == 1 
                    && _commandables.First() is BaseBuilding building)
                {
                    buildingBuildingUI.EnableFor(building);
                }
                else
                {
                    buildingBuildingUI.Disable();   
                }
            }

            if (_commandables.Count == 0)
            {
                actionsUI.Disable();
                buildingBuildingUI.Disable();
            }
        }

        private void HandleUnitDeath(UnitDeathEvent evt)
        {
            _commandables.Remove(evt.Unit);

            RefreshUI();
        }
        
        public static bool IsPointerOverCanvas()
        {
            PointerEventData eventData = new(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.transform.IsChildOf(_instance.transform))
                {
                    return true; // Pointer is over an element within the target canvas
                }
            }
            return false; // Pointer is not over any element within the target canvas
        }
    }
}