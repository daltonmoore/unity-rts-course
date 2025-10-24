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
            }
        }

        public void Disable()
        {
            buildingUnderConstructionUI.Disable();
            buildingBuildingUI.Disable();
            singleUnitSelectedUI.Disable();
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
    }
}