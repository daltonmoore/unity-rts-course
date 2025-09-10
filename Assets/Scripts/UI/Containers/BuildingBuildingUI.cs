using System.Collections;
using UI.Components;
using Units;
using UnityEngine;

namespace UI.Containers
{
    public class BuildingBuildingUI : MonoBehaviour, IUIElement<BaseBuilding>
    {
        [SerializeField] private ProgressBar progressBar;
        
        private Coroutine _buildCoroutine;
        private BaseBuilding _building;
        
        public void EnableFor(BaseBuilding item)
        {
            gameObject.SetActive(true);
            _building = item;
            _building.OnQueueUpdated += HandleQueueUpdated;

            _buildCoroutine = StartCoroutine(UpdateUnitProgress());
        }

        private void HandleQueueUpdated(UnitSO[] unitsInQueue)
        {
            if (unitsInQueue.Length == 1 && _buildCoroutine == null)
            {
                _buildCoroutine = StartCoroutine(UpdateUnitProgress());
            }
        }

        public void Disable()
        {
            if (_building is not null)
            {
                _building.OnQueueUpdated -= HandleQueueUpdated;
            }
            gameObject.SetActive(false);
            _building = null;
            _buildCoroutine = null;
        }
        
        private IEnumerator UpdateUnitProgress()
        {
            while (_building is not null && _building.QueueSize > 0)
            {
                float startTime = _building.CurrentQueueStartTime;
                float endTime = startTime + _building.BuildingUnit.BuildTime;
                float progress = Mathf.Clamp01((Time.time - startTime) / (endTime - startTime));

                progressBar.SetProgress(progress);
                
                yield return null;
            }
        }
    }
}