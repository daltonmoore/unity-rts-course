using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units
{
    public class BaseBuilding : AbstractCommandable
    {
        public int QueueSize => _buildQueue.Count;
        public UnitSO[] BuildingQueue => _buildQueue.ToArray();
        
        [field: SerializeField] public float CurrentQueueStartTime { get; private set; }
        [field: SerializeField] public UnitSO BuildingUnit { get; private set; }

        public delegate void QueueUpdatedEvent(UnitSO[] unitsInQueue);
        public event QueueUpdatedEvent OnQueueUpdated;
        
        private List<UnitSO> _buildQueue = new (MAX_QUEUE_SIZE);

        private const int MAX_QUEUE_SIZE = 5;

        public void BuildUnit(UnitSO unit)
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