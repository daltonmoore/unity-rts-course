using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Units
{
    public class BaseBuilding : AbstractCommandable
    {
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
        }

        private IEnumerator DoBuildUnit()
        {
            while (_buildingQueue.Count > 0)
            {
                yield return new WaitForSeconds(_buildingQueue.Peek().BuildTime);
                var unit = _buildingQueue.Dequeue();
                Instantiate(unit.Prefab, transform.position, Quaternion.identity);
            }
            
        }
    }
}