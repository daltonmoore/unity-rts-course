using System;
using UnityEngine;

namespace UI.Components
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Vector2 padding;
        [SerializeField] private RectTransform mask;
        private RectTransform _maskParentRectTransform;


        //TODO: remove progress and update
        [SerializeField] [Range(0,1)] private float progress;

        private void Update()
        {
            SetProgress(progress);
        }

        private void Awake()
        {
            if (mask is null)
            {
                Debug.LogError($"Progress bar {name} is missing a mask! This progress bar will not work!");
                return;
            }
            
            _maskParentRectTransform = mask.parent.GetComponent<RectTransform>();
        }

        public void SetProgress(float progress)
        {
            Vector2 parentSize = _maskParentRectTransform.sizeDelta;
            Vector2 targetSize = parentSize - padding * 2;

            targetSize.x *= Mathf.Clamp01(progress);
            
            mask.offsetMin = padding;
            mask.offsetMax = new Vector2(padding.x + targetSize.x - parentSize.x, -padding.y);
        }
    }
}