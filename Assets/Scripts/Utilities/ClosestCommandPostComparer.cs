using System.Collections.Generic;
using Units;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Utilities
{
    public class ClosestCommandPostComparer : IComparer<BaseBuilding>
    {
        private Vector3 _targetPosition;

        public ClosestCommandPostComparer(Vector3 position)
        {
            _targetPosition = position;
        }
        
        public int Compare(BaseBuilding x, BaseBuilding y)
        {
            Debug.Assert(x is not null, nameof(x) + " is not null");
            Debug.Assert(y is not null, nameof(y) + " is not null");
            return (x.transform.position - _targetPosition).sqrMagnitude
                .CompareTo((y.transform.position - _targetPosition).sqrMagnitude);
        }
    }
}