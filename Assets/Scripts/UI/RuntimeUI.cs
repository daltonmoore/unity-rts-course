using System;
using System.Collections.Generic;
using System.Linq;
using EventBus;
using Events;
using UI.Containers;
using Units;
using UnityEngine;

namespace UI
{
    public class RuntimeUI : MonoBehaviour
    {
        [SerializeField] public ActionsUI actionsUI; 
        [SerializeField] public BuildingBuildingUI buildingBuildingUI;
        
        private HashSet<AbstractCommandable> _commandables = new (12);
        
        private void Awake()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
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
        }
    }
}