using System;
using EventBus;
using Events;
using Units;
using UnityEngine;

namespace UI.Containers
{
    public class BuildingSelectedUI : MonoBehaviour, IUIElement<BaseBuilding>
    {
        [SerializeField] BuildingBuildingUI buildingBuildingUI;
        [SerializeField] BuildingUnderConstructionUI buildingUnderConstructionUI;
        [SerializeField] SingleUnitSelectedUI singleUnitSelectedUI;
        
        private BaseBuilding _selectedBuilding;

        public void EnableFor(BaseBuilding building)
        {
            _selectedBuilding = building;
            _selectedBuilding.OnQueueUpdated -= OnBuildingQueueUpdated;
            _selectedBuilding.OnQueueUpdated += OnBuildingQueueUpdated;
            

            if (building.Progress.State == BuildingProgress.BuildingState.Completed)
            {
                buildingUnderConstructionUI.Disable();
                OnBuildingQueueUpdated();
            }
            else
            {
                buildingUnderConstructionUI.EnableFor(building);
                buildingBuildingUI.Disable();
                singleUnitSelectedUI.Disable();
                Bus<BuildingSpawnEvent>.OnEvent += HandleBuildingSpawned;
            }
        }

        public void Disable()
        {
            buildingUnderConstructionUI.Disable();
            buildingBuildingUI.Disable();
            Bus<BuildingSpawnEvent>.OnEvent -= HandleBuildingSpawned;
            if (_selectedBuilding != null)
            { 
                _selectedBuilding.OnQueueUpdated -= OnBuildingQueueUpdated;
                _selectedBuilding = null;
            }
        }
        
        private void OnBuildingQueueUpdated(AbstractUnitSO[] _ = null)
        {
            if (_selectedBuilding.QueueSize == 0)
            {
                singleUnitSelectedUI.EnableFor(_selectedBuilding);
                buildingBuildingUI.Disable();
            }
            else
            {
                buildingBuildingUI.EnableFor(_selectedBuilding);
                singleUnitSelectedUI.Disable();
            }
        }
        
        private void HandleBuildingSpawned(BuildingSpawnEvent evt)
        {
            if (evt.Building == _selectedBuilding)
            {
                Bus<BuildingSpawnEvent>.OnEvent -= HandleBuildingSpawned;
                OnBuildingQueueUpdated();
                buildingUnderConstructionUI.Disable();
            }
        }
    }
}