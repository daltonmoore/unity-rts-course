using UnityEngine;
using UnityEngine.AI;

namespace Commands
{
    [CreateAssetMenu(fileName = "BuildingRestriction", menuName = "Buildings/Restrictions", order = 7)]
    public class BuildingRestrictionSO : ScriptableObject
    {
        [field: SerializeField] public bool MustBeFullyOnNavMesh { get; private set; } = true;
        [field: SerializeField] public int NavMeshAgentTypeId { get; private set; }
        [field: SerializeField] public float NavMeshTolerance { get; private set; } = 0.1f;

        [field: SerializeField] public Vector3 Extents { get; private set; } = Vector3.one;
        
        public bool CanPlace(Vector3 position)
        {
            bool isOnNavMesh = true;

            if (MustBeFullyOnNavMesh)
            {
                NavMeshQueryFilter queryFilter = new()
                {
                    areaMask = NavMesh.AllAreas,
                    agentTypeID = NavMeshAgentTypeId
                };

                isOnNavMesh = IsFullyOnNavMesh(position, queryFilter);

                return isOnNavMesh;
            }

            return true;
        }

        private bool IsFullyOnNavMesh(Vector3 position, NavMeshQueryFilter queryFilter)
        {
            bool isOnNavMesh = NavMesh.SamplePosition(
                position + new Vector3(Extents.x, 0, Extents.z),
                out NavMeshHit _,
                NavMeshTolerance,
                queryFilter
            );
                
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(
                position + new Vector3(Extents.x, 0, -Extents.z),
                out NavMeshHit _,
                NavMeshTolerance,
                queryFilter
            );
                
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(
                position + new Vector3(-Extents.x, 0, -Extents.z),
                out NavMeshHit _,
                NavMeshTolerance,
                queryFilter
            );
                
            isOnNavMesh = isOnNavMesh && NavMesh.SamplePosition(
                position + new Vector3(-Extents.x, 0, Extents.z),
                out NavMeshHit _,
                NavMeshTolerance,
                queryFilter
            );
            return isOnNavMesh;
        }
    }
}