using EventBus;
using Units;

namespace Events
{
    public struct UnitLoadEvent : IEvent
    {
        public ITransportable Transportable;
        public ITransporter Transporter;
        
        public UnitLoadEvent(ITransportable transportable, ITransporter transporter)
        {
            Transportable = transportable;
            Transporter = transporter;
        }
    }
}