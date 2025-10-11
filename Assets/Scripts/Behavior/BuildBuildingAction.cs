using System;
using Units;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Build Building", story: "[Self] builds [BuildingSO] at [TargetLocation]", category: "Action/Units", id: "93151e5e97a69263d0ec7a5eb40c8824")]
    public partial class BuildBuildingAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Self;
        [SerializeReference] public BlackboardVariable<BuildingSO> BuildingSO;
        [SerializeReference] public BlackboardVariable<Vector3> TargetLocation;

        private float _startBuildTime;
        private BaseBuilding _completedBuilding;
        private Vector3 _startPosition;
        
        protected override Status OnStart()
        {
            if (!HasValidInputs()) return Status.Failure;
            
            _startBuildTime = Time.time;
            GameObject building = GameObject.Instantiate(BuildingSO.Value.Prefab);
            _completedBuilding = building.GetComponent<BaseBuilding>();
            _startPosition = TargetLocation.Value - Vector3.up * _completedBuilding.MainRenderer.bounds.size.y;
            _completedBuilding.transform.position = _startPosition;
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            float normalizedTime = (Time.time - _startBuildTime) / BuildingSO.Value.BuildTime;
            _completedBuilding.transform.position = Vector3.Lerp(_startPosition, TargetLocation.Value, normalizedTime);
            
            return normalizedTime >= 1 ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
            if (CurrentStatus == Status.Success)
            {
                _completedBuilding.enabled = true;
            }
        }
        
        private bool HasValidInputs()
        {
            return Self.Value != null
                   && BuildingSO.Value != null
                   && BuildingSO.Value.Prefab != null;
        }
    }
}

