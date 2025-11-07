
using Behavior;
using Commands;
using Environment;
using EventBus;
using Events;
using Unity.Behavior;
using UnityEngine;
using Utilities;

namespace Units
{
    public class Worker : AbstractUnit, IBuildingBuilder, ITransportable
    {
        public int TransportCapacityUsage => _unitSO.TransportConfig.GetTransportCapacityUsage();
        
        public bool IsBuilding => GraphAgent.GetVariable("Command", out BlackboardVariable<UnitCommands> command) 
                                  && command.Value == UnitCommands.BuildBuilding;
        public bool HasSupplies {
            get
            {
                if (GraphAgent != null &&
                    GraphAgent.GetVariable("SupplyAmountHeld", out BlackboardVariable<int> heldAmount))
                {
                    return heldAmount.Value > 0;
                }

                return false;
            }
        }
        
        [SerializeField] private BaseCommand cancelBuildingCommand;
        
        protected override void Start()
        {
            base.Start();
            if (GraphAgent.GetVariable("GatherSuppliesEvent",
                    out BlackboardVariable<GatherSuppliesEventChannel> eventChannelVariable))
            {
                eventChannelVariable.Value.Event += HandleGatherSupplies;
            }
            
            if (GraphAgent.GetVariable("BuildingEventChannel",
                    out BlackboardVariable<BuildingEventChannel> buildingEventChannel))
            {
                buildingEventChannel.Value.Event += HandleBuildingEvent;
            }
            
        }

        public void LoadInto(ITransporter transporter)
        {
            throw new System.NotImplementedException();
        }
        
        public void Gather(GatherableSupply supply)
        {
            GraphAgent.SetVariableValue("GatherableSupply", supply);
            GraphAgent.SetVariableValue("Command", UnitCommands.Gather);
        }

        public void ReturnSupplies(GameObject commandPost)
        {
            GraphAgent.SetVariableValue("CommandPost", commandPost);
            GraphAgent.SetVariableValue("Command", UnitCommands.ReturnSupplies);
        }

        public GameObject Build(BuildingSO building, Vector3 targetLocation)
        {
            GameObject instance = Instantiate(building.Prefab, targetLocation, Quaternion.identity);
            
            if (!instance.TryGetComponent(out BaseBuilding _))
            {
                Debug.LogError($"Missing BaseBuilding on Prefab for BuildingSO \"{building.name}\"! Cannot build!");
                return null;
            }
            
            GraphAgent.SetVariableValue("Ghost", instance);
            GraphAgent.SetVariableValue("BuildingSO", building);
            GraphAgent.SetVariableValue("TargetLocation", targetLocation);
            GraphAgent.SetVariableValue("Command", UnitCommands.BuildBuilding);
            
            SetCommandOverrides(new[] { cancelBuildingCommand });
            Bus<SupplyEvent>.Raise(new SupplyEvent(-building.Cost.Minerals, building.Cost.MineralsSO));
            Bus<SupplyEvent>.Raise(new SupplyEvent(-building.Cost.Gas, building.Cost.GasSO));
            
            return instance;
        }

        public void ResumeBuilding(BaseBuilding building)
        {
            GraphAgent.SetVariableValue<GameObject>("Ghost", null);
            GraphAgent.SetVariableValue("BuildingUnderConstruction", building);
            GraphAgent.SetVariableValue("BuildingSO", building.BuildingSO);
            GraphAgent.SetVariableValue("TargetLocation", building.transform.position);
            GraphAgent.SetVariableValue("Command", UnitCommands.BuildBuilding);
        }

        public void CancelBuilding()
        {
            if (GraphAgent.GetVariable("Ghost", out BlackboardVariable<GameObject> ghost) 
                && ghost.Value != null)
            {
                Destroy(ghost.Value);
            }
            
            if (GraphAgent.GetVariable("BuildingUnderConstruction", out BlackboardVariable<BaseBuilding> building) 
                && building.Value != null)
            {
                Destroy(building.Value.gameObject);
                
                BuildingSO buildingSO = building.Value.BuildingSO;
                Bus<SupplyEvent>.Raise(new SupplyEvent(Mathf.FloorToInt(0.75f * buildingSO.Cost.Minerals), buildingSO.Cost.MineralsSO));
                Bus<SupplyEvent>.Raise(new SupplyEvent(Mathf.FloorToInt(0.75f * buildingSO.Cost.Gas), buildingSO.Cost.GasSO));
            }
            
            GraphAgent.SetVariableValue<BaseBuilding>("BuildingUnderConstruction", null);
            SetCommandOverrides(null);
            Stop();
        }

        public override void Deselect()
        {
            if (decal != null)
            {
                decal.gameObject.SetActive(false);
            }
            
            IsSelected = false;

            if (!IsBuilding)
            {
                SetCommandOverrides(null);
            }
            
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this));
        }

        private void HandleGatherSupplies(GameObject self, int amount, SupplySO supplySO)
        {
            Bus<SupplyEvent>.Raise(new SupplyEvent(amount, supplySO));
        }
        
        private void HandleBuildingEvent(GameObject self, BuildingEventType eventType, BaseBuilding building)
        {
            switch (eventType)
            {
                case BuildingEventType.ArrivedAt:
                    if (building != null && building.Progress.State == BuildingProgress.BuildingState.Building)
                    {
                        Stop();
                        break;
                    }
                    SetCommandOverrides(new [] { cancelBuildingCommand });
                    break;
                
                case BuildingEventType.Begin:
                    SetCommandOverrides(new [] { cancelBuildingCommand });
                    break;

                case BuildingEventType.Cancel:
                case BuildingEventType.Abort:
                case BuildingEventType.Completed:
                    SetCommandOverrides(null);
                    break;
                default:
                    break;
            }
        }
    }
}
