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

            if (GraphAgent.GetVariable("TargetGameObject", out BlackboardVariable<GameObject> target)
                && target != null)
            {
                endPosition = target.Value.transform.position + Vector3.up;
            }
            else if (GraphAgent.GetVariable("TargetLocation", out BlackboardVariable<Vector3> targetLocation))
            {
                endPosition = targetLocation;
            }
            
            StartCoroutine(AnimateGrenadeMovement(startPosition, endPosition));
        }

        private IEnumerator AnimateGrenadeMovement(Vector3 startPosition, Vector3 endPosition)
        {
            float time = 0;
            const float speed = 2;
            while (time < 1)
            {
                grenade.transform.position = Vector3.Lerp(startPosition, endPosition, time);
                time += Time.deltaTime * speed;
                yield return null;
            }
            
            explosionParticles.transform.SetParent(null);
            explosionParticles.transform.position = endPosition;
            explosionParticles.Play();

            grenade.transform.SetParent(_grenadeParent);
            grenade.transform.localPosition = _defaultGrenadePosition;
        }
    }
}