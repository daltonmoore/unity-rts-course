using Environment;
using EventBus;

namespace Events
{
    public struct SupplyEvent : IEvent
    {
        public int Amount { get; private set; }
        public SupplySO SupplySO { get; private set; }
        
        public SupplyEvent(int amount, SupplySO supplySO)
        {
            Amount = amount;
            SupplySO = supplySO;
        }
    }
}