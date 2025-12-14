using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lucky.Animation.Color
{
    public class ColorController : MonoBehaviour
    {
        public bool affectColor = true;
        public bool affectAlpha = true;

        public List<MaskableGraphic> graphics;


        private UnityEngine.Color _color = UnityEngine.Color.white;

        public UnityEngine.Color Color
        {
            get => _color;
            set
            {
                UnityEngine.Color newColor = _color;
                if (affectColor)
                {
                    newColor.r = value.r;
                    newColor.g = value.g;
                    newColor.b = value.b;
                }

                if (affectAlpha)
                    newColor.a = value.a;
                if (_color != newColor)
                {
                    _color = newColor;
                    UpdateColor();
                }
            }
        }

        public float Alpha
        {
            get => _color.a;
            set
            {
                if (_color.a != value)
                {
                    _color.a = value;
                    UpdateColor();
                }
            }
        }

        private void Awake()
        {
            UpdateColor();
        }

        private void UpdateColor()
        {
            foreach (var graphic in graphics)
            {
                graphic.color = Color;
            }
        }
    }
}