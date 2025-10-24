using TMPro;
using Units;
using UnityEngine;

namespace UI.Containers
{
    public class SingleUnitSelectedUI : MonoBehaviour, IUIElement<AbstractCommandable>
    {
        [SerializeField] private TextMeshProUGUI unitNameText;
        
        public void EnableFor(AbstractCommandable commandable)
        {
            gameObject.SetActive(true);
            unitNameText.SetText(commandable.UnitSO.Name);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}