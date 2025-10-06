using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Utilities
{
    public class ClosestColliderComparer : IComparer<Collider>
    {
        private Vector3 _targetPosition;

        public ClosestColliderComparer(Vector3 position)
        {
            _targetPosition = position;
        }
        
        public int Compare(Collider x, Collider y)
        {
            Debug.Assert(x is not null, nameof(x) + " is not null");
            Debug.Assert(y is not null, nameof(y) + " is not null");
            return (x.transform.position - _targetPosition).sqrMagnitude
                .CompareTo((y.transform.position - _targetPosition).sqrMagnitude);
        }
    }
}