using Units;
using UnityEngine;

namespace Units
{
    public interface ITransportable
    {
        Transform Transform { get; }
        public int TransportCapacityUsage { get; }
        
        public void LoadInto(ITransporter transporter);
    }
}
