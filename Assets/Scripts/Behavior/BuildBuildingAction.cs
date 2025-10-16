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
        [SerializeReference] public BlackboardVariable<BaseBuilding> BuildingUnderConstruction;

        private float _startBuildTime;
        private BaseBuilding _completedBuilding;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private Transform _rendererTransform;
        
        protected override Status OnStart()
        {
            if (!HasValidInputs()) return Status.Failure;

            if (BuildingUnderConstruction.Value == null)
            {
                GameObject building = GameObject.Instantiate(BuildingSO.Value.Prefab);
                
                if (!building.TryGetComponent(out _completedBuilding) || _completedBuilding.MainRenderer == null) 
                    return Status.Failure;
            }
            else
            {
                _completedBuilding = BuildingUnderConstruction.Value;
            }
            
            _completedBuilding.StartBuilding(Self.Value.GetComponent<IBuildingBuilder>());
            _startBuildTime = _completedBuilding.Progress.StartTime;
            
            BuildingUnderConstruction.Value = _completedBuilding;
            _rendererTransform = _completedBuilding.MainRenderer.transform;
            
            _startPosition = TargetLocation.Value - Vector3.up * _completedBuilding.MainRenderer.bounds.size.y;
            _endPosition = TargetLocation.Value;
            _completedBuilding.transform.position = _endPosition;
            _rendererTransform.position = _rendererTransform.InverseTransformPoint(_startPosition);
            
            return OnUpdate();
        }

        protected override Status OnUpdate()
        {
            float normalizedTime = (Time.time - _startBuildTime) / BuildingSO.Value.BuildTime;
            _rendererTransform.position = Vector3.Lerp(_startPosition, _endPosition, normalizedTime);
            
            return normalizedTime >= 1 ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
            if (CurrentStatus == Status.Success)
            {
                _completedBuilding.enabled = true;
                _completedBuilding.ResetDefaultVisuals();
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

