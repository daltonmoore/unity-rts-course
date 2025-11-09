using Units;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    public interface ITransportable
    {
        Transform Transform { get; }
        public int TransportCapacityUsage { get; }
        public NavMeshAgent Agent { get; }
        public Sprite Icon { get; }
        
        public void LoadInto(ITransporter transporter);
    }
}
