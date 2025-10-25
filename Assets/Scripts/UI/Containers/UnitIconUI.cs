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
        
        private AbstractCommandable _commandable;
        
        private const string HEALTH_TEXT_FORMAT = "{0} / {1}";
        
        public void EnableFor(AbstractCommandable commandable)
        {
            gameObject.SetActive(true);
            healthText.SetText(string.Format(HEALTH_TEXT_FORMAT, commandable.CurrentHealth, commandable.MaxHealth));
            icon.sprite = commandable.UnitSO.Icon;
            _commandable = commandable;
            _commandable.OnHealthUpdated -= OnHealthUpdated;
            _commandable.OnHealthUpdated += OnHealthUpdated;
        }

        public void Disable()
        {
            gameObject.SetActive(false);

            if (_commandable != null)
            {
                _commandable.OnHealthUpdated -= OnHealthUpdated;
                _commandable = null;
            }
        }
        
        private void OnHealthUpdated(AbstractCommandable commandable, int _, int currentHealth)
        {
            healthText.SetText(string.Format(HEALTH_TEXT_FORMAT, currentHealth, commandable.MaxHealth));
        }
    }
}