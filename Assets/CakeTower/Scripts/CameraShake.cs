using DG.Tweening;
using UnityEngine;

namespace CakeTower.Scripts
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private float shakeDuration = 0.5f;
        [SerializeField] private float shakeMagnitude = 0.5f;
        [SerializeField] private CakeTower cakeTower;

        public void Shake()
        {
            transform.DOShakePosition(shakeDuration, shakeMagnitude);
        }

        private void OnEnable()
        {
            cakeTower.OnPlacement += OnPerfectPlacement;
        }

        private void OnDisable()
        {
            cakeTower.OnPlacement -= OnPerfectPlacement;
        }

        private void OnPerfectPlacement(CakeTower.PlacementResult result)
        {
            Shake();
        }
    }
}