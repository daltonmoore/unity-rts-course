using EventBus;
using Units;

namespace Events
{
    public struct UnitUnloadEvent : IEvent
    {
        public ITransportable Transportable;
        public ITransporter Transporter;
        
        public UnitUnloadEvent(ITransportable transportable, ITransporter transporter)
        {
            Transportable = transportable;
            Transporter = transporter;
        }
    }
}