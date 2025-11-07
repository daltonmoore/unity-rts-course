using UnityEngine;

namespace Units
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Units/Unit")]
    public class UnitSO : AbstractUnitSO
    {
        [field: SerializeField] public AttackConfigSO AttackConfig { get; private set; }
        [field: SerializeField] public TransportConfigSO TransportConfig { get; private set; }
    }
}