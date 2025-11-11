using System;
using System.Collections.Generic;
using System.Linq;
using EventBus;
using Events;
using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(SphereCollider))]
    public class DamageableSensor : MonoBehaviour
    {
        public List<IDamageable> Damageables => _damageables.ToList();
        [field: SerializeField] public Owner Owner { get; set; }

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
            if (other.TryGetComponent(out IDamageable damageable) && damageable.Owner != Owner)
            {
                _damageables.Add(damageable);
                OnUnitEnter?.Invoke(damageable);
            }
            
            if (_damageables.Count == 1)
            {
                Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable) && _damageables.Remove(damageable))
            {
                OnUnitExit?.Invoke(damageable);
            }

            if (_damageables.Count == 0)
            {
                Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
            }
        }

        private void OnDestroy()
        {
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
        }

        public void SetupFrom(AttackConfigSO attackConfig)
        {
            _collider.radius = attackConfig.AttackRange;
        }
        
        private void HandleUnitDeath(UnitDeathEvent evt)
        {
            if (_damageables.Contains(evt.Unit))
            {
                OnTriggerExit(evt.Unit.GetComponent<Collider>());
            }
        }
    }
}
