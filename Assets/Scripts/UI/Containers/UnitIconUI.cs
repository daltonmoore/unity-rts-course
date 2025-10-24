using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Containers
{
    public class UnitIconUI : MonoBehaviour, IUIElement<AbstractCommandable>
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI healthText;
        
        private const string HEALTH_TEXT_FORMAT = "{0} / {1}";
        
        public void EnableFor(AbstractCommandable commandable)
        {
            gameObject.SetActive(true);
            healthText.SetText(string.Format(HEALTH_TEXT_FORMAT, commandable.CurrentHealth, commandable.MaxHealth));
            icon.sprite = commandable.UnitSO.Icon;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}