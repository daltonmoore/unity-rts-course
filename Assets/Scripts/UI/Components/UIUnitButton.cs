using System;
using Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Components
{
    [RequireComponent(typeof(Button))]
    public class UIUnitButton : MonoBehaviour, IUIElement<ITransportable, UnityAction>
    {
        [SerializeField] private Image icon;
        
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            Disable();
        }

        public void EnableFor(ITransportable item, UnityAction callback)
        {
            _button.onClick.RemoveAllListeners();
            gameObject.SetActive(true);
            
            icon.sprite = item.Icon;
            _button.onClick.AddListener(callback);
        }

        public void Disable()
        {
            _button.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
        }
    }
}