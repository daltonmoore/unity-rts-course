using System.Collections.Generic;
using System.Linq;
using Commands;
using EventBus;
using Events;
using UI.Components;
using Units;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Containers
{
    public class ActionsUI : MonoBehaviour, IUIElement<HashSet<AbstractCommandable>>
    {
        [SerializeField] private UIActionButton[] actionButtons;

        public void EnableFor(HashSet<AbstractCommandable> selectedUnits)
        {
            RefreshButtons(selectedUnits);
        }

        public void Disable()
        {
            foreach (UIActionButton button in actionButtons)
            {
                button.Disable();
            }
        }
        
        private void RefreshButtons(HashSet<AbstractCommandable> selectedUnits)
        {
            IEnumerable<BaseCommand> availableCommands = selectedUnits.ElementAt(0).AvailableCommands;

            foreach (AbstractCommandable commandable in selectedUnits)
            {
                if (commandable.AvailableCommands != null)
                {
                    availableCommands = availableCommands.Intersect(commandable.AvailableCommands);
                }
            }

            for (int i = 0; i < actionButtons.Length; i++)
            {
                BaseCommand commandForSlot = availableCommands.FirstOrDefault(action => action.Slot == i);

                if (commandForSlot is not null)
                {
                    actionButtons[i].EnableFor(commandForSlot, selectedUnits, HandleClick(commandForSlot));
                }
                else
                {
                    actionButtons[i].Disable();
                }
            }
        }

        private UnityAction HandleClick(BaseCommand action)
        {
            return () => Bus<CommandSelectedEvent>.Raise(Owner.Player1, new CommandSelectedEvent(action));
        }
    }
}
