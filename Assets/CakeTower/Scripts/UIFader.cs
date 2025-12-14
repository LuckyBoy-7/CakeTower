using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace CakeTower.Scripts
{
    public class UIFader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float duration;

        public float FadeOut()
        {
            canvasGroup.DOFade(0, duration);
            return duration;
        }

        public float FadeIn()
        {
            canvasGroup.DOFade(1, duration);
            return duration;
        }
    }
}