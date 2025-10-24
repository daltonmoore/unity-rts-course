using EventBus;
using Units;

namespace Events
{
    public struct BuildingSpawnEvent : IEvent
    {
        public BaseBuilding Building { get; private set; }
        
        public BuildingSpawnEvent(BaseBuilding building)
        {
            Building = building;
        }
    }
}