using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventBus;
using Events;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public class BaseBuilding : AbstractCommandable
    {
        public int QueueSize => _buildQueue.Count;
        public AbstractUnitSO[] BuildingQueue => _buildQueue.ToArray();
        [field: SerializeField] public float CurrentQueueStartTime { get; private set; }
        [field: SerializeField] public AbstractUnitSO BuildingUnit { get; private set; }
        [field: SerializeField] public MeshRenderer MainRenderer { get; private set; }
        [SerializeField] private Material primaryMaterial;
        [SerializeField] private NavMeshObstacle navMeshObstacle;
        [field: SerializeField] public BuildingProgress Progress { get; private set; } = new (
            0, 0, BuildingProgress.BuildingState.Destroyed);

        public delegate void QueueUpdatedEvent(AbstractUnitSO[] unitsInQueue);
        public event QueueUpdatedEvent OnQueueUpdated;
        
        private List<AbstractUnitSO> _buildQueue = new (MAX_QUEUE_SIZE);
        private BuildingSO _buildingSO;
        private IBuildingBuilder _unitBuildingThis;

        private const int MAX_QUEUE_SIZE = 5;

        private void Awake()
        {
            _buildingSO = UnitSO as BuildingSO;
        }

        protected override void Start()
        {
            base.Start();
            if (MainRenderer != null)
            {
                MainRenderer.material = primaryMaterial;
            }
            
            Progress = new BuildingProgress(Progress.StartTime, 1, BuildingProgress.BuildingState.Completed);
            _unitBuildingThis = null;
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
        }

        public void BuildUnit(AbstractUnitSO unit)
        {
            if (_buildQueue.Count == MAX_QUEUE_SIZE)
            {
                Debug.LogError("BuildUnit called when the queue was already full! This is not supported!");
                return;
            }
            
            _buildQueue.Add(unit);
            
            if (_buildQueue.Count == 1)
            {
                StartCoroutine(DoBuildUnit());
            }
            else
            {
                OnQueueUpdated?.Invoke(_buildQueue.ToArray());
            }
        }

        public void CancelBuild(int index)
        {
            if (index < 0 || index >= _buildQueue.Count)
            {
                Debug.LogError("CancelBuild called with invalid index!");
                return;
            }
            
            _buildQueue.RemoveAt(index);
            if (index == 0)
            {
                StopAllCoroutines();
                if (_buildQueue.Count > 0)
                {
                    StartCoroutine(DoBuildUnit());
                }
                else
                {
                    OnQueueUpdated?.Invoke(_buildQueue.ToArray());
                }
            }
            else
            {
                OnQueueUpdated?.Invoke(_buildQueue.ToArray());
            }
        }
        
        public void StartBuilding(IBuildingBuilder builder)
        {
            _unitBuildingThis = builder;
            MainRenderer.material = _buildingSO.PlacementMaterial;
            
            Progress = new BuildingProgress(
                Time.time - _buildingSO.BuildTime * Progress.Progress,
                Progress.Progress,
                BuildingProgress.BuildingState.Building);

            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
            Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
        }

        private void HandleUnitDeath(UnitDeathEvent evt)
        {
            if (evt.Unit.TryGetComponent(out IBuildingBuilder builder) && builder == _unitBuildingThis)
            {

                Progress = new BuildingProgress(
                    Progress.StartTime,
                    (Time.time - Progress.StartTime) / _buildingSO.BuildTime,
                    BuildingProgress.BuildingState.Paused);
                
                Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
            }
        }

        public void ResetDefaultVisuals()
        {
            MainRenderer.material = _buildingSO.DefaultMaterial;
        }

        private IEnumerator DoBuildUnit()
        {
            while (_buildQueue.Count > 0)
            {
                BuildingUnit = _buildQueue[0];
                CurrentQueueStartTime = Time.time;
                OnQueueUpdated?.Invoke(_buildQueue.ToArray());
                
                yield return new WaitForSeconds(BuildingUnit.BuildTime);
                
                Instantiate(BuildingUnit.Prefab, transform.position, Quaternion.identity);
                _buildQueue.RemoveAt(0);
            }
            
            OnQueueUpdated?.Invoke(_buildQueue.ToArray());
        }

        private void OnDestroy()
        {
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
        }
    }
}