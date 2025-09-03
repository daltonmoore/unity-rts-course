using EventBus;
using Units;

namespace Events
{
    public struct UnitSpawnEvent : IEvent
    {
        public AbstractUnit Unit { get; private set; }
        
        public UnitSpawnEvent(AbstractUnit unit)
        {
            Unit = unit;
        }
    }
}