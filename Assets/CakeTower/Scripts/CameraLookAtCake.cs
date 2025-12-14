using System;
using Lucky.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace CakeTower.Scripts
{
    public class CameraLookAtCake : MonoBehaviour
    {
        [SerializeField] private float lerpK;
        [SerializeField] private float heightOffset;
        [SerializeField] private CakeTower cakeTower;
        private float targetHeight;
        
        [SerializeField] private float rotateDuration;
        

        private void Start()
        {
            transform.SetPositionY(heightOffset);
        }

        private void Update()
        {
            targetHeight = cakeTower.MaxLayerHeight + heightOffset;

            transform.position = Vector3.Lerp(transform.position, transform.position.WithY(targetHeight), lerpK);
        }
    }
}