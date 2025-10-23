using Commands;
using Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Components
{
    [RequireComponent(typeof(Button))]
    public class UIActionButton : MonoBehaviour, IUIElement<BaseCommand, UnityAction>, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Tooltip tooltip;

        private bool _isActive;
        private RectTransform _rectTransform;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _rectTransform = GetComponent<RectTransform>();
            Disable();
        }

        public void EnableFor(BaseCommand command, UnityAction onClick)
        {
            _button.onClick.RemoveAllListeners();
            SetIcon(command.Icon);
            _button.interactable = !command.IsLocked(new CommandContext());
            _button.onClick.AddListener(onClick);
            _isActive = true;
            
            if (tooltip != null)
            {
                tooltip.SetText(GetTooltipText(command));
            }
        }

        public void Disable()
        {
            SetIcon(null);
            _button.interactable = false;
            _button.onClick.RemoveAllListeners();
            _isActive = false;
            
            if (tooltip != null)
            {
                tooltip.Hide();
            }
            CancelInvoke();
        }

        private void SetIcon(Sprite icon)
        {
            if (icon is null)
            {
                this.icon.enabled = false;
            }
            else
            {
                this.icon.sprite = icon;
                this.icon.enabled = true;
            }
        }

        private void ShowTooltip()
        {
            if (tooltip != null)
            {
                tooltip.Show();
                tooltip.RectTransform.anchorMin = new Vector2(1f, 1f);
                tooltip.RectTransform.anchorMax = new Vector2(1f, 1f);
                tooltip.RectTransform.anchoredPosition = Vector2.zero;
                tooltip.RectTransform.pivot = new Vector2(1f, 0f);
            }
        }
        
        private string GetTooltipText(BaseCommand command)
        {
            string tooltipText = command.Name + "\n";
            
            SupplyCostSO supplyCostSO = null;
            if (command is BuildUnitCommand unitCommand)
            {
                supplyCostSO = unitCommand.Unit.Cost;
            }
            else if (command is BuildBuildingCommand buildingCommand)
            {
                supplyCostSO = buildingCommand.BuildingSO.Cost;
            }
            
            if (supplyCostSO != null)
            {
                if (supplyCostSO.Minerals > 0)
                {
                    tooltipText += $"{supplyCostSO.Minerals} Minerals.";
                }
                
                if (supplyCostSO.Gas > 0)
                {
                    tooltipText += $"{supplyCostSO.Gas} Gas.";
                }
            }
            
            return tooltipText;
        }

        public void OnPointerEnter(PointerEventData _)
        {
            if (_isActive)
            {
                Invoke(nameof(ShowTooltip), tooltip != null ? tooltip.HoverDelay : 0.5f);
            }
        }

        public void OnPointerExit(PointerEventData _)
        {
            if (tooltip != null)
            {
                tooltip.Hide();
            }
            CancelInvoke();
        }
    }
}
