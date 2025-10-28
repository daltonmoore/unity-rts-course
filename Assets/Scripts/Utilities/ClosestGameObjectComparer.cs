using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Utilities
{
    public class ClosestGameObjectComparer : IComparer<GameObject>
    {
        private Vector3 _targetPosition;

        public ClosestGameObjectComparer(Vector3 position)
        {
            _targetPosition = position;
        }
        
        public int Compare(GameObject x, GameObject y)
        {
            Debug.Assert(x is not null, nameof(x) + " is not null");
            Debug.Assert(y is not null, nameof(y) + " is not null");
            return (x.transform.position - _targetPosition).sqrMagnitude
                .CompareTo((y.transform.position - _targetPosition).sqrMagnitude);
        }
    }
}