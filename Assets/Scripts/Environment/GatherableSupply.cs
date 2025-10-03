using System;
using UnityEngine;

namespace Environment
{
    public class GatherableSupply : MonoBehaviour, IGatherable
    {
        [field: SerializeField] public SupplySO SupplySO { get; private set; }
        [field: SerializeField] public int Amount { get; private set; }
        [field: SerializeField] public bool IsBusy { get; private set; }

        private void Start()
        {
            Amount = SupplySO.MaxAmount;
        }

        public bool BeginGather()
        {
            if (IsBusy) 
            {
                return false;
            }
            
            IsBusy = true;
            return true;
        }

        public int EndGather()
        {
            IsBusy = false;
            int amountGathered = Mathf.Min(Amount, SupplySO.AmountPerGather);
            Amount -= amountGathered;
            
            if (Amount <= 0)
            {
                Destroy(gameObject);
            }
            
            return amountGathered;
        }
    }
}