using System.Collections.Generic;
using TMPro;
using UI.Components;
using Units;
using UnityEngine;

namespace UI.Containers
{
    public class UnitTransportUI : MonoBehaviour, IUIElement<ITransporter>
    {
        [SerializeField] private UIUnitButton[] loadedUnitButtons;
        [SerializeField] private TextMeshProUGUI capacityText;

        private ITransporter _transporter;
        
        private const string CAPACITY_TEXT_FORMAT = "{0} / {1}";
        
        public void EnableFor(ITransporter item)
        {
            gameObject.SetActive(true);
            _transporter = item;
            capacityText.SetText(string.Format(CAPACITY_TEXT_FORMAT, _transporter.UsedCapacity, _transporter.Capacity));

            SetupLoadedUnitButtons();
        }

        private void SetupLoadedUnitButtons()
        {
            List<ITransportable> loadedUnits = _transporter.GetLoadedUnits();

            for (int i = 0; i < loadedUnitButtons.Length; i++)
            {
                if (i < loadedUnits.Count)
                {
                    int index = i;
                    loadedUnitButtons[i].EnableFor(loadedUnits[i], () => HandleClick(loadedUnits[index], index));
                }
                else
                {
                    loadedUnitButtons[i].Disable();
                }
            }
        }

        private void HandleClick(ITransportable transportable, int index)
        {
            Debug.Log($"Unload {transportable.Transform.name}");
            if (_transporter.Unload(transportable))
            {
                loadedUnitButtons[index].Disable();
            }
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            foreach (UIUnitButton button in loadedUnitButtons)
            {
                button.Disable();
            }
        }
    }
}