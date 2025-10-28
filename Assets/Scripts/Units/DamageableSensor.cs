using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(SphereCollider))]
    public class DamageableSensor : MonoBehaviour
    {
        public List<IDamageable> Damageables => _damageables.ToList();

        public delegate void UnitDetectionEvent(IDamageable damageable);

        public event UnitDetectionEvent OnUnitEnter;
        public event UnitDetectionEvent OnUnitExit;

        private SphereCollider _collider;
        private HashSet<IDamageable> _damageables = new();

        private void Awake()
        {
            _collider = GetComponent<SphereCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable damageable)) return;
            _damageables.Add(damageable);
            OnUnitEnter?.Invoke(damageable);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable damageable)) return;
            _damageables.Remove(damageable);
            OnUnitExit?.Invoke(damageable);
        }

        public void SetupFrom(AttackConfigSO attackConfig)
        {
            _collider.radius = attackConfig.AttackRange;
        }
    }
}
