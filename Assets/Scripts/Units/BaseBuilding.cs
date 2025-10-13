using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private NavMeshObstacle navMeshObstacle;

        public delegate void QueueUpdatedEvent(AbstractUnitSO[] unitsInQueue);
        public event QueueUpdatedEvent OnQueueUpdated;
        
        private List<AbstractUnitSO> _buildQueue = new (MAX_QUEUE_SIZE);
        private BuildingSO _buildingSO;

        private const int MAX_QUEUE_SIZE = 5;

        private void Awake()
        {
            _buildingSO = UnitSO as BuildingSO;
        }

        protected override void Start()
        {
            base.Start();
            if (navMeshObstacle != null)
            {
                navMeshObstacle.enabled = true;
            }
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
        
        public void ShowGhostVisuals()
        {
            MainRenderer.material = _buildingSO.PlacementMaterial;
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
    }
}