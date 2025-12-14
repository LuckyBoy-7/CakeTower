using System;
using Lucky.Animation.Color;
using Lucky.Extensions;
using UnityEngine;

namespace Lucky.Interactive.Impl
{
    [RequireComponent(typeof(ColorController))]
    public class Button : InteractableUI
    {
        public Color normalColor = Color.white;
        public Color disabledColor = Color.white;
        public Color hoverColor = Color.white.WithA(0.2f);
        public Color pressColor = Color.white;

        private ColorController colorController;

        public Action OnButtonClicked;

        public bool dontUpdateColor;

        private void Awake()
        {
            colorController = GetComponent<ColorController>();
        }

        private void Update()
        {
            if (dontUpdateColor)
                return;

            if (!CanInteract)
            {
                UpdateColor(disabledColor);
                return;
            }

            if (pressing)
                UpdateColor(pressColor);
            else if (CursorInBounds())
            {
                UpdateColor(hoverColor);
            }
            else
            {
                UpdateColor(normalColor);
            }
        }

        public void UpdateColor(Color color)
        {
            colorController.Color = color;
        }

        public void ResetColor()
        {
            UpdateColor(normalColor);
        }


        protected override void OnCursorClick()
        {
            base.OnCursorClick();
            OnButtonClicked?.Invoke();
        }

        public void Enable()
        {
            CanInteract = true;
            dontUpdateColor = false;
            ResetColor();
        }

        public void Disable()
        {
            CanInteract = false;
            dontUpdateColor = true;
            ResetColor();
        }
    }
}