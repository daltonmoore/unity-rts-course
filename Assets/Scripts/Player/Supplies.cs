using System;
using Environment;
using EventBus;
using Events;
using TMPro;
using UnityEngine;

namespace Player
{
    public class Supplies : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mineralsText;
        [SerializeField] private TextMeshProUGUI gasText;
        [SerializeField] private TextMeshProUGUI populationText;
        
        [SerializeField] private SupplySO mineralsSO;
        [SerializeField] private SupplySO gasSO;
        
        public static int Minerals { get; private set; }
        public static int Gas { get; private set; }
        public static int Population { get; private set; }
        public static int PopulationLimit { get; private set; }

        private void Start()
        {
            // Bus<SupplyEvent>.OnEvent += HandleSupplyEvent;
        }

        private void OnDestroy()
        {
            // Bus<SupplyEvent>.OnEvent -= HandleSupplyEvent;
        }

        private void HandleSupplyEvent(SupplyEvent args)
        {
            if (args.SupplySO.Equals(mineralsSO))
            {
                Minerals += args.Amount;
                mineralsText.SetText(Minerals.ToString());
            }
            else if (args.SupplySO.Equals(gasSO))
            {
                Gas += args.Amount;
                gasText.SetText(Gas.ToString());
            }
        }
    }
}