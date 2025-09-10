using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class BaseBuilding : AbstractCommandable
    {
        public int QueueSize => _buildingQueue.Count;
        [field: SerializeField] public float CurrentQueueStartTime { get; private set; }
        [field: SerializeField] public UnitSO BuildingUnit { get; private set; }

        public delegate void QueueUpdatedEvent(UnitSO[] unitsInQueue);
        public event QueueUpdatedEvent OnQueueUpdated;
        
        private Queue<UnitSO> _buildingQueue = new (MAX_QUEUE_SIZE);

        private const int MAX_QUEUE_SIZE = 5;

        public void BuildUnit(UnitSO unit)
        {
            if (_buildingQueue.Count == MAX_QUEUE_SIZE)
            {
                Debug.LogError("BuildUnit called when the queue was already full! This is not supported!");
                return;
            }
            
            _buildingQueue.Enqueue(unit);
            
            if (_buildingQueue.Count == 1)
            {
                StartCoroutine(DoBuildUnit());
            }
            else
            {
                OnQueueUpdated?.Invoke(_buildingQueue.ToArray());
            }
        }

        private IEnumerator DoBuildUnit()
        {
            while (_buildingQueue.Count > 0)
            {
                BuildingUnit = _buildingQueue.Peek();
                CurrentQueueStartTime = Time.time;
                OnQueueUpdated?.Invoke(_buildingQueue.ToArray());
                
                yield return new WaitForSeconds(BuildingUnit.BuildTime);
                
                Instantiate(BuildingUnit.Prefab, transform.position, Quaternion.identity);
                _buildingQueue.Dequeue();
            }
            
            OnQueueUpdated?.Invoke(_buildingQueue.ToArray());
        }
    }
}