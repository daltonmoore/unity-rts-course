using System.Collections;
using Unity.Behavior;
using UnityEngine;

namespace Units
{
    public class Grenadier : BaseMilitaryUnit
    {
        [SerializeField] private GameObject grenade;
        [SerializeField] private ParticleSystem explosionParticles;
        
        private Transform _grenadeParent;
        private Vector3 _defaultGrenadePosition;
        private Collider[] _splashHits;

        protected override void Awake()
        {
            base.Awake();
            if (grenade == null || explosionParticles == null)
            {
                Debug.Log($"Grenadier {name} is missing a grenade or explosionParticles");
                return;
            }

            _defaultGrenadePosition = grenade.transform.localPosition;
            _grenadeParent = grenade.transform.parent;
        }

        protected override void Start()
        {
            base.Start();
            _splashHits = new Collider[_unitSO.AttackConfig.MaxEnemiesHitPerAttack];
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Destroy(grenade);
            Destroy(explosionParticles.gameObject);
        }

        // Animation Event
        public void OnThrowGrenade()
        {
            grenade.transform.SetParent(null);
            Vector3 startPosition = grenade.transform.position;
            Vector3 endPosition = startPosition + transform.forward * 10;
            IDamageable damageable = null;

            if (GraphAgent.GetVariable("TargetGameObject", out BlackboardVariable<GameObject> target)
                && target != null)
            {
                endPosition = target.Value.transform.position + Vector3.up;
                damageable = target.Value.GetComponent<IDamageable>();
            }
            else if (GraphAgent.GetVariable("TargetLocation", out BlackboardVariable<Vector3> targetLocation))
            {
                endPosition = targetLocation;
            }
            
            StartCoroutine(AnimateGrenadeMovement(startPosition, endPosition, damageable));
        }

        private IEnumerator AnimateGrenadeMovement(Vector3 startPosition, Vector3 endPosition, IDamageable damageable)
        {
            float time = 0;
            const float speed = 2;
            while (time < 1)
            {
                grenade.transform.position = Vector3.Lerp(startPosition, endPosition, time);
                time += Time.deltaTime * speed;
                yield return null;
            }
            
            ApplyDamage(endPosition, damageable);

            explosionParticles.transform.SetParent(null);
            explosionParticles.transform.position = endPosition;
            explosionParticles.Play();

            grenade.transform.SetParent(_grenadeParent);
            grenade.transform.localPosition = _defaultGrenadePosition;
        }

        private void ApplyDamage(Vector3 endPosition, IDamageable damageable)
        {
            damageable?.TakeDamage(_unitSO.AttackConfig.Damage);

            if (_unitSO.AttackConfig.IsAreaOfEffect)
            {
                int writes = Physics.OverlapSphereNonAlloc(endPosition, _unitSO.AttackConfig.AreaEffectRadius,
                    _splashHits, _unitSO.AttackConfig.DamageableLayers);
                for (int i = 0; i < writes; i++)
                {
                    if (_splashHits[i].TryGetComponent(out IDamageable nearbyDamageable) 
                        && damageable != nearbyDamageable)
                    {
                        nearbyDamageable.TakeDamage(
                            _unitSO.AttackConfig.CalculateAreaOfEffectDamage(endPosition,
                                nearbyDamageable.Transform.position));
                    }
                }
            }
        }
    }
}