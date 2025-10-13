using EventBus;
using Units;

namespace Events
{
    public struct UnitDeathEvent : IEvent
    {
        public AbstractUnit Unit { get; private set; }
        
        public UnitDeathEvent(AbstractUnit unit)
        {
            Unit = unit;
        }
    }
}