using Environment;
using EventBus;
using Units;

namespace Events
{
    public struct SupplyEvent : IEvent
    {
        public int Amount { get; private set; }
        public SupplySO SupplySO { get; private set; }
        public Owner Owner { get; private set; }
        
        public SupplyEvent(Owner owner, int amount, SupplySO supplySO)
        {
            Owner = owner;
            Amount = amount;
            SupplySO = supplySO;
        }
    }
}