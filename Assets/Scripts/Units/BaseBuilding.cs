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
                StartCoroutine(DoBuildUnit(unit));
            }
        }

        private IEnumerator DoBuildUnit(UnitSO unit)
        {
            yield return new WaitForSeconds(unit.BuildTime);
            Instantiate(unit.Prefab, transform.position, Quaternion.identity);
        }
    }
}