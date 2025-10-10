using System;
using System.Collections.Generic;
using Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Components
{
    [RequireComponent(typeof(Button))]
    public class UIBuildQueueButton : MonoBehaviour, IUIElement<AbstractUnitSO, UnityAction>
    {
        [SerializeField] private Image icon;
        
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            Disable();
        }

        public void EnableFor(AbstractUnitSO item, UnityAction callback)
        {
            _button.onClick.RemoveAllListeners();
            _button.interactable = true;
            _button.onClick.AddListener(callback);
            icon.gameObject.SetActive(true);
            icon.sprite = item.Icon;
        }

        public void Disable()
        {
            _button.interactable = false;
            _button.onClick.RemoveAllListeners();
            icon.gameObject.SetActive(false);
        }
    }
}