using System.Collections.Generic;
using EventBus;
using Events;
using UI.Containers;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class RuntimeUI : MonoBehaviour
    {
        [SerializeField] public ActionsUI actionsUI;
     
        private HashSet<AbstractCommandable> _commandables = new (12);
        
        private void Awake()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
        }

        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
        }

        private void HandleUnitSelected(UnitSelectedEvent evt)
        {
            if (evt.Unit is AbstractCommandable commandable)
            {
                _commandables.Add(commandable);
                actionsUI.EnableFor(_commandables);
            }
        }
        
        private void HandleUnitDeselected(UnitDeselectedEvent evt)
        {
            if (evt.Unit is AbstractCommandable commandable)
            {
                _commandables.Remove(commandable);
                if (_commandables.Count == 0)
                {
                    actionsUI.Disable();
                }
            }
        }
    }
}