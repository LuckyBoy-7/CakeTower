using System;
using System.Collections.Generic;
using Lucky.Collections;
using Lucky.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace CakeTower.Scripts
{
    public class CakeTower : MonoBehaviour
    {
        public const int CakeSize = 10;
        [SerializeField] private GameObject cakeSegmentPrefab;
        [SerializeField] private CakePlacer cakePlacer;
        private DefaultDict<int, bool[,]> layerToCakeSegments = new(() => new bool[CakeSize, CakeSize]);
        private DefaultDict<int, Transform> layerToParent = new(() => new GameObject("CakeLayer").transform);
        public int MaxLayer { get; private set; }
        public float MaxLayerHeight { get; private set; }

        public Action<PlacementResult> OnPlacement;

        public enum PlacementResult
        {
            Perfect,
            Normal,
            Failed
        }

        private void Awake()
        {
            FillCakeLayer(0);
        }

        private void FillCakeLayer(int layer)
        {
            for (int x = 0; x < CakeSize; x++)
            {
                for (int y = 0; y < CakeSize; y++)
                {
                    SetCake(layer, x, y);
                }
            }
        }

        private void SetCake(int layer, int x, int y)
        {
            MaxLayer = Mathf.Max(MaxLayer, layer);
            layerToCakeSegments[layer][x, y] = true;

            Transform cakeLayer = layerToParent[layer];
            cakeLayer.parent = transform;

            Vector3 startPivot = -new Vector3(CakeSize, 1, CakeSize) / 2;
            Vector3 cakeSegmentPosition = startPivot + new Vector3(x, layer, y);
            cakeLayer.transform.position = cakeLayer.transform.position.WithY(cakeSegmentPosition.y);

            Transform cakeSegment = Instantiate(cakeSegmentPrefab, cakeSegmentPosition, Quaternion.identity).transform;
            cakeSegment.parent = cakeLayer;


            MaxLayerHeight = Mathf.Max(MaxLayerHeight, cakeSegmentPosition.y);
        }

        private int SetCake(int x, int y)
        {
            for (int i = 0; i <= MaxLayer + 1; i++)
            {
                if (!layerToCakeSegments[i][x, y])
                {
                    SetCake(i, x, y);
                    return i;
                }
            }

            return 0;
        }

        public (Transform, bool[,]) GetLayerCopy(int layer)
        {
            Transform layerCopy = Instantiate(layerToParent[layer]);
            return (layerCopy, (bool[,])layerToCakeSegments[layer].Clone());
        }

        public (Transform, bool[,]) GetTopLayerCopy() => GetLayerCopy(MaxLayer);

        public (Transform cakeLayerTransform, bool[,] cakeSegmentsData) GetFullLayerCopy() => GetLayerCopy(0);

        public PlacementResult TryPlace(bool[,] cakeSegmentsData, Vector2Int delta)
        {
            int startX = delta.x;
            int startY = delta.y;
            int newMaxLayer = MaxLayer + 1;
            bool anyPlacedOnTopLayer = false;
            for (int x = 0; x < CakeSize; x++)
            {
                for (int y = 0; y < CakeSize; y++)
                {
                    int nx = x + startX;
                    int ny = y + startY;
                    if (nx is >= 0 and < CakeSize && ny is >= 0 and < CakeSize)
                    {
                        if (cakeSegmentsData[x, y])
                        {
                            int layer = SetCake(nx, ny);
                            if (layer == newMaxLayer)
                                anyPlacedOnTopLayer = true;
                        }
                    }
                }
            }

            PlacementResult result = PlacementResult.Failed;

            if (delta == Vector2Int.zero)
            {
                result = PlacementResult.Perfect;
            }
            else if (cakePlacer.bonus || anyPlacedOnTopLayer)
            {
                cakePlacer.bonus = false;
                result = PlacementResult.Normal;
            }

            OnPlacement?.Invoke(result);
            return result;
        }
    }
}