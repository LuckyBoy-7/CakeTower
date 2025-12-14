using System;
using Lucky.Audio;
using Lucky.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace CakeTower.Scripts
{
    public class CakePlacer : MonoBehaviour
    {
        private Transform cakeLayerTransform;
        private bool[,] cakeSegmentsData;

        [SerializeField] private CakeMover mover;
        [SerializeField] private CakeTower cakeTower;
        public bool bonus;

        private void Start()
        {
            Restart();
        }

        public void Restart()
        {
            if (cakeLayerTransform != null)
                Destroy(cakeLayerTransform.gameObject);
            (cakeLayerTransform, cakeSegmentsData) = cakeTower.GetTopLayerCopy();
            cakeLayerTransform.parent = transform;
            cakeLayerTransform.position = transform.position;
        }

        private void Update()
        {
            if (GameManager.Instance.GameOver)
                return;
            if (Input.GetKeyDown(KeyCode.Space))
                PlaceCake();
        }

        private void PlaceCake()
        {
            Vector3 offset = (transform.position + Vector3.one * 0.5f);
            Vector2Int delta = new Vector2(offset.x, offset.z).FloorToVector2Int();
            cakeTower.TryPlace(cakeSegmentsData, delta);
        }

        public void GetPerfectPlacementStreakBonus()
        {
            if (cakeLayerTransform != null)
                Destroy(cakeLayerTransform.gameObject);
            (cakeLayerTransform, cakeSegmentsData) = cakeTower.GetFullLayerCopy();
            cakeLayerTransform.parent = transform;
            cakeLayerTransform.position = transform.position;
            bonus = true;
        }
    }
}